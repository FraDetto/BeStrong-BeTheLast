/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingsUI : PausableMonoBehaviour
{
    public Text textPrefab;

    private List<Text> textRows = new List<Text>();


    private void Start()
    {
        for (var i = 0; i < 8; i++)
            // Create new instances of our prefab until we've created as many as we specified
            textRows.Add(Instantiate(textPrefab, transform));
    }

    private void FixedUpdate()
    {
        var rankings = GameState.Instance.rankings;
        rankings.Reverse();

        for (var i = 0; i < rankings.Count; i++)
        {
            textRows[i].text = $"{rankings.Count - i} - {rankings[i].getTag()}";
            textRows[i].color = Color.white;

            if(textRows[i].text.Contains(GameManager.Instance.player1choice))
                textRows[i].color = Color.green;

            if(GameManager.Instance.player2added && textRows[i].text.Contains(GameManager.Instance.player2choice))
                textRows[i].color = Color.yellow;
        }
            
    }

}
