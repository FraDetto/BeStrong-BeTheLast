/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using UnityEngine;

public class BoxSpawner : PausableMonoBehaviour
{

    public GameObject BoxAccelerating, BoxSlowing;
    public float waitSpawn = 1.3f;
    public bool GreenTrueRedFalse;
    public Vector3 spawnPos2;


    int startColour;

    void Start()
    {
        startColour = Random.Range(0, 10);
        GreenTrueRedFalse = (startColour < 5);
        spawnPos2 = transform.GetChild(0).transform.position;
        StartCoroutine(spawnBox());
    }

    IEnumerator spawnBox()
    {
        GameObject go;

        while (true)
        {
            go = Instantiate(GreenTrueRedFalse ? BoxAccelerating : BoxSlowing, transform.position, Quaternion.identity);
            //go.transform.Rotate(0, -90, 0);

            startColour = Random.Range(0, 10);
            GreenTrueRedFalse = (startColour < 5);

            yield return new WaitForSeconds(waitSpawn);

            go = Instantiate(GreenTrueRedFalse ? BoxAccelerating : BoxSlowing, spawnPos2, Quaternion.identity);
            //go.transform.Rotate(0, -90, 0);

            startColour = Random.Range(0, 10);
            GreenTrueRedFalse = (startColour < 5);

            yield return new WaitForSeconds(waitSpawn);
        }
    }

}