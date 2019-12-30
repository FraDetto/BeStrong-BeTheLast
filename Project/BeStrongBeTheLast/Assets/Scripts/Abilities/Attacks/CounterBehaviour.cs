/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using UnityEngine;

public class CounterBehaviour : aAbilitiesBehaviour
{

    public SphereCollider sphereCollider;

    internal float raggioDiAzione =>
        sphereCollider.radius;

    internal float diametroDiAzione =>
        raggioDiAzione * 2;

    public float accelerationFromCounter=2f;


    private void Start()
    {
        var abilities = new Dictionary<aBSBTLKart.ePlayer, string>()
        {
            { aBSBTLKart.ePlayer.Bard, "SoundWave" },
            { aBSBTLKart.ePlayer.EarthRestorer, "StungunHit" },
            { aBSBTLKart.ePlayer.Flapper, "GaseousState" },
            { aBSBTLKart.ePlayer.Hypogeum, "LionRoar" },
            { aBSBTLKart.ePlayer.Imps, "StungunHit" },
            { aBSBTLKart.ePlayer.Kiddo, "StungunHit" },
            { aBSBTLKart.ePlayer.Politician, "Bribe" },
            { aBSBTLKart.ePlayer.Steamdunker, "SteamJet" },
        };

        Start_();

        var kartController = transform.root.GetComponentInChildren<KartController>();
        var ability = GB.FindTransformInChildWithName(transform, abilities[kartController.playerType]);

        ability.gameObject.SetActive(true);

        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
        {
            if(!other.transform.root.gameObject.Equals(transform.root.gameObject))
            {
                var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
                kartController.Accelerate(accelerationFromCounter);

                foreach(var c in other.transform.root.GetComponentsInChildren<KartCollision>())
                {
                    c.hitBy = user;
                    StartCoroutine(c.hitByImmunity());
                }
                    
            }   
        }
        else if(other.CompareTag("Obstacles"))
        {

        }
        else if(other.gameObject.layer == 12)
        {

        }
    }

    protected override void LifeTimeElapsed()
    {
        // nothing
    }

}