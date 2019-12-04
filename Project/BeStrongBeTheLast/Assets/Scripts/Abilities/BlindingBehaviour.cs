/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindingBehaviour : aAbilitiesBehaviour
{

    public float lengthTimeInSeconds = 10f;

    private GameObject user;

    private GameObject[] players, bots;


    void Start()
    {
        user = transform.parent.gameObject;
        players = GameObject.FindGameObjectsWithTag("Player");
        bots = GameObject.FindGameObjectsWithTag("CPU");

        foreach (GameObject player in players)
            if (player != user)
            {
                var ac = player.GetComponent<KartController>();

                if (ac != null)
                {
                    ac.debuff.Find("Blinded").gameObject.SetActive(true);

                    ac.blindingFront.enabled = true;
                    ac.blindingRear.enabled = true;
                }
            }

        MakeBotsEyes(true);

        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lengthTimeInSeconds);

        foreach (GameObject player in players)
            if (player != null && player != user)
            {
                var ac = player.GetComponent<KartController>();

                if (ac != null)
                {
                    ac.debuff.Find("Blinded").gameObject.SetActive(false);

                    ac.blindingFront.enabled = false;
                    ac.blindingRear.enabled = false;
                }
            }

        MakeBotsEyes(false);

        if (enabled)
            Destroy(gameObject);
    }

    private void MakeBotsEyes(bool blinded)
    {
        foreach (var bot in bots)
            if (bot != null)
                if (blinded)
                {
                    //TO DO: Make them drive bad
                }
                else
                {
                    //TO DO: Make their drive return to normal
                }
    }


}