/*
MIT License
Copyright (c) 2019: Francesco Dettori, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GeneralCar))]
[RequireComponent(typeof(Rigidbody))]
public abstract class aAuto : MonoBehaviour
{

    public enum eTrazione
    {
        anteriore, posteriore
    }


    public eTrazione Trazione = eTrazione.anteriore;

    private const short PosizionePavimento = -5;
    protected bool RibaltaDisabilitato = false;
    protected bool Ribalta = false;

    private Quaternion OriginalRotation;
    private Quaternion[] WheelErrorCorrectionR = new Quaternion[4];
    protected WheelCollider[] Colliders = new WheelCollider[4];
    private TrailRenderer[] Scie = new TrailRenderer[4];
    private GameObject[] Wheels = new GameObject[4];

    private Transform CentroDiMassa;
    protected Rigidbody TheCarRigidBody;

    //The class that owns the stats of the faction    
    protected GeneralCar generalCar;

    private Vector3 CentroDiMassaAssettoCorsa, CentroDiMassa3D;

    //private HudScriptManager HUD;    
    protected float instantSteeringAngle, instantTorque;
    protected float xAxis;

    //To manage the sand particle effect
    //private ParticleSystem sandParticle;

    //private AudioSource carAudioSource, audienceConstantSoundAudioSource, hypeAudioSource;


    protected void Start_()
    {
        var i = 0u;
        var wc = GetComponentsInChildren<WheelCollider>();
        var obj_figli = GetComponentsInChildren<MeshRenderer>();

        foreach (var w in wc)
        {
            foreach (var o in obj_figli)
                if (o.gameObject.name.Equals(w.name))
                {
                    WheelErrorCorrectionR[i] = o.gameObject.transform.rotation;
                    Colliders[i] = w;
                    Wheels[i] = o.gameObject;
                    Scie[i] = w.GetComponent<TrailRenderer>();
                    break;
                }

            i++;
        }

        generalCar = GetComponent<GeneralCar>();
        TheCarRigidBody = GetComponent<Rigidbody>();

        CentroDiMassa = transform.Find("CentroDiMassa");
        OriginalRotation = TheCarRigidBody.transform.rotation;

        //var HUDo = GameObject.FindGameObjectWithTag("HUD");
        //HUD = HUDo.GetComponent<HudScriptManager>();

        CentroDiMassa3D = TheCarRigidBody.centerOfMass;
        CentroDiMassaAssettoCorsa = CentroDiMassa.position - transform.position;
        TheCarRigidBody.centerOfMass = CentroDiMassaAssettoCorsa;

        //sandParticle = gameObject.GetComponentInChildren<ParticleSystem>();

        //carAudioSource = gameObject.GetComponent<AudioSource>();
        //hypeAudioSource = GameObject.Find("HypeVoicesManager").GetComponent<AudioSource>();
        //audienceConstantSoundAudioSource = GameObject.Find("ConstantVoicesManager").GetComponent<AudioSource>();
        //audienceConstantSoundAudioSource.Play();
    }

    private IEnumerator AbilitaRibalta()
    {
        yield return new WaitForSeconds(4);
        StopCoroutine(AbilitaRibalta());
        RibaltaDisabilitato = false;
    }

    protected void Update_()
    {
        //DX-SX
        instantSteeringAngle = generalCar.maxSteeringAngle * xAxis;

        //Avanti-dietro
        instantTorque = generalCar.maxTorque;

        if (GB.ms_to_kmh(TheCarRigidBody.velocity.magnitude) > generalCar.Speed)
            instantTorque = 0;

        if (TheCarRigidBody.velocity.magnitude < 0)
            instantTorque *= 3;

        if (!RibaltaDisabilitato)
            if (Ribalta)
            {
                RibaltaDisabilitato = true;

                instantTorque = 0;
                instantSteeringAngle = 0;

                var ppp = TheCarRigidBody.gameObject.transform.position;
                TheCarRigidBody.gameObject.transform.SetPositionAndRotation(new Vector3(ppp.x, 0, ppp.z), OriginalRotation);

                StartCoroutine(AbilitaRibalta());
            }
    }

    protected void FixedUpdate_()
    {
        Quaternion worldPose_rotation;
        Vector3 worldPose_position;

        for (var i = 0u; i < Colliders.Length; i++)
        {
            var FrontWheel = Colliders[i].CompareTag("FrontWheel");

            if (FrontWheel)
                Colliders[i].steerAngle = instantSteeringAngle;

            switch (Trazione)
            {
                case eTrazione.anteriore:
                    if (FrontWheel)
                        Colliders[i].motorTorque = instantTorque * generalCar.Accellerazione * -1;
                    break;

                case eTrazione.posteriore:
                    if (!FrontWheel)
                        Colliders[i].motorTorque = instantTorque * generalCar.Accellerazione * -1;
                    break;
            }

            //rotate the 3d object
            Colliders[i].GetWorldPose(out worldPose_position, out worldPose_rotation);

            Wheels[i].transform.position = worldPose_position;
            Wheels[i].transform.rotation = worldPose_rotation * WheelErrorCorrectionR[i];
        }

        var mySpeed = TheCarRigidBody.velocity.magnitude;

        //sandParticle.playbackSpeed = (generalCar.transform.position.y < PosizionePavimento ? mySpeed / 10 : 0);

        var RuoteCheCollidono = 0u;
        for (var j = 0u; j < Colliders.Length; j++)
            if (Colliders[j].isGrounded)
                RuoteCheCollidono++;

        //quando sei in aria usa il centro di massa al centro del box 3d, altrimenti usa il centro di massa che Michele ha settato a mano per ogni auto            
        TheCarRigidBody.centerOfMass = (RuoteCheCollidono == 0 ? CentroDiMassa3D : CentroDiMassaAssettoCorsa);

        GestioneScie(RuoteCheCollidono, mySpeed);

        generalCar.actualSpeed = mySpeed;

        //HUD.generalCar = generalCar;
        //GB.PlayCarEngine(carAudioSource, mySpeed);
    }

    private void GestioneScie(uint RuoteCheCollidono, float speed)
    {
        var mostra = (speed > 10 && RuoteCheCollidono == 0);

        for (var i = 0u; i < Scie.Length; i++)
            Scie[i].emitting = mostra;
    }


}