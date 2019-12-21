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

    internal bool touchingGround = true;

    // ============== HUMAN ==============
    public bool UsaWrongWay;

    private float wrongWayTimer = 2, wrongWayMaxTimer = 1;

    // ============== HUMAN ==============

    // =============== CPU ===============
    private const byte errorDelta = 8;

    private GameObject[] CPUCars;
    private GameObject[] PlayersCars;
    private List<KartController> AllCars = new List<KartController>();

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

    [Range(0, 1)]
    public float ProbabilitàDiSparare = 0.5f;

    private bool aspettandoDiSparare;


    private void Start()
    {
        CPUCars = GameObject.FindGameObjectsWithTag("CPU");
        PlayersCars = GameObject.FindGameObjectsWithTag("Player");

        var AllCarsTemp = new List<GameObject>(CPUCars.Length + PlayersCars.Length);
        AllCarsTemp.AddRange(CPUCars);
        AllCarsTemp.AddRange(PlayersCars);

        foreach (var c in AllCarsTemp)
        {
            var kc = c.GetComponent<KartController>();

            if (kc)
                AllCars.Add(kc);
        }

        switch (KCType)
        {
            case eKCType.Human:
                GameState.Instance.kartTypes.Add(transform.parent.name, "Player");
                setDestination(0, 0, true);
                break;

            case eKCType.CPU:
                GameState.Instance.kartTypes.Add(transform.parent.name, "CPU");
                setDestinationWithError();
                break;
        }

        Start_();
        started = true;
    }

    private void Update()
    {
        if (Paused)
            return;

        wrong = wrongWay;

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
                    driftDisabled = !touchingGround;

                    Update_(
                        touchingGround ? GB.GetAxis(input + "Horizontal") : 0,
                        GB.GetButton(input + "Drift"),
                        GB.GetButtonUp(input + "Drift")
                    );
                }

                if (UsaWrongWay && wrong)
                {
                    SetOnTrack();
                    wrongWayTimer = 0;
                    driftDisabled = true;
                }

                if (GB.GetButtonDown(input + "Counter") || GB.GetAxis(input + "Counter") != 0)
                    Counter();

                if (GB.GetButtonDown(input + "Projectile"))
                {
                    var direzione = Input.GetAxis(input + "Vertical") < 0 ? rearSpawnpoint : frontSpawnpoint;

                    if (myAbility.myAttractor == null)
                        Projectile(direzione);
                    else
                        Attractor(direzione);
                }

                if (GB.GetButtonDown(input + "Special") || GB.GetAxis(input + "Special") != 0)
                    Special();

                if (GB.GetButtonDown(input + "MenuA") && rankPanel)
                    rankPanel.SetActive(!rankPanel.activeSelf);

                break;

            case eKCType.CPU:
                if (Vector3.Distance(transform.position, lookAtDestOriginal) < splineDistance)
                    setDestinationWithError();

                if (CurrentSplineObject != null && CurrentSplineObject.CanBeClosedByThisWall != null && CurrentSplineObject.CanBeClosedByThisWall.closed)
                    setTheOtherFork();

                CPU_AI_Find_Obstacles(wrong);

                bool steerCond = !iAmBlinded || Mathf.CeilToInt(Time.time) % 3 == 0;

                CPU_AI_Find_UseWeapons();

                var drift_ = CurrentSplineObject.splineType == SplineObject.eSplineType.Drift;
                var jumpBUP = !bJumpReleased && drift_ && driftPower > 250;
                var jumpBDown = drift_ && !jumpBUP;
                var driftAxis = (drift_ ? 0.0001f : 0);

                if (steerCond)
                {
                    float angle = steerAngle(lookAtDest);
                    bool turn = Mathf.Abs(angle) > 10f;
                    bool left = angle < 0;

                    driftAxis = turn ? (left ? 1 : -1) : 0;

                    if (drift_ || Mathf.Abs(angle) > 45)
                    {
                        //Possibly add some conditions regarding driftHeatingValue here, to avoid unreasonable use of the drift function
                        jumpBDown = true;
                        jumpBUP = false;
                    }
                }

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

                if (transform.position == lastPosition)
                {
                    if (LastStuck > -1)
                        LastStuck = Time.time;

                    if (Time.time - LastStuck > 6)
                    {
                        var p = CurrentSplineObject.transform.position;
                        transform.position = new Vector3(p.x, p.y + 2, p.z);
                        LastStuck = -1;
                    }
                }

                lastPosition = transform.position;
                break;
        }

        Debug.DrawLine(transform.position, lookAtDestOriginal, Color.green);
        Debug.DrawLine(transform.position, lookAtDest, Color.yellow);
    }

    private float steerAngle(Vector3 dest)
    {
        dest.y = 0;

        var lookPos = dest - transform.position;
        lookPos.y = 0;

        var rotation = Quaternion.LookRotation(lookPos);

        return Mathf.DeltaAngle(rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
    }

    private void FixedUpdate() =>
        FixedUpdate_();

    private bool wrongWay
    {
        get
        {
            if (!wrongWayImmunity)
            {
                var currentSplineDistance1 = Vector3.Distance(transform.position, curSplinePos);
                var currentSplineDistance0 = Vector3.Distance(transform.position, prevSplinePos);

                var wrong =
                    lastSplineDistance > 0 &&
                    prevSplineDistance > 0 &&
                    currentSplineDistance1 > lastSplineDistance &&
                    (currentSplineDistance0 < prevSplineDistance || GameState.Instance.positions[playerName] == 0);

                lastSplineDistance = currentSplineDistance1;
                prevSplineDistance = currentSplineDistance0;

                return wrong;
            }

            return false;
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
        // TODO NON FUNZIONA
        RaycastHit raycastHit_;
        Vector3 d;

        for (float angolo = -45; angolo < 45; angolo += 10)
        {
            d = Quaternion.AngleAxis(angolo, Vector3.up) * direction;
            Physics.Raycast(transform.position, d, out raycastHit_, 60);

            if (raycastHit_.collider)
                if (raycastHit_.collider.gameObject)
                    if (GB.CompareORTags(raycastHit_.collider.gameObject, "CPU", "Player"))
                        return true;
        }

        return false;
    }

    private void CPU_AI_Find_UseWeapons()
    {
        foreach (var car in AllCars)
            if (!Equals(car))
            {
                if (canUseSpecial())
                {
                    Special();
                    break;
                }
                else if (canUseCounter())
                {
                    var counterBehaviour = counter.GetComponent<CounterBehaviour>();

                    if (Vector3.Distance(transform.position, car.transform.position) < counterBehaviour.raggioDiAzione)
                    {
                        Counter();
                        break;
                    }
                }
            }
    }

    internal void fieldOfViewCollision(FieldOfViewCollider.eDirection direction, Collider collider) =>
        onCollisionWithPlayer_or_CPU(collider, (kartController) =>
        {
            if (!Equals(kartController))
            {

            }
        });

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

}