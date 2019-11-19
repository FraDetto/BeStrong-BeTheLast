/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public abstract class aKartController : MonoBehaviour
{

    public enum eKCType
    {
        Human, CPU
    }

    public eKCType KCType = eKCType.Human;


    //PostProcessVolume postVolume;
    PostProcessProfile postProfile;

    RaycastHit hitNear;
    //RaycastHit hitOn, hitNear;

    List<ParticleSystem> primaryParticles, secondaryParticles, tubeTurboParticles;

    private const short PosizionePavimento = 2;
    protected bool RibaltaDisabilitato, Ribalta;

    public Transform kartModel, kartNormal;
    public Rigidbody sphere;

    protected float speed;
    internal float currentSpeed;

    float rotate, currentRotate;
    protected float driftPower;
    int driftDirection, driftMode;
    bool drifting, first, second, third;
    protected bool driftDisabled;
    Color currentDriftColor;

    [Range(1, 6)]
    public float TempestivityOfDriftGearChange = 4;

    [Header("Parameters")]
    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    [Header("Model Parts")]
    //public Transform frontWheels, backWheels;    
    public Transform frontRightWheel, frontLeftWheel, backRightWheel, backLeftWheel, steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles, flashParticles;
    public Color[] turboColors;

    public GameObject CPUSpline;
    protected SplineObject[] CPUSplines;
    protected SplineObject CurrentSplineObject;
    protected sbyte CurrentSpline = -1;
    protected const byte splineDistance = 5;
    protected Vector3 lookAtDest, curSplinePos, prevSplinePos;
    private bool isSquished = false, limitSpeed = false, hardRotate = true;
    private float limitSpeedValue;

    private byte lastUsedFork;
    protected float lastSplineDistance, prevSplineDistance;

    private string[] tubes = { "Tube001", "Tube002" };


    protected void Start_()
    {
        CPUSplines = new SplineObject[CPUSpline.transform.childCount];

        byte x = 0;
        foreach (var el in CPUSpline.transform)
        {
            var t = el as Transform;
            CPUSplines[x] = t.gameObject.GetComponent<SplineObject>();
            x++;
        }

        var postVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        primaryParticles = new List<ParticleSystem>();
        secondaryParticles = new List<ParticleSystem>();
        tubeTurboParticles = new List<ParticleSystem>();

        for (var i = 0; i < wheelParticles.GetChild(0).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());

        for (var i = 0; i < wheelParticles.GetChild(1).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());

        foreach (var p in flashParticles.GetComponentsInChildren<ParticleSystem>())
            secondaryParticles.Add(p);
        
        foreach (var tube in tubes)
            tubeTurboParticles.Add(kartModel.GetChild(0).Find(tube).GetComponentInChildren<ParticleSystem>());
    }

    protected void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

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
            var amount = Mathf.Abs(xAxis);

            Steer(dir, amount);
        }

        //Drift        
        if (jumpBDown && !drifting && xAxis != 0)
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
            //kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, xAxis * 15, kartModel.localEulerAngles.z), .2f);            
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

        if (!RibaltaDisabilitato)
            if (Ribalta)
            {
                RibaltaDisabilitato = true;

                currentSpeed = 0;
                currentRotate = 0;

                var io = gameObject.transform.parent;

                var ppp = io.position; //forse quella prima è meglio
                var rrr = io.rotation.eulerAngles;
                io.SetPositionAndRotation(new Vector3(ppp.x, PosizionePavimento, ppp.z), Quaternion.Euler(0, rrr.y, 0));

                StartCoroutine(AbilitaRibalta());
            }
    }

    protected void FixedUpdate_()
    {
        //Forward Acceleration
        //if (!drifting)
        //    sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        //else
        //    sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        if (drifting)
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        var theRay = transform.position + (transform.up * .1f);
        //Physics.Raycast(theRay, Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(theRay, Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    private IEnumerator AbilitaRibalta()
    {
        yield return new WaitForSeconds(4);
        RibaltaDisabilitato = false;
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
        if (driftMode > 0)
            switch (KCType)
            {
                case eKCType.Human:
                    //DOVirtual.Float(currentSpeed * 3, currentSpeed, .3f * driftMode, Speed); // per accelerare
                    DOVirtual.Float(currentSpeed * 0.65f, currentSpeed, .3f * driftMode, Speed); //per rallentare
                    DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));
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
                GameObject.Find("CM vcam1").GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                break;
        }

        foreach (var p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    void Speed(float x) =>
        currentSpeed = x;

    void ChromaticAmount(float x) =>
        postProfile.GetSetting<ChromaticAberration>().intensity.value = x;

    public void Accelerate(float amount)
    {
        currentSpeed *= amount;

        if (amount > 1)
            PlayTurboEffect();
    }

    public void LimitSpeed(float speedLimit, int duration)
    {
        LimitSpeed(speedLimit);

        if (!limitSpeed)
            StartCoroutine(RestoreSpeedLimit(duration));
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

    public void AddForce(float force, ForceMode forceMode, Vector3 direction) =>
        sphere.AddForce(direction * force, forceMode);

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
            p.Play();
    }

    protected void setDestination(float xRndError, float zRndError, byte errorDelta)
    {
        CurrentSpline++;

        if (CurrentSpline == CPUSplines.Length)
            CurrentSpline = 0;

        CurrentSplineObject = CPUSplines[CurrentSpline];

        if (CPUSplines[CurrentSpline].forkNumber > 0)
            if (CPUSplines[CurrentSpline].forkNumber == lastUsedFork)
            {
                CurrentSplineObject = CPUSplines[nextSpline(CurrentSpline)];
            }
            else
            {
                lastUsedFork = CPUSplines[CurrentSpline].forkNumber;

                var forks = getForks(CurrentSpline);

                foreach (var fork in forks)
                    if (Random.value < fork.probability)
                    {
                        CurrentSplineObject = fork;
                        break;
                    }
            }

        prevSplinePos = CPUSplines[prevSpline(CurrentSpline)].transform.position;
        curSplinePos = CPUSplines[nextSpline(CurrentSpline)].transform.position;

        var p = CurrentSplineObject.transform.position;

        lastSplineDistance = 0;
        prevSplineDistance = 0;

        lookAtDest = new Vector3(p.x + xRndError, transform.position.y, p.z + zRndError);
    }

    private int nextSpline(int i)
    {
        if (i == CPUSplines.Length)
            i = 0;

        if (CPUSplines[i].forkNumber > 0)
            return nextSpline(i + 1);

        return i;
    }

    private int prevSpline(int i)
    {
        if (i < 1)
            i = CPUSplines.Length;

        i--;

        if (CPUSplines[i].forkNumber > 0)
            return prevSpline(i - 1);

        return i;
    }

    protected List<SplineObject> getForks(sbyte i)
    {
        var forks = new List<SplineObject>();

        var x = i;
        while (x < CPUSplines.Length - 1)
        {
            x++;
            if (CPUSplines[i].forkNumber == CPUSplines[x].forkNumber) // è una forcazione  
                forks.Add(CPUSplines[x]);
        }

        return forks;
    }

}