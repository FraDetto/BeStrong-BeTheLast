/*
MIT License
Copyright (c) 2019: Francesco Dettori, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

[RequireComponent(typeof(CameraManager))]
public class AutoGuida : aAuto
{

    private bool derapata = false;

    [Range(0, 9)]
    public float MoltiplicatoreEffettoVelocitaCamera = 0.333f;

    private Transform LookHere, Position;
    private float fieldOfView;

    void Start()
    {
        fieldOfView = Camera.main.fieldOfView;

        var MyCamera = Camera.main.GetComponent<CameraManager>();
        MyCamera.lookAtTarget = transform.Find("CameraAnchor/LookHere");
        MyCamera.positionTarget = transform.Find("CameraAnchor/Position");

        Start_();
    }

    private void EffettoVelocitaCamera()
    {
        if (MoltiplicatoreEffettoVelocitaCamera > 0)
            Camera.main.fieldOfView = fieldOfView + (TheCarRigidBody.velocity.magnitude * MoltiplicatoreEffettoVelocitaCamera);
    }

    void Update()
    {
        var xAxis = Input.GetAxis("Horizontal");

        derapata = Input.GetKey(KeyCode.Space);

        Update_(xAxis);

        EffettoVelocitaCamera();
    }

    void FixedUpdate()
    {
        for (var i = 0u; i < Colliders.Length; i++)
        {
            var FrontWheel = Colliders[i].CompareTag("FrontWheel");

            if (!FrontWheel)
            {
                //var sf = Colliders[i].sidewaysFriction;
                //sf.stiffness = (derapata ? 1.7f : 1.2f);
                //Colliders[i].sidewaysFriction = sf;

                Colliders[i].brakeTorque = (derapata ? 2 * generalCar.brakingTorque * generalCar.Accellerazione : 0);
            }
        }

        FixedUpdate_();
    }


}