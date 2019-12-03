/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{

    public Text lapText, posText;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        var controller = other.gameObject.transform.root.GetComponentInChildren<KartController>();
        if (go.CompareTag("Player"))
        {
            if (GameState.getInstance().getLaps().ContainsKey(go.tag))
                GameState.getInstance().getLaps()[go.tag]++;
            else
                GameState.getInstance().getLaps().Add(go.tag, 0);
        }
        else
        {
            if (go.CompareTag("CPU"))
            {
                if (GameState.getInstance().getLaps().ContainsKey(go.name))
                    GameState.getInstance().getLaps()[go.name]++;
                else
                    GameState.getInstance().getLaps().Add(go.name, 0);
            }
        }
        if (go.CompareTag("Player"))
        {
            if (!GameState.getInstance().getKartControllers().ContainsKey(go.tag))
                GameState.getInstance().getKartControllers().Add(go.tag, controller);
        }
        else
        {
            if (go.CompareTag("CPU"))
            {
                if (!GameState.getInstance().getKartControllers().ContainsKey(go.name))
                    GameState.getInstance().getKartControllers().Add(go.name, controller);
            }
        }

        if (GameState.getInstance().getLaps().ContainsKey("Player") && GameState.getInstance().getLaps()["Player"] == GameState.getInstance().getLapsNumberSetting())
        {
            //END THE GAME HERE
        }
    }

    private void FixedUpdate()
    {
        if (GameState.getInstance().getLaps().ContainsKey("Player"))
        {
            lapText.text = "Lap " + GameState.getInstance().getLaps()["Player"] + "/" + GameState.getInstance().getLapsNumberSetting();
        }
        if (GameState.getInstance().getPositions().ContainsKey("Player"))
        {
            int[] rank = GameState.getInstance().getCurrentRanking("Player");
            posText.text = rank[0] + "/" + rank[1];
        }
    }
}