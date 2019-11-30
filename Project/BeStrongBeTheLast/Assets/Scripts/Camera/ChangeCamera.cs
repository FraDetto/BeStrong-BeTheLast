/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{

    public GameObject vCam1, vCam2, vCam3, vCam4;

    private bool doppiaCameraInScena;

    private void Start()
    {
        doppiaCameraInScena = vCam1 != null && vCam2 != null && vCam3 != null && vCam4 != null;
    }

    private void OnTriggerEnter(Collider co)
    {
        if (doppiaCameraInScena)
        {
            Debug.Log("Colliso fuori");

            if (co.CompareTag("Player"))
            {
                Debug.Log("Colliso player");
                if (name.Equals("Gate1"))
                {
                    //var camActive = vCam1.active;
                    vCam1.SetActive(false);
                    vCam2.SetActive(true);
                }else if (name.Equals("Gate2"))
                {
                    vCam2.SetActive(false);
                    vCam1.SetActive(true);
                }
                else if (name.Equals("Gate3"))
                {
                    vCam1.SetActive(false);
                    vCam3.SetActive(true);
                }
                else if (name.Equals("Gate4"))
                {
                    vCam3.SetActive(false);
                    vCam4.SetActive(true);
                }
                else
                {
                    vCam4.SetActive(false);
                    vCam1.SetActive(true);
                }
                
            }
        }
    }

}