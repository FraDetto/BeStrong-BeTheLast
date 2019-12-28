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
    [SerializeField]
    protected Camera camera_; //camera fa parte di GameObject.camera

    public CinemachineImpulseSource vCam;

    //PostProcessVolume postVolume;
    internal PostProcessProfile postProfile;

    RaycastHit hitNear, giovane;
    //RaycastHit hitOn, hitNear;

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


    [Header("AI")]
    public eKCType KCType = eKCType.Human;

    [Range(0, 8)]
    public byte playerNumber = 1;

    public LayerMask wallMask;

    public SplineObject CurrentSplineObject;

    [Header("Parameters")]
    [Range(1, 6)]
    public float TempestivityOfDriftGearChange = 4;

    public float base_acceleration = 80f;
    public float max_acceleration_change = 0.5f;
    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 20f;
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

    protected const byte splineDistance = 5;
    protected const byte obstacleDistance = 30;

    protected Vector3 lookAtDest, lookAtDestOriginal, curSplinePos, prevSplinePos;
    private bool isSquished = false, limitSpeed = false, hardRotate = true;
    private float limitSpeedValue;

    protected float lastSplineDistance, prevSplineDistance;

    private string[] tubes = { "Tube001", "Tube002" };

    private Vector3 vettoreCorrezioneSfera = new Vector3(0, 0.4f, 0);

    internal float driftHeatingValue;

    internal bool driftCooldown;

    internal bool iAmAnnoyed;

    internal bool settingOnTrack, rotateToSpline = false;

    internal float annoyingAmount = 1f;

    private AudioSource boostAudioSource;

    internal float gravityMultiplier = 1f;

    protected bool wrong;


    protected void Start_()
    {
        Paused = true;

        if (camera_ == null)
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (root.tag.Equals("MainCamera"))
                    camera_ = root.GetComponent<Camera>();

        var audioSources = GetComponents<AudioSource>();
        foreach (var a in audioSources)
            if (!a.loop)
            {
                boostAudioSource = a;
                break;
            }

        //var postVolume = Camera.main.GetComponent<PostProcessVolume>();
        var postVolume = camera_.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

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
    }

    protected void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        if (Paused)
            return;

        if (settingOnTrack)
        {
            var hittingRight = Physics.Raycast(kartNormal.transform.position, kartNormal.transform.TransformDirection(new Vector3(0.4f, 0, 0.6f)), out giovane, 2f, wallMask);
            var hittingLeft = Physics.Raycast(kartNormal.transform.position, kartNormal.transform.TransformDirection(new Vector3(-0.4f, 0, 0.6f)), out giovane, 2f, wallMask);

            if (hittingLeft && hittingRight)
            {
                xAxis = 0;
                var dir = lookAtDestOriginal - kartModel.transform.position;
                var rot = Quaternion.Slerp(kartModel.transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 5f);

                var eul = rot.eulerAngles;
                eul.x = 0;
                eul.z = 0;

                transform.eulerAngles = eul;
            }
            else if (hittingLeft && !hittingRight)
            {
                xAxis = 1f;
                Accelerate(2f);
            }
            else if (!hittingLeft && hittingRight)
            {
                xAxis = -1f;
                Accelerate(2f);
            }
            else if (!hittingLeft && !hittingRight)
            {
                Accelerate(2f);
                sphere.AddForce(kartNormal.transform.forward * 300f, ForceMode.Impulse);
                settingOnTrack = false;
            }
        }

        //Follow Collider
        transform.position = sphere.transform.position - vettoreCorrezioneSfera;

        //Accelerate       
        speed = acceleration; // auto-acceleration

        if (driftDisabled)
        {
            jumpBDown = false;
            jumpBUp = false;
            clearDrift();
        }

        //Steer
        if (xAxis != 0)
        {
            var dir = xAxis > 0 ? 1 : -1;
            var amount = Mathf.Abs(xAxis) * annoyingAmount;

            Steer(dir, amount);
        }

        //Drift
        if (CanDrift() && jumpBDown && /*!drifting &&*/ xAxis != 0)
        {
            drifting = true;
            driftDirection = xAxis > 0 ? 1 : -1;

            foreach (var p in primaryParticles)
            {
                p.startColor = Color.clear;
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
            //Debug.Log(driftPower);

            ColorDrift();

            if (driftHeatingValue > 1f)
            {
                driftHeatingValue = 1f;
                driftCooldown = true;
                heatingSpeed *= driftPenalty;
                clearDrift();
            }
            else
                driftHeatingValue += heatingSpeed * Time.deltaTime;
        }
        else
        {
            if (driftHeatingValue < 0f)
            {
                driftHeatingValue = 0f;
                if (driftCooldown)
                {
                    driftCooldown = false;
                    heatingSpeed /= driftPenalty;
                }
            }
            else if (!iAmAnnoyed)
                driftHeatingValue -= heatingSpeed / 2f * Time.deltaTime;
        }

        if (jumpBUp && drifting)
            Boost();

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 5f);
        speed = 0f;

        if (limitSpeed && currentSpeed > limitSpeedValue)
            currentSpeed = limitSpeedValue;

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
        //frontWheels.localEulerAngles = new Vector3(0, (xAxis * 15), frontWheels.localEulerAngles.z);
        //frontWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        frontRightWheel.localEulerAngles = new Vector3(0, xAxis * 15, frontRightWheel.localEulerAngles.z);
        frontLeftWheel.localEulerAngles = new Vector3(0, xAxis * 15, frontLeftWheel.localEulerAngles.z);

        frontRightWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        frontLeftWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);

        //backWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        backRightWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        backLeftWheel.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);

        //c) Steering Wheel
        steeringWheel.localEulerAngles = new Vector3(-25, 90, xAxis * 45);
    }

    protected void FixedUpdate_()
    {
        //Forward Acceleration        
        if (drifting)
            sphere.AddForce(kartNormal.forward * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity * gravityMultiplier, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        var theRay = transform.position + (transform.up * 0.1f);
        Physics.Raycast(theRay, Vector3.down, out hitNear, 2f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    protected void clearDrift()
    {
        driftDirection = 0;
        driftPower = 0;
        driftMode = 0;
        first = false;
        second = false;
        third = false;
        drifting = false;

        foreach (var p in primaryParticles)
        {
            p.startColor = Color.clear;
            p.Stop();
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
    }

    void Boost()
    {
        boostAudioSource.Play(0);

        if (driftMode > 0)
            switch (KCType)
            {
                case eKCType.Human:
                    //DOVirtual.Float(currentSpeed * 3, currentSpeed, .3f * driftMode, Speed); // per accelerare
                    DOVirtual.Float(currentSpeed * 0.65f, currentSpeed, 0.3f * driftMode, Speed); // per rallentare
                    DOVirtual.Float(0, 1, 0.5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, 0.5f, ChromaticAmount));
                    break;
            }

        clearDrift();
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

    public float getCurrentSplineDistance() =>
        Vector3.Distance(CurrentSplineObject.transform.position, transform.position);

    void Speed(float x) =>
        currentSpeed = x;

    void ChromaticAmount(float x) =>
        postProfile.GetSetting<ChromaticAberration>().intensity.value = x;

    public void Accelerate(float amount)
    {
        sphere.velocity = transform.forward * acceleration / 2f;

        float bonusBias = GameState.Instance.getScoreBiasBonus(playerName);

        if (amount > 1)
            amount = amount - (Mathf.Max(amount - 1.1f, 0)) * bonusBias;
        else
            amount = amount - (Mathf.Max(amount - 0.1f, 0)) * bonusBias;

        currentSpeed *= amount;

        float speedCap = (enableSpeedRubberbanding) ? 200 - 60 * bonusBias : 200;

        if (currentSpeed > speedCap)
            currentSpeed = speedCap;

        if (amount > 1)
            PlayTurboEffect();
    }

    public void LimitSpeed(float speedLimit, int duration)
    {
        if (!limitSpeed)
        {
            LimitSpeed(speedLimit);
            StartCoroutine(RestoreSpeedLimit(duration));
        }
    }

    public void LimitSpeed(float speedLimit)
    {
        if (!limitSpeed)
        {
            limitSpeedValue = speedLimit;
            limitSpeed = true;
        }
    }

    public void RestoreSpeedLimit() =>
        limitSpeed = false;

    public void EnableHardRotate(bool enable_) =>
        hardRotate = enable_;

    public void AddForce(float force, ForceMode forceMode, Vector3 direction)
    {
        getWrongWayImmunity(2f);

        direction.y = 0; // giusto?
        sphere.AddForce(direction * force, forceMode);
    }

    public void getWrongWayImmunity(float duration)
    {
        wrongWayImmunity = true;
        StartCoroutine(disableWrongWayImmunity(duration));
    }

    IEnumerator disableWrongWayImmunity(float countdown)
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
        foreach (var p in tubeTurboParticles)
            foreach (var pp in p)
                pp.Play();
    }

    internal void setDestination(float xRndError, float zRndError) =>
        setDestination(xRndError, zRndError, false);

    internal void setDestination(float xRndError, float zRndError, bool firstTime) =>
        setDestination(xRndError, zRndError, first, CurrentSplineObject.nextFirstSpline);

    internal void setDestination(float xRndError, float zRndError, bool firstTime, SplineObject nextSpline)
    {
        if (nextSpline)
        {
            lastSplineDistance = 0;
            prevSplineDistance = 0;

            prevSplinePos = CurrentSplineObject.transform.position;

            if (!firstTime)
                CurrentSplineObject = nextSpline;

            curSplinePos = CurrentSplineObject.transform.position;

            var p = CurrentSplineObject.transform.position;

            lookAtDest = new Vector3(p.x + xRndError, p.y, p.z + zRndError);
            lookAtDestOriginal = lookAtDest;

            if (rotateToSpline)
                rotateToSpline = false;
        }
    }

    internal void SetOnTrack()
    {
        if (wrong)
        {
            var dir = lookAtDestOriginal - transform.position;
            var rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 1f);

            var eul = rot.eulerAngles;
            eul.x = 0;
            eul.z = 0;

            transform.eulerAngles = eul;
            dir.y = 0; // giusto?

            sphere.AddForce(dir.normalized * 500f, ForceMode.Impulse);
            Accelerate(3f);
        }
        else
            settingOnTrack = true;
    }

    internal void activNewCamera(int indexCamToActiv, int indexCamToDis)
    {
        camera_.transform.GetChild(indexCamToActiv).gameObject.SetActive(true);
        camera_.transform.GetChild(indexCamToDis).gameObject.SetActive(false);
    }

    protected bool CanDrift() =>
        !driftCooldown;

    internal string playerName =>
        transform.parent.gameObject.name;

}