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

    internal bool closed = true;

    private int staticFrames, currentStaticFrames, newStaticFrames;
    private ShortcutInstantClose trigger;


    void Start() =>
        staticFrames = Random.Range(0, maxStaticFrames);

    void FixedUpdate()
    {
        var ready_ = currentStaticFrames > staticFrames;

        if (closed && ready_ && transform.localPosition.y <= basePositionOpen)
            transform.Translate(transform.up * 0.1f);
        else if (!closed && ready_ && transform.localPosition.y >= basePositionClosed)
            transform.Translate(-transform.up * 0.1f);
        else
            MantainPosition(ready_);
    }

    public void setTrigger(ShortcutInstantClose trigger) =>
        this.trigger = trigger;

    void MantainPosition(bool flipClosed)
    {
        if (flipClosed)
        {
            closed = !closed;
            currentStaticFrames = 0;

            if (newStaticFrames == 0)
            {
                staticFrames = Random.Range(0, maxStaticFrames);
            }
            else
            {
                staticFrames = newStaticFrames;
                newStaticFrames = 0;

                if (trigger)
                    trigger.Reset();
            }
        }

        currentStaticFrames++;
    }

    public void forceChangeState() =>
        currentStaticFrames = staticFrames + 10;

    private void imposeNewTimeout(int timeout) =>
        newStaticFrames = 60 * timeout;

    public void CloseNow(int timeout)
    {
        closed = false;
        forceChangeState();
        imposeNewTimeout(timeout);
    }

}