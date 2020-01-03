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

    // ============== HUMAN ==============
    public bool UsaWrongWay;
    private float wrongWayTimer = 2;
    private const float wrongWayMaxTimer = 1;

    internal GameObject rankPanel;
    // ============== HUMAN ==============


    // =============== CPU ===============
    private const byte errorDelta = 8;

    private ObstacleDetector frontSpawnpoint_obstacleDetector;

    private List<KartController> PlayersCars, CPUCars;
    private List<KartController> AllCars = new List<KartController>();

    private bool bJumpReleased;

    private static HashSet<GameObject> currentObstacleOtherCPU = new HashSet<GameObject>(); // common to ALL instance of KartController
    private FixedSizedQueue<GameObject> excludeObstacles = new FixedSizedQueue<GameObject>(5);
    private GameObject currentObstacle;
    // =============== CPU ===============    


    private GameObject ExcludeObstacle =>
        excludeObstacles.Count > 0 ? excludeObstacles.Peek() : null;

    private float currentObstacleDistance, prevObstacleDistance;
    internal bool started;

    //[Range(0, 1)] non è stato più usato
    //public float ProbabilitàDiSparare = 0.5f;


    private void Start()
    {
        frontSpawnpoint_obstacleDetector = frontSpawnpoint.GetComponent<ObstacleDetector>();

        CPUCars = GetKartControllers(GameObject.FindGameObjectsWithTag("CPU"));
        PlayersCars = GetKartControllers(GameObject.FindGameObjectsWithTag("Player"));

        AllCars.AddRange(CPUCars);
        AllCars.AddRange(PlayersCars);

        switch (KCType)
        {
            case eKCType.Human:
                GameState.Instance.kartTypes.Add(transform.parent.name, "Player");
                SetDestination(0, 0, true, CurrentSplineObject);
                break;

            case eKCType.CPU:
                GameState.Instance.kartTypes.Add(transform.parent.name, "CPU");
                SetDestinationWithError();
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
                var wrong = WrongWayFromSpline;

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
                    SetOnTrack(true);
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
                    SetDestinationWithError(CurrentSplineObject.nextAlternativeSpline);

                CPU_AI_Find_Obstacles(WrongWayFromSpline || WrongWayFromObstacle);

                bool steerCond = !iAmBlinded || Mathf.CeilToInt(Time.time) % 3 == 0;

                CPU_AI_UseWeapons();

                lookAtDest.y = 0;
                lookAtDestOriginal.y = 0;

                var drift_ = CurrentSplineObject.splineType == SplineObject.eSplineType.Drift;
                var jumpBUP = !bJumpReleased && drift_ && driftPower > 250;
                var jumpBDown = drift_ && !jumpBUP;
                var driftAxis = drift_ ? 0.0001f : 0;

                if (steerCond)
                {
                    float angle = SteerAngle(lookAtDest);
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
                    ClearDrift();

                Update_(driftAxis, jumpBDown, jumpBUP);

                if (jumpBUP)
                    bJumpReleased = true;

                break;
        }

        Debug.DrawLine(transform.position, lookAtDestOriginal, Color.green);
        Debug.DrawLine(transform.position, lookAtDest, Color.red);
    }

    private float SteerAngle(Vector3 dest)
    {
        dest.y = 0;

        var lookPos = dest - transform.position;
        lookPos.y = 0;

        var rotation = Quaternion.LookRotation(lookPos);

        return Mathf.DeltaAngle(rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
    }

    private bool WrongWayFromObstacle
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

    public float CurrentSplineDistance =>
        Vector3.Distance(transform.position, curSplinePos);

    internal void SetDestinationWithError() =>
        SetDestinationWithError(CurrentSplineObject.nextRandomSpline);

    internal void SetDestinationWithError(SplineObject nextSpline) =>
           SetDestination(GB.NormalizedRandom(-1f, 1f) * errorDelta, GB.NormalizedRandom(-1f, 1f) * errorDelta, false, nextSpline);

    internal void NextSpline() =>
        SetDestination(0, 0);

    private void CPU_AI_UseWeapons()
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

    internal void FieldOfViewCollision(FieldOfViewCollider.eDirection direction, Collider collider) =>
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
            if (currentObstacle != null && ExcludeObstacle != currentObstacle)
                excludeObstacles.Enqueue(currentObstacle);

            if (currentObstacle != null)
                currentObstacleOtherCPU.Remove(currentObstacle);

            currentObstacle = null;
            lookAtDest = lookAtDestOriginal;
        }
        else
        {
            if (currentObstacle && ExcludeObstacle != currentObstacle && Vector3.Distance(transform.position, currentObstacle.transform.position) < obstacleDistance)
            {
                lookAtDest = currentObstacle.transform.position;
            }
            else
            {
                currentObstacle = SelectaCandidateObstacleToFollow();

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

    private GameObject SelectaCandidateObstacleToFollow()
    {
        var detectedObstacles = frontSpawnpoint_obstacleDetector.detectedObstacles;

        if (detectedObstacles.Count > 0)
        {
            var ostacoliScelti = detectedObstacles.OrderBy(obstacle => Vector3.Distance(transform.position, obstacle.Value.transform.position));

            foreach (var ostacoloScelto in ostacoliScelti)
                if (ostacoloScelto.Value)
                    if (ExcludeObstacle != ostacoloScelto.Value)
                        if (!Physics.Raycast(transform.position, transform.forward, Vector3.Distance(transform.position, ostacoloScelto.Value.transform.position), wallMask))
                            return ostacoloScelto.Value;
        }

        return null;
    }

    internal void SetObstacleDestroyed(GameObject gameObject)
    {
        if (gameObject != null)
            excludeObstacles.Enqueue(gameObject);

        if (currentObstacle)
            currentObstacleOtherCPU.Remove(currentObstacle);

        currentObstacle = null;
    }

    private List<KartController> GetKartControllers(GameObject[] cars)
    {
        var R = new List<KartController>();

        foreach (var c in cars)
        {
            var kc = c.GetComponent<KartController>();

            if (kc)
                R.Add(kc);
        }

        return R;
    }

}