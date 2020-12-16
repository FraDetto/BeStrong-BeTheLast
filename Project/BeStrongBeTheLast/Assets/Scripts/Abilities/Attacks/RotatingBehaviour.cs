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

public class RotatingBehaviour : aAbilitiesBehaviour
{
    float accelerationFromShot = 1.25f;
    public float rotatingSpeed = 2f;
    public float targetRadius = 5f;

    private CapsuleCollider collider_; // collider esiste in MonoBehaviour


    private void Start()
    {
        Start_();

        user = transform.root.gameObject;
        user.GetComponentInChildren<KartController>().counterImmunity = true;
        collider_ = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if(Paused)
            return;

        if (collider_.radius < targetRadius)
        {
            collider_.radius += Time.deltaTime;
            if (collider_.radius > targetRadius)
                collider_.radius = targetRadius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GB.CompareORTags(other, "Player", "CPU"))
            if (!other.transform.root.gameObject.Equals(user))
            {
                var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
                kartController.Accelerate(accelerationFromShot, 2f);

                foreach (var c in other.transform.root.GetComponentsInChildren<KartCollision>())
                {
                    c.hitBy = user;
                    StartCoroutine(c.hitByImmunity());
                }
            }
    }

    protected override void LifeTimeElapsed()
    {
        user.GetComponentInChildren<KartController>().counterImmunity = false;
    }

}