/*
MIT License
Copyright (c) 2019: Francesco Dettori, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.AI;

public class AutoCPU : MonoBehaviour
{
    public enum eTrazione
    {
        anteriore, posteriore
    }

    public eTrazione Trazione = eTrazione.anteriore;

    private const short PosizionePavimento = -5;
    private bool RibaltaDisabilitato = false;

    //private TrailRenderer[] Scie = new TrailRenderer[4];    

    private Transform CentroDiMassa;
    private Rigidbody TheCarRigidBody;

    //The class that owns the stats of the faction    
    private GeneralCar generalCar;

    private Vector3 CentroDiMassaAssettoCorsa, CentroDiMassa3D;

    //private HudScriptManager HUD;

    //To manage the sand particle effect
    //private ParticleSystem sandParticle;

    //private AudioSource carAudioSource, audienceConstantSoundAudioSource, hypeAudioSource;

    private NavMeshAgent agent;
    public GameObject player;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        generalCar = GetComponent<GeneralCar>();
        TheCarRigidBody = GetComponent<Rigidbody>();
        CentroDiMassa = transform.Find("CentroDiMassa");

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

    private void Update()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.destination = new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }


}