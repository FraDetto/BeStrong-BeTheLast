/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class AutoCPU : aAuto
{

    public GameObject CPUSpline;
    private Transform[] CPUSplines;
    private int CurrentSplinePrev = -1;
    private int CurrentSpline = -1;

    private GameObject[] CPUCars;

    private float LastStuck = -1;
    private Vector3 lastPosition;


    void Start()
    {
        CPUCars = GameObject.FindGameObjectsWithTag("CPU");
        CPUSplines = new Transform[CPUSpline.transform.childCount];

        var x = 0u;
        foreach (var el in CPUSpline.transform)
        {
            CPUSplines[x] = el as Transform;
            x++;
        }

        Start_();
    }

    void Update()
    {
        if (CurrentSpline == -1)
            setDestination();

        if (Vector3.Distance(transform.position, CPUSplines[CurrentSpline].position) < 4)
            setDestination();

        var offset = transform.position - CPUSplines[CurrentSpline].position;
        transform.LookAt(transform.position + offset);

        Update_();

        foreach (var cpu in CPUCars)
            if (!cpu.name.Equals(name))
                if (Vector3.Distance(cpu.transform.position, transform.position) < 0.5)
                    instantTorque = 0;

        if (transform.position == lastPosition && TheCarRigidBody.velocity.magnitude < 1)
        {
            if (LastStuck > -1)
                LastStuck = Time.time;

            if (Time.time - LastStuck > 5)
                transform.position = new Vector3(CPUSplines[CurrentSpline].position.x, CPUSplines[CurrentSpline].position.y + 2, CPUSplines[CurrentSpline].position.z);
        }

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        FixedUpdate_();
    }

    void setDestination()
    {
        if (CurrentSpline > -1)
            CPUSplines[CurrentSpline].gameObject.SetActive(false);

        CurrentSplinePrev = CurrentSpline;
        CurrentSpline++;

        if (CurrentSpline == CPUSplines.Length)
        {
            CurrentSpline = 0;

            foreach (var s in CPUSplines)
                s.gameObject.SetActive(true);
        }

        if (CurrentSplinePrev > -1)
            CPUSplines[CurrentSplinePrev].gameObject.GetComponent<Renderer>().material.color = Color.white;

        CPUSplines[CurrentSpline].gameObject.GetComponent<Renderer>().material.color = Color.red;
    }


}