/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Francesco Dettori
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArrowPanelOnSpline : MonoBehaviour
{
    public Sprite imageToShow;

    Image container;
    Dictionary<GameObject, Transform> playerUIArrows = new Dictionary<GameObject, Transform>();

    private void Start()
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name.Equals("UI"))
                for (int i = 0; i < root.transform.GetChildCount(); i++)
                {
                    var c = root.transform.GetChild(i);

                    if (c.CompareTag("PlayerCanvas") && c.gameObject.active)
                    {
                        var lapManager = c.gameObject.GetComponent<LapManager>();
                        var arrow = GB.FindTransformInChildWithName(c, "Arrow");

                        playerUIArrows.Add(lapManager.player, arrow);
                    }
                }
    }

    private void OnTriggerEnter(Collider co)
    {
        if (co.CompareTag("Player"))
        {
            var player = co.transform.root.gameObject;

            if (playerUIArrows.ContainsKey(player))
            {
                var arrow = playerUIArrows[player];
                container = arrow.GetComponent<Image>();

                container.sprite = imageToShow;
                container.enabled = true;

                StartCoroutine(compareImage());
            }
        }
    }

    IEnumerator compareImage()
    {
        yield return new WaitForSeconds(2);
        container.enabled = false;
    }

}