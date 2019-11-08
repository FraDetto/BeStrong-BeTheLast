/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class KartController : aKartController
{

    private bool wrongWay = false;
    private float lastSplineDistance;

    private void Start()
    {
        Start_();
    }

    private void Update()
    {
        if (wrongWay)
            Update_(0, false, false);
        else
            Update_(Input.GetAxis("Horizontal"), Input.GetButtonDown("Jump"), Input.GetButtonUp("Jump"));

        if (CurrentSpline < 0)
            setDestination();

        var currentSplineDistance = Vector3.Distance(transform.position, lookAtDest);


        if (lastSplineDistance > 0 && currentSplineDistance > lastSplineDistance)
            //Debug.LogError("Wrong Way!");
        wrongWay = (lastSplineDistance > 0 && currentSplineDistance > lastSplineDistance);

        if (wrongWay)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(CPUSplines[CurrentSpline].transform.position), 0.15f);
            var dir = CPUSplines[CurrentSpline].transform.position - transform.position;
            sphere.AddForce(dir * 400f, ForceMode.Impulse);
        }

        lastSplineDistance = currentSplineDistance;
    }

    internal void nextSpline()
    {
        CPUSplines[CurrentSpline].gameObject.GetComponent<Renderer>().material.color = Color.white;
        setDestination();
    }

    private void FixedUpdate()
    {
        FixedUpdate_();
    }

    void setDestination()
    {
        CurrentSpline++;
        lastSplineDistance = 0;

        if (CurrentSpline == CPUSplines.Length)
            CurrentSpline = 0;

        CPUSplines[CurrentSpline].gameObject.GetComponent<Renderer>().material.color = Color.magenta;

        lookAtDest = new Vector3(
            CPUSplines[CurrentSpline].position.x,
            transform.position.y,
            CPUSplines[CurrentSpline].position.z
        );
    }


}