﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Concurrent;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    internal ConcurrentDictionary<string, GameObject> detectedObstacles = new ConcurrentDictionary<string, GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacles") && !detectedObstacles.ContainsKey(other.gameObject.name))
            detectedObstacles.TryAdd(other.gameObject.name, other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacles") && detectedObstacles.ContainsKey(other.gameObject.name))
            detectedObstacles.TryRemove(other.gameObject.name, out GameObject j);
    }

}