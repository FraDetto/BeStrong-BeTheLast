/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using UnityEngine;

namespace Assets.Scripts.Obstacles.Base
{
    public abstract class aCollisionManager : PausableMonoBehaviour
    {

        protected bool onCollisionWithPlayer(Component component, Action<KartController> callback) =>
            onCollisionWithKartController(component, callback, "Player");

        protected bool onCollisionWithCPU(Component component, Action<KartController> callback) =>
            onCollisionWithKartController(component, callback, "CPU");

        protected bool onCollisionWithPlayer_or_CPU(Component component, Action<KartController> callback) =>
            onCollisionWithKartController(component, callback, "Player", "CPU");

        private bool onCollisionWithKartController(Component component, Action<KartController> callback, params string[] tags)
        {
            var collidedWithTags = GB.CompareORTags(component, tags);

            if (collidedWithTags)
            {
                var kartController = component.transform.root.GetComponentInChildren<KartController>();

                if (kartController)
                    callback(kartController);
                else
                    throw new Exception("Questo componente non ha il kartController!");
            }

            return collidedWithTags;
        }

    }
}