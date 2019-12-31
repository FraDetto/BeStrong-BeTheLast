/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class ChangeFloor : PausableMonoBehaviour
{

    public int camToActive, camToDis;
    public float gravityMultiplier = 1f;
    private bool doppiaCameraInScena;


    private void Start() =>
        doppiaCameraInScena = camToActive != 0 && camToDis != 0;

    private void OnTriggerEnter(Collider co)
    {
        if (doppiaCameraInScena)
            if (co.CompareTag("Player") || co.CompareTag("CPU"))
            {
                var kart = co.transform.root.GetComponentInChildren<KartController>();

                //è stato fatto al rigo 331 di aKartController
                //kart.gravityMultiplier = Mathf.Clamp(gravityMultiplier, 0.5f, 2f);

                if (kart.playerType.Equals(aKartController.eKCType.Human))
                    kart.ActivNewCamera(camToActive, camToDis);

                //camToActive.SetActive(true);
                //camToDis.SetActive(false);   
            }
    }

}