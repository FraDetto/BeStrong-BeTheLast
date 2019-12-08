/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class ShortcutMovement : MonoBehaviour
{

    public int maxStaticFrames;
    public float basePositionClosed, basePositionOpen;

    private RigidbodyConstraints freezePositionConstraints;
    private Rigidbody thisRigidbody;
    private Vector3 oldPosition;
    private int staticFrames, currentStaticFrames;
    private bool closed = true;


    void Start()
    {
        freezePositionConstraints = RigidbodyConstraints.FreezeAll;
        thisRigidbody = GetComponent<Rigidbody>();
        oldPosition = transform.position;
        staticFrames = Random.Range(0, maxStaticFrames);
    }

    private void Update()
    {
        oldPosition = transform.position;
    }

    void FixedUpdate()
    {
        var ready_ = currentStaticFrames > staticFrames;

        if (closed && ready_ && transform.localPosition.y <= basePositionOpen)
            MoveUp();
        else if (!closed && ready_ && transform.localPosition.y >= basePositionClosed)
            Close();
        else
            MantainPosition(ready_);
    }

    void MoveUp()
    {
        thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        thisRigidbody.AddForce(transform.up * 10f, ForceMode.Acceleration);
    }

    void Close()
    {
        thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    void MantainPosition(bool flipClosed)
    {
        thisRigidbody.constraints = freezePositionConstraints;

        if (flipClosed)
        {
            closed = !closed;
            currentStaticFrames = 0;
            staticFrames = Random.Range(0, maxStaticFrames);
        }

        currentStaticFrames++;
    }

    public void forceChangeState()
    {
        currentStaticFrames = staticFrames + 10;
    }

    void SetBoostEnabled(bool setting)
    {
        var script = GetComponentInChildren<RepulsiveWallStraight>();
        script.SetEnabled(setting);
    }

}