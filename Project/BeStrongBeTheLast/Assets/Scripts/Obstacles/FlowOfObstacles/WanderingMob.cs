/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WanderingMob : MonoBehaviour
{
    private int rotationSpeed, maxRotationSpeed = 200, rotationFrames, maxRotationFrames, 
        maxRotationFramesSetting = 100, movementFrames, maxMovementFrames, maxMovementFramesSetting = 200;
    private Rigidbody thisRigidbody;
    public float slowAmount = 0.5f;
    private enum Phases
    {
        moving, rotating, flying
    }
    private Phases phase = Phases.moving;

    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        maxMovementFrames = Random.Range(0, maxMovementFramesSetting);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (phase == Phases.moving)
        {
            thisRigidbody.AddForce(transform.forward*Random.Range(3000, 4000), ForceMode.Force);
            movementFrames++;
            if (movementFrames >= maxMovementFrames)
            {
                phase = Phases.rotating;
                movementFrames = 0;
                maxRotationFrames = Random.Range(0, maxRotationFramesSetting);
                rotationSpeed = Random.Range(20, maxRotationSpeed);
            }
        }else if (phase == Phases.rotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotationFrames++;
            if (rotationFrames >= maxRotationFrames)
            {
                phase = Phases.moving;
                rotationFrames = 0;
            }
        }else if (phase == Phases.flying)
        {
            if (thisRigidbody.position.y > 30)
            {
                Destroy(gameObject);
                transform.parent.GetComponent<WanderingMobSpawner>().SpawnNew();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var kartController = collision.collider.transform.parent.GetComponentInChildren<aKartController>();
            var sphereKartRb = collision.collider.transform.parent.GetComponentInChildren<Rigidbody>();

            sphereKartRb.AddForce(-kartController.transform.forward * 200 * kartController.currentSpeed, ForceMode.Impulse);
            kartController.currentSpeed *= slowAmount;
            phase = Phases.flying;
            thisRigidbody.AddForce(transform.up*10000, ForceMode.Impulse);
        }
    }
}
