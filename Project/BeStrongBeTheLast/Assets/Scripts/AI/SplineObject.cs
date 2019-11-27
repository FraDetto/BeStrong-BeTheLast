/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SplineObject : MonoBehaviour, IComparable
{

    public enum eSplineType
    {
        Normal, Drift
    }

    public eSplineType splineType = eSplineType.Normal;

    [Range(0, 254)]
    public byte forkNumber = 0;

    [Range(0, 1)]
    public float probability = 0;

    private SplineObject[] CPUSplines;
    private byte CurrentSpline;

    List<Vector3> lookAtDests_ = new List<Vector3>();
    List<Vector3> lookAtDests
    {
        get
        {
            if (CPUSplines == null)
            {
                var CPUSpline = transform.parent;

                CPUSplines = new SplineObject[CPUSpline.transform.childCount];

                byte x = 0;
                foreach (var el in CPUSpline.transform)
                {
                    var t = el as Transform;
                    CPUSplines[x] = t.gameObject.GetComponent<SplineObject>();

                    if (CPUSplines[x].name.Equals(this.name))
                        CurrentSpline = x;

                    x++;
                }

                Array.Sort(CPUSplines);

                var next = CurrentSpline + 1;

                while (forkNumber > 0 && CPUSplines[next].forkNumber == forkNumber)
                {
                    next++;

                    if (next == CPUSplines.Length)
                        next = 0;
                }

                if (next == CPUSplines.Length)
                    next = 0;

                if (CPUSplines[next].forkNumber > 0 && CPUSplines[next].forkNumber != forkNumber)
                {
                    var forks = getForks(next);

                    foreach (var fork in forks)
                        lookAtDests_.Add(fork.transform.position);
                }

                lookAtDests_.Add(CPUSplines[next].transform.position);
            }

            return lookAtDests_;
        }
    }

    private void Start()
    {
        OnDrawGizmosSelected();
    }

    private List<SplineObject> getForks(int i)
    {
        var forks = new List<SplineObject>();

        var x = 0;
        while (x < CPUSplines.Length)
        {
            if (CPUSplines[i].forkNumber == CPUSplines[x].forkNumber) // è una forcazione  
                forks.Add(CPUSplines[x]);

            x++;
        }

        return forks;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var lookAtDest in lookAtDests)
            Debug.DrawLine(transform.position, lookAtDest, Color.magenta);
    }

    public int CompareTo(object obj)
    {
        if (obj is SplineObject)
        {
            var so = obj as SplineObject;

            return name.CompareTo(so.name);
        }

        return 0;
    }

}