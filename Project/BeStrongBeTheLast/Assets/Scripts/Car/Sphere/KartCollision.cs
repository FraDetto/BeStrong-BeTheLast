/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
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


    private void Start()
    {
        var kart = transform.root.GetChild(0);
        myKartController = kart.GetComponent<KartController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {
            if (myKartController && kartController && myKartController != kartController)
            {
                var speedDifference = Mathf.Abs(myKartController.currentSpeed - kartController.currentSpeed);
                var forceModifier = (myKartController.currentSpeed > kartController.currentSpeed) ? (speedDifference / myKartController.currentSpeed) : (speedDifference / kartController.currentSpeed);
                var hitDirection = collider.transform.position - transform.position;

                switch (mode)
                {
                    case Mode.left:
                    case Mode.right:
                        kartController.AddForce(2000 + 1000 * forceModifier, ForceMode.Impulse, hitDirection);
                        break;

                    case Mode.rear:
                        if (myKartController.currentSplineDistance <= kartController.currentSplineDistance && speedDifference > 1)
                        {
                            myKartController.AddForce(200 * forceModifier, ForceMode.Impulse, hitDirection);
                            kartController.AddForce(200 * forceModifier, ForceMode.Impulse, -hitDirection);

                            myKartController.Accelerate(1.1f + 1f * forceModifier);
                            kartController.Accelerate(0.9f - 0.5f * forceModifier);
                        }
                        break;
                }
            }
        }, "Player", "CPU");
    }

}