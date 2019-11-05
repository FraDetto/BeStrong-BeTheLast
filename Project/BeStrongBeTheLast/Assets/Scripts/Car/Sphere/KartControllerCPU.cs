/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class KartControllerCPU : aKartController
{

    private const byte errorDelta = 8;
    private float xRndError, zRndError;

    private GameObject[] CPUCars;

    private float LastStuck = -1;
    private Vector3 lastPosition;


    private void Start()
    {
        CPUCars = GameObject.FindGameObjectsWithTag("CPU");

        var Box001 = GB.FindTransformInChildWithTag(transform, "Carrozzeria");
        var renderer = Box001.gameObject.GetComponent<Renderer>();
        renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

        Start_();
    }

    private void Update()
    {
        if (CurrentSpline < 0)
            setDestination();

        if (Vector3.Distance(transform.position, lookAtDest) < splineDistance)
            setDestination();

        lookAtDest.y = transform.position.y;

        transform.LookAt(lookAtDest);

        Update_(0, false, false);

        foreach (var cpu in CPUCars)
            if (!cpu.name.Equals(name))
                if (Vector3.Distance(cpu.transform.position, transform.position) < 0.5)
                    speed = 0;

        if (transform.position == lastPosition && currentSpeed < 1)
        {
            if (LastStuck > -1)
                LastStuck = Time.time;

            if (Time.time - LastStuck > 60)
                transform.position = new Vector3(CPUSplines[CurrentSpline].position.x, CPUSplines[CurrentSpline].position.y + 2, CPUSplines[CurrentSpline].position.z);
        }

        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        FixedUpdate_();
    }

    void setDestination()
    {
        xRndError = Random.Range(-1f, 1f) * errorDelta;
        zRndError = Random.Range(-1f, 1f) * errorDelta;

        CurrentSpline++;

        if (CurrentSpline == CPUSplines.Length)
            CurrentSpline = 0;

        lookAtDest = new Vector3(
            CPUSplines[CurrentSpline].position.x + xRndError,
            transform.position.y,
            CPUSplines[CurrentSpline].position.z + zRndError
        );
    }

}