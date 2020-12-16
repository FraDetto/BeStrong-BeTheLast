/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoxMovement : aCollisionManager
{

    float accelerationFromBox = 1.25f;
    public float ImpulseFromBox;
    public bool automaticallyMove = true;
    public Transform kartModel;
    public float oldX = 0;

    private Rigidbody thisRigidbody;


    void Start() =>
        thisRigidbody = GetComponent<Rigidbody>();

    void Update()
    {
        if (automaticallyMove)
        {
            oldX += 0.5f;
            thisRigidbody.AddForce(-transform.right * 0.04f, ForceMode.Impulse);
            transform.localEulerAngles = new Vector3(oldX, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision) =>
        onCollisionWithPlayer_or_CPU(collision.collider, (kartController) =>
        {
            kartController.AddForce(ImpulseFromBox, ForceMode.Impulse, -kartController.transform.forward, true);
            kartController.Accelerate(accelerationFromBox, 2f);

            if (ImpulseFromBox == 0)
                kartController.PlayTurboEffect();

            Destroy(gameObject);
        });

}