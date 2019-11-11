/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpingMob : WanderingMob
{
    private new Phases phase = Phases.moving;
    private bool jumped = false;
    public float slowAmountSquished = 0.1f, squishedSpeedLimit = 30f;
    public int squishDuration = 7;
    
    private void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        maxMovementFrames = Random.Range(minMovementFramesSetting, maxMovementFramesSetting);
        spawner = transform.parent.GetChild(0);
    }

    private void FixedUpdate()
    {
        if (phase == Phases.moving)
        {
            Moving();
        }
        else if (phase == Phases.rotating)
        {
            Rotating();
        }
        else if (phase == Phases.flying)
        {
            Flying();
        }
    }

    protected void Moving()
    {
        force = Random.Range(1000, 2000);
        int jumpForce = Random.Range(1000, 2000);
        if (!jumped)
        {
            thisRigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
            thisRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumped = true;
            movementFrames = 0;
        }

        movementFrames++;
    }
    protected void Rotating()
    {
        if (Vector3.Distance(transform.position, spawner.position) > (roadWidth / 2))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotationSpeed = 100;
            Vector3 targetDir = transform.position - spawner.position;
            if (Vector3.Angle(transform.forward, -targetDir) < 20f)
            {
                phase = Phases.moving;
                rotationFrames = 0;
            }
        }
        else
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotationFrames++;

            if (rotationFrames >= maxRotationFrames)
            {
                phase = Phases.moving;
                rotationFrames = 0;
            }  
        }
    }
    protected void Flying()
    {
        if (thisRigidbody.position.y > 30)
        {
            Destroy(gameObject);
            transform.parent.GetComponent<JumpingMobSpawner>().SpawnNew(avoidBehaviour);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("CPU"))
        {
            if (phase == Phases.rotating)
            {
                var kartController = collision.collider.transform.parent.GetComponentInChildren<aKartController>();
                Vector3 hitDirection = collision.collider.transform.position - transform.position;
                
                kartController.AddForce(200 * kartController.currentSpeed, ForceMode.Impulse, -kartController.transform.forward);
                kartController.Accelerate(slowAmount);


                phase = Phases.flying;

                rotationFrames = 0;
                movementFrames = 0;

                thisRigidbody.AddForce((transform.up - hitDirection) * 12000, ForceMode.Impulse); 
            }else if (phase == Phases.moving)
            {
                var kartController = collision.collider.transform.parent.GetComponentInChildren<aKartController>();
                kartController.Accelerate(slowAmountSquished);
                kartController.BeSquished(squishDuration);
                kartController.LimitSpeed(squishedSpeedLimit, squishDuration);
            }
        }
        else
        {
            if (jumped && phase == Phases.moving && movementFrames > 10)
            {
                jumped = false;
                phase = Phases.rotating;
                maxRotationFrames = Random.Range(minRotationFramesSetting, maxRotationFramesSetting);
                rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);

            }
        }
    }

}