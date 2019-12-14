/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class MovingRamp : PausableMonoBehaviour
{

    public Transform ramp;
    public float speed = 10;
    private bool down = true;
    public float rotationThreshold = 0.123f;
    private int frames = 0;


    private void Start() =>
        ramp = transform;

    void FixedUpdate()
    {
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
                transform.RotateAround(ramp.position, ramp.right, (down ? 1f : -1f) * speed * Time.deltaTime);
            }
        }
        else
        {
            if (ramp.transform.rotation.x > rotationThreshold)
            {
                down = false;
                transform.RotateAround(ramp.position, ramp.right, -speed * Time.deltaTime);
                frames++;
            }
            else if (ramp.transform.rotation.x < 0)
            {
                down = true;
                transform.RotateAround(ramp.position, ramp.right, speed * Time.deltaTime);
                frames++;
            }
        }
    }

}