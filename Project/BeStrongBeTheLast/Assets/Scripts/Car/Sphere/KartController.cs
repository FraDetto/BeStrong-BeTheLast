/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class KartController : aBSBTLKart
{

    internal bool touchingGround = true;

    // ============== HUMAN ==============
    public bool UsaWrongWay;

    private float wrongWayTimer = 2, wrongWayMaxTimer = 1;

    internal GameObject rankPanel;

    // ============== HUMAN ==============

    // =============== CPU ===============
    private const byte errorDelta = 8;

    private ObstacleDetector frontSpawnpoint_obstacleDetector;

    private GameObject[] CPUCars;
    private GameObject[] PlayersCars;
    private List<KartController> AllCars = new List<KartController>();

    private bool bJumpReleased;

    private static HashSet<GameObject> currentObstacleOtherCPU = new HashSet<GameObject>();
    private Stack<GameObject> excludeObstacles = new Stack<GameObject>(5);
    private GameObject currentObstacle;
    // =============== CPU ===============    

    private GameObject excludeObstacle =>
        excludeObstacles.Count > 0 ? excludeObstacles.Peek() : null;

    internal bool started;

    [Range(0, 1)]
    public float ProbabilitàDiSparare = 0.5f;


    private void Start()
    {
        CPUCars = GameObject.FindGameObjectsWithTag("CPU");
        PlayersCars = GameObject.FindGameObjectsWithTag("Player");
        frontSpawnpoint_obstacleDetector = frontSpawnpoint.GetComponent<ObstacleDetector>();

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

    private void FixedUpdate() =>
        FixedUpdate_();

    private void Update()
    {
        if (Paused)
            return;

        switch (KCType)
        {
            case eKCType.Human:
                var wrong = wrongWayFromSpline;

                if (wrong || (!wrong && wrongWayTimer < wrongWayMaxTimer))
                {
                    Update_(0, false, false);
                    wrongWayTimer += Time.deltaTime;
                }
                else
                {
                    driftDisabled = !touchingGround;

                    Update_(
                        touchingGround ? GB.GetAxis(JoystickName + "Horizontal") : 0,
                        GB.GetButton(JoystickName + "Drift"),
                        GB.GetButtonUp(JoystickName + "Drift")
                    );
                }

                if (UsaWrongWay && wrong)
                {
                    SetOnTrack();
                    wrongWayTimer = 0;
                    driftDisabled = true;
                }

                if (GB.GetButtonDown(JoystickName + "Counter") || GB.GetAxis(JoystickName + "Counter") != 0)
                    Counter();

                if (GB.GetButtonDown(JoystickName + "Projectile"))
                {
                    var direzione = Input.GetAxis(JoystickName + "Vertical") < 0 ? rearSpawnpoint : frontSpawnpoint;

                    if (myAbility.myAttractor == null)
                        Projectile(direzione);
                    else
                        Attractor(direzione);
                }

                if (GB.GetButtonDown(JoystickName + "Special") || GB.GetAxis(JoystickName + "Special") != 0)
                    Special();

                if (GB.GetButtonDown(JoystickName + "MenuA") && rankPanel)
                    rankPanel.SetActive(!rankPanel.activeSelf);

                break;

            case eKCType.CPU:
                if (CurrentSplineObject != null && CurrentSplineObject.isThisSplineClosed())
                    setDestinationWithError(CurrentSplineObject.nextAlternativeSpline);

                CPU_AI_Find_Obstacles(wrongWayFromObstacle);

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

                break;
        }

        // Debug.DrawLine(transform.position, lookAtDestOriginal, Color.green);
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

    float currentObstacleDistance, prevObstacleDistance;

    private bool wrongWayFromObstacle
    {
        get
        {
            if (currentObstacle)
            {
                currentObstacleDistance = Vector3.Distance(transform.position, currentObstacle.transform.position);

                var wrong =
                    currentObstacleDistance > 0 &&
                    prevObstacleDistance > 0 &&
                    currentObstacleDistance > prevObstacleDistance;

                prevObstacleDistance = currentObstacleDistance;

                return wrong;
            }

            return false;
        }
    }

    public float currentSplineDistance =>
        Vector3.Distance(transform.position, curSplinePos);

    internal void setDestinationWithError() =>
        setDestinationWithError(CurrentSplineObject.nextRandomSpline);

    internal void setDestinationWithError(SplineObject nextSpline) =>
           setDestination(GB.NormalizedRandom(-1f, 1f) * errorDelta, GB.NormalizedRandom(-1f, 1f) * errorDelta, false, nextSpline);

    internal void nextSpline() =>
        setDestination(0, 0);

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
                // si doveva usare ma non l'abbiamo usato ancora
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
        var detectedObstacles = frontSpawnpoint_obstacleDetector.detectedObstacles;

        if (detectedObstacles.Count > 0)
        {
            var ostacoloScelto = detectedObstacles.OrderBy(obstacle => Vector3.Distance(transform.position, obstacle.transform.position)).First();

            if (!Physics.Raycast(transform.position, transform.forward, Vector3.Distance(transform.position, ostacoloScelto.transform.position), wallMask))
                return ostacoloScelto;
        }

        return null;
    }

    internal void SetObstacleDestroyed(GameObject gameObject)
    {
        excludeObstacles.Push(gameObject);

        if (currentObstacle)
            currentObstacleOtherCPU.Remove(currentObstacle);

        currentObstacle = null;
    }

}