/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using Assets.Scripts.Obstacles.Base;
using UnityEngine;

public class RepulsiveWallStraight : aCollisionManager
{

    public bool AttivaCollisioniConMura;

    private bool active = true;
    private KartController kartController;
    private float distToGround;


    private void Start()
    { 
        kartController = transform.parent.GetChild(0).GetComponentInChildren<KartController>();
        distToGround = transform.GetComponent<Collider>().bounds.extents.y;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }

    private void FixedUpdate()
    {
        kartController.touchingGround = IsGrounded();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var pavimento = false;

        foreach (var contact in collision.contacts)
            if (contact.normal.x == 0 && contact.normal.z == 0 && contact.normal.y == 1)
            {
                kartController.gravity_ = kartController.gravity;
                pavimento = true;
                break;
            }

        if (!pavimento)
        {
            kartController.gravity_ += 5;
        }

        if (AttivaCollisioniConMura && active)
        {
            var meshFilter = collision.gameObject.GetComponent<MeshFilter>();

            if (meshFilter && GB.CompareORNames(meshFilter.sharedMesh.name,
                "barrierWall",
                "roadCornerSmallWall",
                "roadCornerSmallBorder",
                "roadCornerLargeBorder",
                "roadCornerLargerBorderInner",
                "roadCornerLargeWallInner"
            ))
                kartController.SetOnTrack();
        }
    }

    public void SetEnabled(bool setting) =>
        active = setting;

}