using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRamp : MonoBehaviour
{
    public Transform ramp;
    public float speed = 10;
    private bool down = true;
    public float rotationThreshold = 0.123f;
    private int frames = 0;

    private void Start()
    {
        
    }

    void FixedUpdate() {
        if (ramp.transform.rotation.x <= rotationThreshold && ramp.transform.rotation.x >= 0)
        {
            if (frames != 0)
            {
                frames++;
                if (frames == 180)
                    frames = 0;
            }
            else
            {
                if (down == true)
                    transform.RotateAround(ramp.position, ramp.right, speed * Time.deltaTime);
                else
                    transform.RotateAround(ramp.position, ramp.right, -speed * Time.deltaTime);
            }

            
            
        }
        else
        {
            if (ramp.transform.rotation.x > rotationThreshold)
            {
                down = false;
                transform.RotateAround(ramp.position, ramp.right, -speed * Time.deltaTime);
                frames++;
            } else if (ramp.transform.rotation.x < 0)
            {
                down = true;
                transform.RotateAround(ramp.position, ramp.right, speed * Time.deltaTime);
                frames++;
            }
        }
    }
}
