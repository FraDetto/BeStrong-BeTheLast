/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public sealed class KartController : aBSBTLKart
{

    // ============== HUMAN ==============
    public bool UsaWrongWay = false;

    private bool wrongWay = false;
    private float wrongWayTimer = 2, wrongWayMaxTimer = 1;
    // ============== HUMAN ==============

    // =============== CPU ===============
    private const byte errorDelta = 8;
    private float xRndError, zRndError;

    private GameObject[] CPUCars;

    private float LastStuck = -1;
    private Vector3 lastPosition;
    // =============== CPU ===============    


    private void Start()
    {
        switch (KCType)
        {
            case eKCType.CPU:
                CPUCars = GameObject.FindGameObjectsWithTag("CPU");

                var Box001 = GB.FindTransformInChildWithTag(transform, "Carrozzeria");
                var renderer_ = Box001.gameObject.GetComponent<Renderer>();

                var c = Color.black;

                while (c == Color.black || GB.usedColors.Contains(renderer_.material.color))
                    c = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

                GB.usedColors.Add(c);
                renderer_.material.color = c;
                break;
        }

        Start_();
    }

    bool bJumpReleased;

    private void Update()
    {
        switch (KCType)
        {
            case eKCType.Human:
                if (wrongWay || (!wrongWay && wrongWayTimer < wrongWayMaxTimer))
                {
                    Update_(0, false, false);
                    wrongWayTimer += Time.deltaTime;
                }
                else
                {
                    driftDisabled = false;
                    Update_(Input.GetAxis("Horizontal"), Input.GetButtonDown("Jump"), Input.GetButtonUp("Jump"));
                }

                if (UsaWrongWay)
                {
                    if (CurrentSpline < 0)
                        setDestination(0, 0, 0);

                    var currentSplineDistance1 = Vector3.Distance(transform.position, curSplinePos);
                    var currentSplineDistance0 = Vector3.Distance(transform.position, prevSplinePos);

                    wrongWay =
                        lastSplineDistance > 0 &&
                        prevSplineDistance > 0 &&
                        currentSplineDistance1 > lastSplineDistance &&
                        currentSplineDistance0 < prevSplineDistance;

                    if (wrongWay)
                    {
                        driftDisabled = true;
                        wrongWayTimer = 0;

                        var rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(CPUSplines[CurrentSpline].transform.position), 1f);
                        Debug.Log(CPUSplines[CurrentSpline].gameObject+ " " +rot.eulerAngles.ToString());

                        var eul = rot.eulerAngles;
                        eul.x = 0;
                        eul.z = 0;

                        transform.eulerAngles = eul;

                        var dir = CPUSplines[CurrentSpline].transform.position - transform.position;
                        sphere.AddForce(dir * 100f, ForceMode.Impulse);
                    }

                    lastSplineDistance = currentSplineDistance1;
                    prevSplineDistance = currentSplineDistance0;
                }
                break;
            case eKCType.CPU:
                if (CurrentSpline < 0)
                    setDestinationWithError();

                if (Vector3.Distance(transform.position, lookAtDest) < splineDistance)
                    setDestinationWithError();

                lookAtDest.y = transform.position.y;

                transform.LookAt(lookAtDest);

                var drift_ = CurrentSplineObject.splineType == SplineObject.eSplineType.Drift;
                var jumpBUP = !bJumpReleased && drift_ && driftPower > 250;
                var jumpBDown = drift_ && !jumpBUP;
                var driftAxis = (drift_ ? 0.0001f : 0);

                if (bJumpReleased)
                    bJumpReleased = false;

                if (!drift_)
                    clearDrift();

                Update_(driftAxis, jumpBDown, jumpBUP);

                if (jumpBUP)
                    bJumpReleased = true;

                foreach (var cpu in CPUCars)
                    if (!cpu.name.Equals(name))
                        if (Vector3.Distance(cpu.transform.position, transform.position) < 0.5)
                            speed = 0;

                if (transform.position == lastPosition && currentSpeed < 1)
                {
                    if (LastStuck > -1)
                        LastStuck = Time.time;

                    if (Time.time - LastStuck > 60)
                    {
                        var p = CPUSplines[CurrentSpline].transform.position;
                        transform.position = new Vector3(p.x, p.y + 2, p.z);
                    }
                }

                lastPosition = transform.position;

                CPUAI();
                break;
        }
    }

    void setDestinationWithError()
    {
        xRndError = Random.Range(-1f, 1f) * errorDelta;
        zRndError = Random.Range(-1f, 1f) * errorDelta;

        setDestination(xRndError, zRndError, errorDelta);
    }

    internal void nextSpline() =>
        setDestination(0, 0, 0);

    private void FixedUpdate() =>
        FixedUpdate_();

    private void CPUAI()
    {

    }

    internal void fieldOfViewCollision(FieldOfViewCollider.eDirection direction, Collider other)
    {
        //
    }

    public void resetLastSplineDistance()
    {
        lastSplineDistance = 0;
    }

}