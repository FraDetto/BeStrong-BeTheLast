/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public abstract class aKartController : aCollisionManager
{

    public enum eKCType
    {
        Human, CPU
    }


    [Header("Camera")]
    public Camera camera_; //camera fa parte di GameObject.camera

    public CinemachineImpulseSource vCam;

    internal PostProcessProfile postProfile;

    RaycastHit hitNear;

    List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();
    List<ParticleSystem[]> tubeTurboParticles = new List<ParticleSystem[]>();

    protected float speed;
    internal bool wrongWayImmunity = false;

    float rotate, currentRotate;
    protected float driftPower;
    internal int driftDirection, driftMode;
    internal bool drifting, first, second, third;
    protected bool driftDisabled;
    Color currentDriftColor;

    [Header("Physics")]
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;
    public float currentSpeed;

    public float distanzaYDallaSfera = 0.4f;
    private Vector3 vettoreCorrezioneSfera;

    [Header("AI")]
    public eKCType KCType = eKCType.Human;

    [Range(0, 8)]
    public byte playerNumber = 1;

    protected string JoystickName =>
        "P" + playerNumber;

    internal string PlayerName =>
        transform.parent.gameObject.name;

    public LayerMask wallMask;

    public SplineObject CurrentSplineObject;

    [Header("Parameters")]
    [Range(1, 6)]
    public float TempestivityOfDriftGearChange = 4;

    private const float real_gravity = 25;
    private float accelleration_gravity = 25;

    [Range(20, 200)]
    public byte MaxKMH = 200;

    public float base_acceleration = 80f;
    public float max_acceleration_change = 0.5f;
    public float acceleration = 30f;
    public float steering = 80f;
    public float maxAngle = 30f;
    public float wallsMultiplier = 25f;
    public float accelerationMultiplier = 1f;
    public float kartWallDistance = 5f; 
    public float heatingSpeed = 0.25f;
    public float driftPenalty = 1f;
    public bool enableSpeedRubberbanding;
    public LayerMask layerMask;

    [Header("Model Parts")]
    public Transform frontRightWheel;
    public Transform frontLeftWheel;
    public Transform backRightWheel;
    public Transform backLeftWheel;
    public Transform steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;

    protected const byte obstacleDistance = 30;

    protected Vector3 lookAtDest, lookAtDestOriginal, curSplinePos, prevSplinePos;
    private bool isSquished = false, limitSpeed = false, hardRotate = true;
    private float limitSpeedValue;

    protected float currSplineDistance_t0, prevSplineDistance_t0;

    private readonly string[] tubes = { "Tube001", "Tube002" };

    private AudioSource driftAudioSource;
    private AudioSource boostAudioSource;

    internal bool touchingGround = true;

    private GameState.GameStateInternal gameState = GameState.Instance;

    private int numberOfPlayers;

    internal bool counterImmunity;

    public bool touchingWall = false;

    protected void Start_()
    {
        if(vCam)
            vCam.gameObject.SetActive(true);

        Paused = true;

        vettoreCorrezioneSfera = new Vector3(0, distanzaYDallaSfera, 0);

        if (camera_ == null)
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (root.tag.Equals("MainCamera"))
                    camera_ = root.GetComponent<Camera>();

        var audioSources = GetComponents<AudioSource>();
        driftAudioSource = audioSources[1];
        boostAudioSource = audioSources[2];
        

        for (var i = 0; i < wheelParticles.GetChild(0).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());

        for (var i = 0; i < wheelParticles.GetChild(1).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());

        foreach (var p in flashParticles.GetComponentsInChildren<ParticleSystem>())
            secondaryParticles.Add(p);

        foreach (var tube in tubes)
            tubeTurboParticles.Add(kartModel.GetChild(0).Find(tube).GetComponentsInChildren<ParticleSystem>());

        if (!CurrentSplineObject)
            throw new Exception("Non hai settato CurrentSplineObject!");

        numberOfPlayers = gameState.positions.Count - 1; //Formula goes from 0 to 7 instead of 1 to 8
    }

    protected void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        if (Paused)
            return;

        var hittingRight = Physics.Raycast(transform.position, transform.right, kartWallDistance, wallMask);
        var hittingLeft = Physics.Raycast(transform.position, -transform.right, kartWallDistance, wallMask);

        if(hittingLeft || hittingRight)
        {
            touchingWall = true;
            if((hittingLeft && xAxis < 0) || (hittingRight && xAxis > 0))
                xAxis = 0;
        }
        else
            touchingWall = false;

        //Follow Collider
        transform.position = sphere.transform.position - vettoreCorrezioneSfera;

        //Accelerate       
        speed = acceleration; // auto-acceleration

        if (driftDisabled)
        {
            jumpBDown = false;
            jumpBUp = false;
            ClearDrift();
        }

        //Steer
        if (xAxis != 0)
        {
            var dir = xAxis > 0 ? 1 : -1;
            var amount = Mathf.Abs(xAxis);

            Steer(dir, amount);
        }

        //Drift
        if (jumpBDown && xAxis != 0)
        {
            if(!driftAudioSource.isPlaying)
                driftAudioSource.Play();
            drifting = true;
            driftDirection = xAxis > 0 ? 1 : -1;

            foreach (var p in primaryParticles)
            {
                var pMain = p.main;
                pMain.startColor = Color.clear;
                p.Play();
            }

            kartModel.parent.DOComplete();
            kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
        }

        if (drifting)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(xAxis, -1, 1, 0, 2) : ExtensionMethods.Remap(xAxis, -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(xAxis, -1, 1, .2f, 1) : ExtensionMethods.Remap(xAxis, -1, 1, 1, .2f);

            Steer(driftDirection, control);
            driftPower += powerControl * TempestivityOfDriftGearChange;

            ColorDrift();
        }

        if (jumpBUp && drifting)
            Boost();

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 5f);
        speed = 0f;

        currentRotate = Mathf.Lerp(currentRotate, rotate / (hardRotate ? 2 : 1), Time.deltaTime * 4f);
        rotate = 0f;

        //Animations    

        //a) Kart
        if (drifting)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(xAxis, -1, 1, .5f, 2) : ExtensionMethods.Remap(xAxis, -1, 1, 2, .5f);

            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, control * 15 * driftDirection, .2f), 0);
        }
        else
        {
            kartModel.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.localEulerAngles.y, xAxis * 15, .2f), 0);
        }

        //b) Wheels
        frontRightWheel.localEulerAngles = new Vector3(0, xAxis * 25, frontRightWheel.localEulerAngles.z);
        frontLeftWheel.localEulerAngles = new Vector3(0, xAxis * 25, frontLeftWheel.localEulerAngles.z);

        frontRightWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        frontLeftWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);

        backRightWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        backLeftWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);

        //c) Steering Wheel
        steeringWheel.localEulerAngles = new Vector3(-25, 90, xAxis * 45);
    }

    protected void FixedUpdate_()
    {
        if(Paused)
            return;

        //Wall Repulsion
        if(touchingWall)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, CurrentSplineObject.transform.rotation, Time.deltaTime * 10f);
            sphere.AddForce(transform.forward * currentSpeed * wallsMultiplier, ForceMode.Acceleration);
            PlayTurboEffect();
        }
        else
            sphere.AddForce(transform.forward * currentSpeed * accelerationMultiplier, ForceMode.Acceleration);


        //Gravity
        if (touchingGround) // premesso che questo booleano sia vero        
            accelleration_gravity = real_gravity;
        else
            accelleration_gravity += 5; // da tunare        

        sphere.AddForce(Vector3.down * accelleration_gravity, ForceMode.Acceleration);

        //Steering
        var carSplineAngle = Vector3.SignedAngle(CurrentSplineObject.transform.forward, transform.forward, Vector3.up); 
        if(carSplineAngle + currentRotate < maxAngle && carSplineAngle + currentRotate > -maxAngle)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + currentRotate, 0)), Time.deltaTime * 5f);

        var theRay = transform.position + (transform.up * 0.1f);
        Physics.Raycast(theRay, Vector3.down, out hitNear, 2f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    protected bool WrongWayFromSpline
    {
        get
        {
            if (!wrongWayImmunity)
            {
                var currSplineDistance_t1 = Vector3.Distance(transform.position, curSplinePos);
                var prevSplineDistance_t1 = Vector3.Distance(transform.position, prevSplinePos);

                var wrong =
                    currSplineDistance_t0 > 0 &&
                    prevSplineDistance_t0 > 0 &&
                    currSplineDistance_t1 > currSplineDistance_t0 &&
                    prevSplineDistance_t1 < prevSplineDistance_t0;

                currSplineDistance_t0 = currSplineDistance_t1;
                prevSplineDistance_t0 = prevSplineDistance_t1;

                return wrong;
            }

            return false;
        }
    }

    protected void ClearDrift()
    {
        driftAudioSource.Stop();
        driftDirection = 0;
        driftPower = 0;
        driftMode = 0;
        first = false;
        second = false;
        third = false;
        drifting = false;

        foreach (var p in primaryParticles)
        {
            var pMain = p.main;
            pMain.startColor = Color.clear;
            p.Stop();
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
    }

    void Boost()
    {
        if (driftMode == 3)
        {
            //DOVirtual.Float(currentSpeed * 3, currentSpeed, .3f * driftMode, Speed); // per accelerare
            //DOVirtual.Float(currentSpeed * 0.65f, currentSpeed, driftMode, Speed); // per rallentare
            //DOVirtual.Float(0, 1, 0.5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, 0.5f, ChromaticAmount));
            Accelerate(0.875f, 2f);
        }         

        ClearDrift();
    }

    void Steer(int direction, float amount) =>
        rotate = steering * direction * amount;

    void ColorDrift()
    {
        if (!first)
            currentDriftColor = Color.clear;

        if (driftPower > 50 && driftPower < 100 - 1 && !first)
        {
            first = true;
            currentDriftColor = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(currentDriftColor);
        }

        if (driftPower > 100 && driftPower < 150 - 1 && !second)
        {
            second = true;
            currentDriftColor = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(currentDriftColor);
        }

        if (driftPower > 150 && !third)
        {
            third = true;
            currentDriftColor = turboColors[2];
            driftMode = 3;
            
            PlayFlashParticle(currentDriftColor);
            
        }

        foreach (var p in primaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = currentDriftColor;
        }

        foreach (var p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = currentDriftColor;
        }
    }

    void PlayFlashParticle(Color c)
    {
        switch (KCType)
        {
            case eKCType.Human:
                vCam.GenerateImpulse();
                break;
        }

        foreach (var p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    public float GetCurrentSplineDistance() =>
        Vector3.Distance(CurrentSplineObject.transform.position, transform.position);

    void Speed(float x) =>
        currentSpeed = x;

    void ChromaticAmount(float x) =>
        postProfile.GetSetting<ChromaticAberration>().intensity.value = x;

    public void Accelerate(float amount, float time)
    {
        accelerationMultiplier = amount;
        StartCoroutine(RestoreSpeed(time));

        if(amount > 1)
            PlayTurboEffect();
    }

    public void RestoreSpeedLimit() =>
        limitSpeed = false;

    public void EnableHardRotate(bool enable_) =>
        hardRotate = enable_;

    public void AddForce(float force, ForceMode forceMode, Vector3 direction, bool GetTheWrongWayImmunity)
    {
        if (GetTheWrongWayImmunity)
            GetWrongWayImmunity(2f);

        direction.y = 0; // giusto?
        sphere.AddForce(direction * force, forceMode);
    }

    public void GetWrongWayImmunity(float duration)
    {
        wrongWayImmunity = true;
        StartCoroutine(DisableWrongWayImmunity(duration));
    }

    IEnumerator DisableWrongWayImmunity(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        wrongWayImmunity = false;
    }

    public void BeSquished(int duration)
    {
        if (!isSquished)
        {
            isSquished = true;
            transform.parent.transform.localScale += new Vector3(0, -0.5f, 0);
            transform.parent.GetComponentInChildren<SphereCollider>().radius = 0.6f;
            StartCoroutine(RestoreSquishedShape(duration));
        }
    }

    IEnumerator RestoreSquishedShape(int duration)
    {
        yield return new WaitForSeconds(duration);

        transform.parent.transform.localScale += new Vector3(0, +0.5f, 0);
        transform.parent.GetComponentInChildren<SphereCollider>().radius = 0.85f;

        isSquished = false;
    }

    IEnumerator RestoreSpeedLimit(int duration)
    {
        yield return new WaitForSeconds(duration);
        limitSpeed = false;
    }

    public void PlayTurboEffect()
    {
        if(!boostAudioSource.isPlaying)
            boostAudioSource.Play(0);

        var play = true;

        foreach(var p in tubeTurboParticles)
            foreach(var pp in p)
                if(pp.isPlaying)
                    play = false;

        if(play)
            foreach(var p in tubeTurboParticles)
                foreach(var pp in p)
                    pp.Play();  
    }

    internal void SetDestination(float xRndError, float zRndError) =>
        SetDestination(xRndError, zRndError, false);

    internal void SetDestination(float xRndError, float zRndError, bool firstTime) =>
        SetDestination(xRndError, zRndError, first, CurrentSplineObject.NextFirstSpline(this));

    internal void SetDestination(float xRndError, float zRndError, bool firstTime, SplineObject nextSpline)
    {
        if (nextSpline)
        {
            currSplineDistance_t0 = 0;
            prevSplineDistance_t0 = 0;

            prevSplinePos = CurrentSplineObject.transform.position;

            if (firstTime)
            {
                var c = CurrentSplineObject;

                while (CurrentSplineObject.prev_Spline == null)
                    c = c.NextFirstSpline(this);

                prevSplinePos = CurrentSplineObject.prev_Spline.transform.position;
            }
            else
            {
                CurrentSplineObject = nextSpline;
            }

            curSplinePos = CurrentSplineObject.transform.position;

            var p = CurrentSplineObject.transform.position;

            lookAtDest = new Vector3(p.x + xRndError, p.y, p.z + zRndError);
            lookAtDestOriginal = lookAtDest;
        }
    }

    internal void ActivNewCamera(int indexCamToActiv, int indexCamToDis)
    {
        camera_.transform.GetChild(indexCamToActiv).gameObject.SetActive(true);
        camera_.transform.GetChild(indexCamToDis).gameObject.SetActive(false);
    }

    /* To be used when you want effects changing depending on your rank:
     * Amount is the base amount of the effect (e.g. 0.5 for annoying amount)
     * AmountDiff is the difference between amount and the maxAmount (e.g. maxAmount is 1 for annoying, so amountDiff will be 0.5) */
    internal float EffectDistributionFormula(float amount, float amountDiff) =>
        amount + (amountDiff * ((numberOfPlayers - (gameState.getCurrentRanking(PlayerName) - 1)) / (float)numberOfPlayers));

    IEnumerator RestoreSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        accelerationMultiplier = 1f;
    }
}