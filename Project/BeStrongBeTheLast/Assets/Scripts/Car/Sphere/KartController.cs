﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class KartController : aBSBTLKart
{

    // ============== HUMAN ==============
    public bool UsaWrongWay = false;

    private float wrongWayTimer = 2, wrongWayMaxTimer = 1;
    // ============== HUMAN ==============

    // =============== CPU ===============
    private const byte errorDelta = 8;

    private GameObject[] CPUCars;

    private bool bJumpReleased;
    private float LastStuck = -1;
    private Vector3 lastPosition;

    private static HashSet<GameObject> currentObstacleOtherCPU = new HashSet<GameObject>();
    private Stack<GameObject> excludeObstacles = new Stack<GameObject>(5);
    private GameObject currentObstacle;
    // =============== CPU ===============    


    private GameObject excludeObstacle =>
        excludeObstacles.Count > 0 ? excludeObstacles.Peek() : null;


    private void Start()
    {
        switch (KCType)
        {
            case eKCType.Human:
                setDestination(0, 0, true);
                break;

            case eKCType.CPU:
                CPUCars = GameObject.FindGameObjectsWithTag("CPU");

                var Box001 = GB.FindTransformInChildWithTag(transform, "Carrozzeria");
                var renderer_ = Box001.gameObject.GetComponent<Renderer>();

                var c = Color.black;

                while (c == Color.black || GB.usedColors.Contains(renderer_.material.color))
                    c = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

                GB.usedColors.Add(c);
                renderer_.material.color = c;

                setDestinationWithError();

                break;
        }

        Start_();
    }

    private void Update()
    {
        var wrong = wrongWay;

        switch (KCType)
        {
            case eKCType.Human:
                Debug.Log("Speed" + currentSpeed);

                if (Vector3.Distance(transform.position, lookAtDestOriginal) < splineDistance)
                    setDestination(0, 0, false, CurrentSplineObject);

                if (wrong || (!wrong && wrongWayTimer < wrongWayMaxTimer))
                {
                    Update_(0, false, false);
                    wrongWayTimer += Time.deltaTime;
                }
                else
                {
                    driftDisabled = false;
                    Update_(Input.GetAxis("Horizontal"), Input.GetButtonDown("Jump"), Input.GetButtonUp("Jump"));
                }

                if (UsaWrongWay && wrong)
                {
                    SetOnTrack();
                    wrongWayTimer = 0;
                    driftDisabled = true;
                }
                break;

            case eKCType.CPU:
                if (Vector3.Distance(transform.position, lookAtDest) < splineDistance)
                    if (GB.compareVector3(GB.EAxis.Y, lookAtDest, lookAtDestOriginal))
                        setDestinationWithError();

                CPUAI(wrong);

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
                        var p = CurrentSplineObject.transform.position;
                        transform.position = new Vector3(p.x, p.y + 2, p.z);
                    }
                }

                lastPosition = transform.position;
                break;
        }

        Debug.DrawLine(transform.position, lookAtDestOriginal, Color.green);
    }

    private void FixedUpdate() =>
        FixedUpdate_();

    private bool wrongWay
    {
        get
        {
            var currentSplineDistance1 = Vector3.Distance(transform.position, curSplinePos);
            var currentSplineDistance0 = Vector3.Distance(transform.position, prevSplinePos);

            var wrong =
                lastSplineDistance > 0 &&
                prevSplineDistance > 0 &&
                currentSplineDistance1 > lastSplineDistance &&
                currentSplineDistance0 < prevSplineDistance;

            lastSplineDistance = currentSplineDistance1;
            prevSplineDistance = currentSplineDistance0;

            return wrong;
        }
    }

    public float currentSplineDistance =>
        Vector3.Distance(transform.position, curSplinePos);

    void setDestinationWithError() =>
        setDestination(GB.NormalizedRandom(-1f, 1f) * errorDelta, GB.NormalizedRandom(-1f, 1f) * errorDelta);

    internal void nextSpline() =>
        setDestination(0, 0);


    private void CPUAI(bool wrong)
    {
        if (wrong)
        {
            if (currentObstacle != null && excludeObstacle != currentObstacle)
                excludeObstacles.Push(currentObstacle);

            if (currentObstacle != null)
                currentObstacleOtherCPU.Remove(currentObstacle);

            currentObstacle = null;
            lookAtDest = lookAtDestOriginal;
        }
        else
        {
            if (currentObstacle != null && excludeObstacle != currentObstacle && Vector3.Distance(transform.position, currentObstacle.transform.position) < 30)
            {
                lookAtDest = currentObstacle.transform.position;
            }
            else
            {
                foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
                    if (root.name.Equals("Obstacles"))
                        foreach (var obstacle in GB.FindGameObjectsInChildWithTag(root.transform, "Obstacles"))
                            if (Vector3.Distance(transform.position, obstacle.transform.position) < 30)
                                if (excludeObstacle != obstacle)
                                    if (!currentObstacleOtherCPU.Contains(obstacle))
                                    {
                                        currentObstacle = obstacle;
                                        currentObstacleOtherCPU.Add(currentObstacle);
                                        break;
                                    }

                lookAtDest = currentObstacle == null ? lookAtDestOriginal : currentObstacle.transform.position;
            }
        }
    }

    internal void SetObstacleDestroyed(GameObject gameObject)
    {
        excludeObstacles.Push(gameObject);
        currentObstacleOtherCPU.Remove(currentObstacle);
        currentObstacle = null;
    }

    internal void fieldOfViewCollision(FieldOfViewCollider.eDirection direction, Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {

        }, "Player", "CPU");
    }

}