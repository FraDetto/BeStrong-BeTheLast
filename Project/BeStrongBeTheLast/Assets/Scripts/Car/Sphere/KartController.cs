/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class KartController : aBSBTLKart
{

    public bool touchingGround = true;

    // ============== HUMAN ==============
    public bool UsaWrongWay = false;

    private float wrongWayTimer = 2, wrongWayMaxTimer = 1;

    // ============== HUMAN ==============

    // =============== CPU ===============
    private const byte errorDelta = 8;

    private GameObject[] CPUCars;
    private GameObject[] PlayersCars;
    private List<GameObject> AllCars = new List<GameObject>();

    private bool bJumpReleased;
    private float LastStuck = -1;
    private Vector3 lastPosition;

    private static HashSet<GameObject> currentObstacleOtherCPU = new HashSet<GameObject>();
    private Stack<GameObject> excludeObstacles = new Stack<GameObject>(5);
    private GameObject currentObstacle;
    // =============== CPU ===============    


    private GameObject excludeObstacle =>
        excludeObstacles.Count > 0 ? excludeObstacles.Peek() : null;

    internal bool started;


    private void Start()
    {
        CurrentSplineObject = GameObject.FindGameObjectWithTag("Spline").transform.GetChild(0).GetComponent<SplineObject>();

        CPUCars = GameObject.FindGameObjectsWithTag("CPU");
        PlayersCars = GameObject.FindGameObjectsWithTag("Player");
        AllCars.AddRange(CPUCars);
        AllCars.AddRange(PlayersCars);

        switch (KCType)
        {
            case eKCType.Human:
                setDestination(0, 0, true);
                break;

            case eKCType.CPU:
                var Box001 = GB.FindTransformInChildWithTag(transform, "Carrozzeria");
                var renderer_ = Box001.gameObject.GetComponent<Renderer>();

                var c = Color.black;

                while (c == Color.black || GB.usedColors.Contains(renderer_.material.color))
                    c = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

                GB.usedColors.Add(c);
                renderer_.material.color = c;

                setDestinationWithError();
                break;
        }

        Start_();
        started = true;
    }

    private void Update()
    {
        var wrong = wrongWay;

        switch (KCType)
        {
            case eKCType.Human:
                var input = "P" + playerNumber;

                if (Vector3.Distance(transform.position, lookAtDestOriginal) < splineDistance)
                    setDestination(0, 0, false, CurrentSplineObject);

                if (wrong || (!wrong && wrongWayTimer < wrongWayMaxTimer))
                {
                    Update_(0, false, false);
                    wrongWayTimer += Time.deltaTime;
                }
                else
                {
                    if (touchingGround)
                    {
                        driftDisabled = false;
                        Update_(GB.GetAxis(input + "Horizontal"), GB.GetButtonDown(input + "Drift"), GB.GetButtonUp(input + "Drift"));
                    }
                    else
                    {
                        driftDisabled = true;
                        Update_(0, GB.GetButtonDown(input + "Drift"), GB.GetButtonUp(input + "Drift"));
                    }
                }

                if (UsaWrongWay && wrong)
                {
                    SetOnTrack();
                    wrongWayTimer = 0;
                    driftDisabled = true;
                }
                break;

            case eKCType.CPU:
                if (Vector3.Distance(transform.position, lookAtDestOriginal) < splineDistance)
                    setDestinationWithError();

                if (CurrentSplineObject.CanBeClosedByThisWall && CurrentSplineObject.CanBeClosedByThisWall.closed)
                    setTheOtherFork();

                CPU_AI_Find_Obstacles(wrong);

                // go straight
                lookAtDest.y = transform.position.y;
                lookAtDestOriginal.y = transform.position.y;

                if (iAmBlinded)
                {
                    //TODO: sono stato accecato fare qualcosa di stupido
                }
                else
                {
                    transform.LookAt(lookAtDest);
                }

                CPU_AI_Find_UseWeapons();

                var drift_ = CurrentSplineObject.splineType == SplineObject.eSplineType.Drift;
                var jumpBUP = !bJumpReleased && drift_ && driftPower > 250;
                var jumpBDown = drift_ && !jumpBUP;
                var driftAxis = (drift_ ? 0.0001f : 0);

                if (bJumpReleased)
                    bJumpReleased = false;

                if (!drift_)
                    clearDrift();

                Update_(driftAxis, jumpBDown, jumpBUP);

                if (jumpBUP)
                    bJumpReleased = true;

                foreach (var cpu in CPUCars)
                    if (!cpu.name.Equals(name))
                        if (Vector3.Distance(cpu.transform.position, transform.position) < 0.5)
                            speed = 0;

                if (transform.position == lastPosition && currentSpeed < 1)
                {
                    if (LastStuck > -1)
                        LastStuck = Time.time;

                    if (Time.time - LastStuck > 60)
                    {
                        var p = CurrentSplineObject.transform.position;
                        transform.position = new Vector3(p.x, p.y + 2, p.z);
                    }
                }

                lastPosition = transform.position;
                break;
        }

        Debug.DrawLine(transform.position, lookAtDestOriginal, Color.green);
        Debug.DrawLine(transform.position, lookAtDest, Color.yellow);
    }

    private void FixedUpdate() =>
        FixedUpdate_();

    private bool wrongWay
    {
        get
        {
            var currentSplineDistance1 = Vector3.Distance(transform.position, curSplinePos);
            var currentSplineDistance0 = Vector3.Distance(transform.position, prevSplinePos);

            var wrong =
                lastSplineDistance > 0 &&
                prevSplineDistance > 0 &&
                currentSplineDistance1 > lastSplineDistance &&
                currentSplineDistance0 < prevSplineDistance;

            lastSplineDistance = currentSplineDistance1;
            prevSplineDistance = currentSplineDistance0;

            return wrong;
        }
    }

    public float currentSplineDistance =>
        Vector3.Distance(transform.position, curSplinePos);

    internal void setTheOtherFork() =>
        setDestinationWithError(CurrentSplineObject.CanBeClosedByThisWall_AlternativeFork);

    internal void setDestinationWithError() =>
        setDestinationWithError(CurrentSplineObject.nextRandomSpline);

    internal void setDestinationWithError(SplineObject nextSpline) =>
           setDestination(GB.NormalizedRandom(-1f, 1f) * errorDelta, GB.NormalizedRandom(-1f, 1f) * errorDelta, false, nextSpline);

    internal void nextSpline() =>
        setDestination(0, 0);

    private bool FindEnemyDirection(Vector3 direction)
    {
        RaycastHit raycastHit_;
        Vector3 d;

        for (float angolo = -45; angolo < 45; angolo += 10)
        {
            d = Quaternion.AngleAxis(angolo, Vector3.up) * direction;
            Physics.Raycast(transform.position, d, out raycastHit_, 60);

            if (GB.CompareORTags(raycastHit_.collider.gameObject, "CPU", "Player"))
                return true;
        }

        return false;
    }

    private void CPU_AI_Find_UseWeapons()
    {
        if (canUseProjectile())
        {
            if (FindEnemyDirection(-transform.forward))
                Projectile(rearSpawnpoint);
            else if (FindEnemyDirection(transform.forward))
                Projectile(frontSpawnpoint);
        }
        else if (canUseSpecial())
        {
            Special();
        }
        else if (canUseCounter())
        {
            var counterBehaviour = counter.GetComponent<CounterBehaviour>();

            foreach (var cars in AllCars)
                if (cars != gameObject)
                    if (Vector3.Distance(transform.position, cars.transform.position) < counterBehaviour.diametroDiAzione)
                    {
                        Counter();
                        break;
                    }
        }
    }

    private void CPU_AI_Find_Obstacles(bool wrong)
    {
        if (wrong)
        {
            if (currentObstacle != null && excludeObstacle != currentObstacle)
                excludeObstacles.Push(currentObstacle);

            if (currentObstacle != null)
                currentObstacleOtherCPU.Remove(currentObstacle);

            currentObstacle = null;
            lookAtDest = lookAtDestOriginal;
        }
        else
        {
            if (currentObstacle && excludeObstacle != currentObstacle && Vector3.Distance(transform.position, currentObstacle.transform.position) < obstacleDistance)
            {
                lookAtDest = currentObstacle.transform.position;
            }
            else
            {
                currentObstacle = selectaCandidateObstacleToFollow();

                if (currentObstacle)
                {
                    currentObstacleOtherCPU.Add(currentObstacle);
                    lookAtDest = currentObstacle.transform.position;
                }
                else
                {
                    lookAtDest = lookAtDestOriginal;
                }
            }
        }
    }

    private GameObject selectaCandidateObstacleToFollow()
    {
        GameObject nearestObstacle = null;
        var nearDistandce = float.MaxValue;

        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name.Equals("Obstacles"))
                foreach (var obstacle in GB.FindGameObjectsInChildWithTag(root.transform, "Obstacles"))
                {
                    var dist = Vector3.Distance(transform.position, obstacle.transform.position);

                    if (dist < obstacleDistance)
                        if (excludeObstacle != obstacle)
                            if (!currentObstacleOtherCPU.Contains(obstacle))
                            {
                                var distance_MeToNextSpline = Vector3.Distance(transform.position, curSplinePos);
                                var distance_ObstacleToNextSpline = Vector3.Distance(obstacle.transform.position, curSplinePos);

                                if (distance_MeToNextSpline > distance_ObstacleToNextSpline)
                                    if (dist < nearDistandce)
                                    {
                                        nearDistandce = dist;
                                        nearestObstacle = obstacle;
                                    }
                            }
                }

        return nearestObstacle;
    }

    internal void SetObstacleDestroyed(GameObject gameObject)
    {
        excludeObstacles.Push(gameObject);
        currentObstacleOtherCPU.Remove(currentObstacle);
        currentObstacle = null;
    }

    internal void fieldOfViewCollision(FieldOfViewCollider.eDirection direction, Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {

        }, "Player", "CPU");
    }

}