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
        myKartController = GB.FindComponentInDadWithName<KartController>(transform, "Kart");
    }

    private void OnTriggerEnter(Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {
            var hitDirection = collider.transform.position - transform.position;

            var fast = myKartController;
            var slow = kartController;

            if (fast != slow)
            {
                Debug.Log(slow + " | " + fast);

                var speedDifference = Mathf.Abs(fast.currentSpeed - slow.currentSpeed);
                var forceModifier = (fast.currentSpeed > slow.currentSpeed) ? (speedDifference / fast.currentSpeed) : (speedDifference / slow.currentSpeed);

                switch (mode)
                {
                    case Mode.left:
                    case Mode.right:
                        kartController.AddForce(2000 + 1000 * forceModifier, ForceMode.Impulse, hitDirection);
                        break;

                    case Mode.rear:
                        if (fast.currentSplineDistance <= kartController.currentSplineDistance && speedDifference > 1)
                        {
                            fast.AddForce(200 * forceModifier, ForceMode.Impulse, hitDirection);
                            slow.AddForce(200 * forceModifier, ForceMode.Impulse, -hitDirection);

                            fast.Accelerate(1.1f + 1f * forceModifier);
                            slow.Accelerate(0.9f - 0.5f * forceModifier);
                        }
                        break;
                }
            }
        }, "Player", "CPU");
    }
}
