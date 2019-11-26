/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WanderingMob : aCollisionManager
{

    public enum avoidBehaviourOptions
    {
        CPU, Player, Mob
    }

    protected enum Phases
    {
        moving, rotating, flying, avoidingA, avoidingB
    }

    protected int rotationSpeed,
        maxRotationSpeed = 200,
        rotationFrames,
        maxRotationFrames,
        maxRotationFramesSetting = 100,
        movementFrames,
        flyingFrames,
        maxFlyingFrames = 120,
        maxMovementFrames,
        minMovementFramesSetting = 35,
        minRotationFramesSetting = 20,
        maxMovementFramesSetting = 130,
        force;

    protected long lastAvoidanceFrame = 300;

    protected Rigidbody thisRigidbody;
    protected Transform spawner;

    protected Phases phase;

    public float slowAmount = 0.5f;
    public float roadWidth;
    public List<avoidBehaviourOptions> avoidBehaviour = new List<avoidBehaviourOptions>();


    public WanderingMob()
    {
        phase = Phases.moving;
    }

    private void Start()
    {
        Start_();
    }

    protected void Start_()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        maxMovementFrames = Random.Range(minMovementFramesSetting, maxMovementFramesSetting);
        spawner = transform.parent.GetChild(0);
    }

    private void FixedUpdate()
    {
        AvoidingCheck();

        switch (phase)
        {
            case Phases.moving:
                Moving();
                break;
            case Phases.rotating:
                Rotating();
                break;
            case Phases.flying:
                Flying();
                break;
            case Phases.avoidingA:
                AvoidingA();
                break;
            case Phases.avoidingB:
                AvoidingB();
                break;
        }
    }

    void Moving()
    {
        thisRigidbody.AddForce(transform.forward * force, ForceMode.Force);
        movementFrames++;

        if (movementFrames >= maxMovementFrames)
        {
            phase = Phases.rotating;
            movementFrames = 0;
            maxRotationFrames = Random.Range(minRotationFramesSetting, maxRotationFramesSetting);
            rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        }
    }

    void Rotating()
    {
        if (Vector3.Distance(transform.position, spawner.position) > (roadWidth / 2))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotationSpeed = 100;

            Vector3 targetDir = transform.position - spawner.position;

            if (Vector3.Angle(transform.forward, -targetDir) < 20f)
            {
                phase = Phases.moving;
                force = Random.Range(3300, 3400);
                rotationFrames = 0;
                maxMovementFrames = Random.Range(minMovementFramesSetting, maxMovementFramesSetting);
            }
        }
        else
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotationFrames++;

            if (rotationFrames >= maxRotationFrames)
            {
                phase = Phases.moving;
                force = Random.Range(3300, 3400);
                rotationFrames = 0;
                maxMovementFrames = Random.Range(minMovementFramesSetting, maxMovementFramesSetting);
            }
        }
    }

    void AvoidingCheck()
    {
        if (lastAvoidanceFrame < 250)
            lastAvoidanceFrame++;
        else
            foreach (var hitCollider in Physics.OverlapSphere(transform.position, 10))
                if (AvoidTag(hitCollider))
                    if (phase != Phases.avoidingA && phase != Phases.avoidingB)
                    {
                        phase = Phases.avoidingA;
                        lastAvoidanceFrame = 0;
                        rotationFrames = 0;
                        movementFrames = 0;
                        maxRotationFrames = Random.Range(0, 10);
                        rotationSpeed = Random.Range(100, 500);
                        break;
                    }
    }

    void AvoidingA()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        rotationFrames++;
        if (rotationFrames >= maxRotationFrames)
        {
            phase = Phases.avoidingB;
            rotationFrames = 0;
        }
    }

    void AvoidingB()
    {
        thisRigidbody.AddForce(transform.forward * Random.Range(2000, 3000), ForceMode.Impulse);
        phase = Phases.rotating;
        maxRotationFrames = Random.Range(minRotationFramesSetting, maxRotationFramesSetting);
        rotationSpeed = Random.Range(20, maxRotationSpeed);
    }

    void Flying()
    {
        if (thisRigidbody.position.y > 30 || flyingFrames > maxFlyingFrames)
        {
            transform.parent.GetComponent<WanderingMobSpawner>().SpawnNew(avoidBehaviour);
            Destroy(gameObject);
        }
        flyingFrames++;
    }

    void OnCollisionEnter(Collision collision)
    {
        onCollisionWithTags(collision.collider, (kartController) =>
        {
            var hitDirection = collision.collider.transform.position - transform.position;

            kartController.AddForce(200 * kartController.currentSpeed, ForceMode.Impulse, -kartController.transform.forward);
            kartController.Accelerate(slowAmount);
            kartController.currentObstacle = null;

            phase = Phases.flying;

            rotationFrames = 0;
            movementFrames = 0;

            thisRigidbody.AddForce((transform.up - hitDirection) * 12000, ForceMode.Impulse);
        }, "Player", "CPU");
    }

    bool AvoidTag(Collider collider)
    {
        for (var i = 0; i < avoidBehaviour.Count; i++)
            if (collider.CompareTag(avoidBehaviour[i].ToString()))
                return true;

        return false;
    }

}