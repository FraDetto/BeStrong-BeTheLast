/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class SingleShotBehaviour : aAbilitiesBehaviour
{

    public LayerMask roadMask, wallMask;

    TrishotBehaviour trishotBehaviour;


    private void Start() =>
        Start_();

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, roadMask))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.up, hit.normal, 1f), -transform.forward);
            transform.Rotate(Vector3.right, 90f);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 1f, wallMask))
            KillMe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GB.CompareORTags(other, "Player", "CPU"))
        {
            trishotBehaviour = GetComponentInParent<TrishotBehaviour>();

            if (!other.transform.root.gameObject.Equals(trishotBehaviour.user))
            {
                var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
                kartController.Accelerate(trishotBehaviour.accelerationFromShot);

                foreach (var c in other.transform.root.GetComponentsInChildren<KartCollision>())
                {
                    c.hitBy = user;
                    StartCoroutine(c.hitByImmunity());
                }

                KillMe();
            }
        }
    }

    protected override void LifeTimeElapsed()
    {
        // nothing
    }

}