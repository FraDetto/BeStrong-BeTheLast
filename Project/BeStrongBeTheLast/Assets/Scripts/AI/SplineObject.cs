﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using System.Collections.Generic;
using UnityEngine;

public sealed class SplineObject : aCollisionManager, System.IComparable
{

    public enum eSplineType
    {
        Normal, Drift
    }

    public eSplineType splineType = eSplineType.Normal;

    [Range(0, 1)]
    public float probability = 0;

    public List<SplineObject> nextSplines = new List<SplineObject>();

    public ShortcutMovement CanBeClosedByThisWall;
    public SplineObject CanBeClosedByThisWall_AlternativeFork;
    public bool MostraInPlay = false;
    private bool closePortal = false;
    private GameObject portalCar;
    private TeleporterPortal portalToClose;

    internal SplineObject prev_Spline;


    internal SplineObject NextAlternativeSpline
    {
        get
        {
            while (true)
            {
                if (CanBeClosedByThisWall_AlternativeFork)
                {
                    CanBeClosedByThisWall_AlternativeFork.prev_Spline = this;

                    return CanBeClosedByThisWall_AlternativeFork;
                }

                if (prev_Spline.CanBeClosedByThisWall_AlternativeFork)
                    return prev_Spline.CanBeClosedByThisWall_AlternativeFork;
            }
        }
    }

    internal SplineObject NextFirstSpline(aKartController kartController)
    {
        while (true)
        {
            foreach (var nextS in nextSplines)
                if (!IsWallClosed(nextS, kartController))
                {
                    nextS.prev_Spline = this;

                    return nextS;
                }

            if (CanBeClosedByThisWall_AlternativeFork)
            {
                CanBeClosedByThisWall_AlternativeFork.prev_Spline = this;

                return CanBeClosedByThisWall_AlternativeFork;
            }
        }
    }

    internal SplineObject NextRandomSpline(aKartController kartController)
    {
        while (true)
        {
            foreach (var nextS in nextSplines)
                if (!IsWallClosed(nextS, kartController))
                    if (nextS.probability == 0 || Random.value <= nextS.probability)
                    {
                        nextS.prev_Spline = this;

                        return nextS;
                    }

            if (CanBeClosedByThisWall_AlternativeFork)
            {
                CanBeClosedByThisWall_AlternativeFork.prev_Spline = this;

                return CanBeClosedByThisWall_AlternativeFork;
            }
        }
    }

    private void Start()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.enabled = MostraInPlay;

        OnDrawGizmosSelected();
    }

    private bool IsClosed(ShortcutMovement shortcut, aKartController kartController) =>
        shortcut && shortcut.closed && (shortcut.wallClosedBy == null || !shortcut.wallClosedBy.Equals(kartController));

    internal bool IsThisSplineClosed(aKartController kartController) =>
        IsClosed(CanBeClosedByThisWall, kartController) || (prev_Spline && IsClosed(prev_Spline.CanBeClosedByThisWall, kartController));

    private bool IsWallClosed(SplineObject destination, aKartController kartController) =>
        IsClosed(CanBeClosedByThisWall, kartController) || IsClosed(destination.CanBeClosedByThisWall, kartController);

    public int CompareTo(object obj) =>
        obj is SplineObject ? name.CompareTo((obj as SplineObject).name) : 0;

    private void OnDrawGizmosSelected()
    {
        foreach (var s in nextSplines)
            Debug.DrawLine(transform.position, s.gameObject.transform.position, Color.magenta);
    }

    private void OnTriggerEnter(Collider other) =>
        onCollisionWithPlayer_or_CPU(other, (kartController) =>
        {
            /*if (closePortal && other.transform.root.gameObject.name.Equals(portalCar.name))
            {
                portalToClose.ClosePortal();
            }*/
            switch (kartController.KCType)
            {
                case aKartController.eKCType.Human:
                    kartController.SetDestination(0, 0, false, NextFirstSpline(kartController));
                    break;

                case aKartController.eKCType.CPU:
                    kartController.SetDestinationWithError(NextRandomSpline(kartController));
                    break;
            }
        });

    /*public void ClosePortal(TeleporterPortal portal, GameObject car)
    {
        closePortal = true;
        portalCar = car;
        portalToClose = portal;
    }

    public void ResetPortal()
    {
        closePortal = false;
    }*/

}