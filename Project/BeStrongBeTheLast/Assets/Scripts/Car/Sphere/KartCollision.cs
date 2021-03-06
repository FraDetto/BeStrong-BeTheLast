﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors: Michele Maione, Riccardo Lombardi
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using System.Collections;
using UnityEngine;

public class KartCollision : aCollisionManager
{

    public enum Mode
    {
        left,
        right,
        rear
    }

    public Mode mode;

    private KartController myKartController;


    internal GameObject hitBy;
    internal float rotatingPush = 1f;


    private void Start()
    {
        var kart = transform.root.GetChild(0);
        myKartController = kart.GetComponent<KartController>();
    }

    internal IEnumerator hitByImmunity()
    {
        yield return new WaitForSeconds(2f);
        hitBy = null;
    }

    private void OnTriggerEnter(Collider collider) =>
        onCollisionWithPlayer_or_CPU(collider, (kartController) =>
        {
            if (myKartController && kartController && myKartController != kartController)
            {
                var speedDifference = Mathf.Abs(myKartController.currentSpeed - kartController.currentSpeed);
                //var forceModifier = speedDifference / Mathf.Max(0.01f, myKartController.currentSpeed > kartController.currentSpeed ? myKartController.currentSpeed : kartController.currentSpeed);
                var hitDirection = collider.transform.position - transform.position;

                switch (mode)
                {
                    case Mode.left:
                    case Mode.right:
                        if (!hitBy || !kartController.transform.root.gameObject)
                            myKartController.Accelerate(0.875f, 2f);
                        break;

                    case Mode.rear:
                        if (!hitBy || !kartController.transform.root.gameObject)
                            if (myKartController.CurrentSplineDistance <= kartController.CurrentSplineDistance && speedDifference > 1)
                            {
                                myKartController.Accelerate(1.25f, 2f);
                                kartController.Accelerate(0.75f, 2f);
                            }
                        break;
                }
            }
        });

}