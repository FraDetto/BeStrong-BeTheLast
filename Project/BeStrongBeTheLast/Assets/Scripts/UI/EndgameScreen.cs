/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors: Riccardo Lombardi
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utilities;

public class EndgameScreen : PausableMonoBehaviour
{
    public Text[] results;
    public Button mainMenu;
    public SceneField startMenu;

    private void Start()
    {
        var i = -1;
        var finalRankings = GameState.Instance.finalRankings;

        finalRankings.Reverse();

        if (results.Length == finalRankings.Count)
            foreach (var finalRank in finalRankings)
            {
                if(finalRank.Contains(GameManager.Instance.player1choice))
                {
                    i += 1;
                    results[i].color = Color.green;
                    results[i].text = (finalRankings.Count - i) + " - " + finalRank;
                }
                else if(GameManager.Instance.player2added && finalRank.Contains(GameManager.Instance.player2choice))
                {
                    i += 1;
                    results[i].color = Color.yellow;
                    results[i].text = (finalRankings.Count - i) + " - " + finalRank;
                }
                else   
                    results[i += 1].text = (finalRankings.Count - i) + " - " + finalRank;


            }
    }

    public void resetGame()
    {
        GameState.resetGame();
        var audio = GameManager.Instance.GetComponent<AudioSource>();
        audio.clip = GameManager.Instance.menuTrack;
        audio.volume = 1f;
        audio.Play();
        SceneManager.LoadScene(startMenu);
    }


}
