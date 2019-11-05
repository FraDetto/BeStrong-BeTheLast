﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoxMovement : MonoBehaviour
{

    public float accelerationFromBox;  
    public float ImpulseFromBox;
    public bool automaticallyMove = true;

    private Rigidbody thisRigidbody;


    void Start()
    {
       
        thisRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (automaticallyMove)
            thisRigidbody.AddForce(transform.forward, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var kartController = collision.collider.transform.parent.GetComponentInChildren<aKartController>();
            var sphereKartRb = collision.collider.transform.parent.GetComponentInChildren<Rigidbody>();
           
            Debug.Log("nome transform " + kartController.name);
            sphereKartRb.AddForce(-kartController.transform.forward * ImpulseFromBox* kartController.currentSpeed, ForceMode.Impulse);
            kartController.currentSpeed *= accelerationFromBox;
        

            Destroy(gameObject);
        }
    }

}