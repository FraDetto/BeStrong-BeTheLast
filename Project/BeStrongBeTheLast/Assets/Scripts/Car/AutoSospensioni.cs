﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class AutoSospensioni : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float destrezza = 10;

    [Range(0f, 3f)]
    public float smorzamento = 0.8f;

    [Range(-1f, 1f)]
    public float distanzaDalCentroDiMassa = 0.03f;

    public bool regolaDistanzaMolle = true;

    private Rigidbody m_Rigidbody;


    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        foreach (var wc in GetComponentsInChildren<WheelCollider>())
        {
            var molla = wc.suspensionSpring;

            var sqrtSprungMass = Mathf.Sqrt(wc.sprungMass);
            molla.spring = sqrtSprungMass * destrezza * sqrtSprungMass * destrezza;
            molla.damper = 2f * smorzamento * Mathf.Sqrt(molla.spring * wc.sprungMass);

            wc.suspensionSpring = molla;

            var corpo = transform.InverseTransformPoint(wc.transform.position);
            var distance = m_Rigidbody.centerOfMass.y - corpo.y + wc.radius;

            wc.forceAppPointDistance = distance - distanzaDalCentroDiMassa;

            if (molla.targetPosition > 0 && regolaDistanzaMolle)
                wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (molla.targetPosition * molla.spring);
        }
    }


}