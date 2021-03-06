﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LapManager : PausableMonoBehaviour
{

    [Range(0, 9)]
    public int countDownDuration;

    public Text lapText, posText, startText, endText;
    public GameObject player, pausePanel, endPanel;
    private int countdown;
    private Color defaultColor, bronze, silver, gold;

    public AudioSource count;


    private void Start()
    {
        var loading = GameObject.Find("Loading");
        if(!loading)
            Start_();
    }

    public void Start_()
    {
        countdown = countDownDuration;
        defaultColor = new Color(98f / 255f, 98f / 255f, 210f / 255f);
        bronze = new Color(205f/255f, 127f/255f, 50f/255f);
        silver = new Color(192f/255f, 192f/255f, 192f/255f);
        gold = new Color(218f/255f, 165f/255f, 32f/255f);
        startLoop();

        if (count.isActiveAndEnabled)
            count.Play();
    }

    private void startLoop()
    {
        if (countdown > 0 || (countdown == 0 && countDownDuration == 0))
        {
            startText.text = "Ready in " + countdown + "...";
            countdown--;

            StartCoroutine(FadeTextToZeroAlpha((countDownDuration == 0) ? 0.01f : 1f, startText));
        }
        else if (countdown == 0 || (countdown == -1 && countDownDuration == 0))
        {
            startText.text = "GO!!!";
            startText.color = new Color(startText.color.r, startText.color.g, startText.color.b, 1);
            countdown--;

            StartCoroutine(FadeObjectToZeroAlpha((countDownDuration == 0) ? 0.01f : 1f, pausePanel.GetComponent<CanvasGroup>()));
        }
        else
        {
            foreach (var kartController in FindObjectsOfType<KartController>())
            {
                kartController.Paused = false;
                kartController.Accelerate(2f, 2f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (player)
        {
            if (GameState.Instance.laps.ContainsKey(player.name))
                lapText.text = "Lap " + Mathf.Max(1, GameState.Instance.laps[player.name]) + "/" + GameState.Instance.lapsNumberSetting;

            if (GameState.Instance.positions.ContainsKey(player.name))
            {
                //posText.text = GameState.Instance.getCurrentRanking(player.name) + "/" + GameState.Instance.kartControllers.Count;
                switch (GameState.Instance.getCurrentRanking(player.name))
                {
                    case 1:
                        posText.text = "1st";
                        posText.color = Color.red;
                        break;
                    case 2:
                        posText.text = "2nd";
                        posText.color = defaultColor;
                        break;
                    case 3:
                        posText.text = "3rd";
                        posText.color = defaultColor;
                        break;
                    case 6:
                        posText.text = "6th";
                        posText.color = bronze;
                        break;
                    case 7:
                        posText.text = "7th";
                        posText.color = silver;
                        break;
                    case 8:
                        posText.text = "8th";
                        posText.color = gold;
                        break;
                    default:
                        posText.text = GameState.Instance.getCurrentRanking(player.name) + "th";
                        posText.color = defaultColor;
                        break;
                }
            }

            if (!(GameState.Instance.laps[player.name] <= GameState.Instance.lapsNumberSetting))
            {
                endPanel.SetActive(true);

                int rank = GameState.Instance.finalRankings.IndexOf(player.name) + 1;
                string numterm = "th";

                if (rank == 1)
                    numterm = "st";
                else if (rank == 2)
                    numterm = "nd";
                else if (rank == 3)
                    numterm = "rd";

                endText.text = "FINISH! You placed " + rank + numterm;
            }
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        startLoop();
    }

    public IEnumerator FadeObjectToZeroAlpha(float t, CanvasGroup i)
    {
        startLoop();
        if (i)
        {
            i.alpha = 1;
            while (i.alpha > 0.0f)
            {
                i.alpha = i.alpha - (Time.deltaTime / t);
                yield return null;
            }
        }
    }

}