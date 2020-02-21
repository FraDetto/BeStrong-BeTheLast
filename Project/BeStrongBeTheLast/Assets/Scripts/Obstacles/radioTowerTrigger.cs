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

public class radioTowerTrigger : PausableMonoBehaviour
{

    public float acceleration;
    public int respawnElecTime;
    public GameObject[] particlesToDis;

    private void Update()
    {
        if(Paused)
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
        {
            other.transform.root.GetComponentInChildren<KartController>().Accelerate(acceleration);
            //StartCoroutine(disableElectricity());
        }
    }

    IEnumerator disableElectricity()
    {
        gameObject.GetComponent<Collider>().enabled = false;

        foreach (GameObject part in particlesToDis)
            part.SetActive(false);

        yield return new WaitForSeconds(respawnElecTime);

        foreach (GameObject part in particlesToDis)
            part.SetActive(true);

        gameObject.GetComponent<Collider>().enabled = true;
    }

}