﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TeleporterPortal : PausableMonoBehaviour
{
    public Transform CPUSplineRoot;
    public SplineObject assignedSpline;

    public GameObject end;

    private int kartPassed;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Paused)
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;

        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            kartPassed += 1;
            //endScriptCallback.corControl = false;
            TeleportCar(go);
            if(kartPassed == 8)
                StartCoroutine(ClosePortal());
        }
    }

    private IEnumerator ClosePortal()
    {
        yield return new WaitForSeconds(5f);
        end.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    public void TeleportCar(GameObject car)
    {
        var controller = car.GetComponentInChildren<KartController>();
        var sphere = controller.sphere;

        /*int lap, splineIndex, splineScore, splineTot = CPUSplineRoot.childCount;

        splineScore = GameState.Instance.averageSplineScore;
        lap = Mathf.FloorToInt(splineScore / (float)splineTot);
        splineIndex = Mathf.Max(0, (splineScore - lap * splineTot) % splineTot); */

        // Debug.Log(splineIndex + "/" + splineTot);

        /*var splineTransform = CPUSplineRoot.GetChild(splineIndex).transform;

        if(splineIndex >= controller.CurrentSplineObject.transform.GetSiblingIndex())
            GameState.Instance.laps[controller.transform.root.gameObject.name]--; */

        sphere.transform.position = assignedSpline.transform.position;

        // Miky: COSA FA?

        //controller.rotateToSpline = true;
    }
}