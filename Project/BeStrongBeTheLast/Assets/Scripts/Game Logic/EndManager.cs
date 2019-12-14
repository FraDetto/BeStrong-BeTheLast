/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Boo.Lang;
using UnityEngine;
using Utilities;

public class EndManager : PausableMonoBehaviour
{

    public GameObject[] cars;
    public Transform CPUSplineRoot;
    public SceneField scena;

    private void Start()
    {
        foreach (var car in cars)
        {
            var Kart = car.transform.GetChild(0);
            var kc = Kart.GetComponent<KartController>();

            GameState.Instance.kartControllers.Add(car.name, kc);
            GameState.Instance.laps.Add(car.name, 0);
            GameState.Instance.positions.Add(car.name, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        var controller = other.gameObject.transform.root.GetComponentInChildren<KartController>();

        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            if (GameState.Instance.laps.ContainsKey(go.name))
                GameState.Instance.laps[go.name]++;
            else
                GameState.Instance.laps.Add(go.name, 0);

            if (GameState.Instance.laps[go.name] > GameState.Instance.lapsNumberSetting)
            {
                GameState.Instance.finalRankings.Add(go.name);
                //controller.Paused = true;
                //other.enabled = false;
                go.SetActive(false);
            }
        }

        if (endGame())
        {
            List<string> finalCPUs = new List<string>();
            List<GameState.RankObj> objToRemove = new List<GameState.RankObj>();
            foreach (var lap in GameState.Instance.laps)
                if (GameState.Instance.kartTypes[lap.Key].Equals("CPU") &&
                    lap.Value <= GameState.Instance.lapsNumberSetting)
                {
                    finalCPUs.Add(lap.Key);
                }
            foreach (var rank in GameState.Instance.rankings)
            {
                if (!isInList(rank.getTag(), finalCPUs))
                {
                    objToRemove.Add(rank);
                }
            }
            foreach (var rank in objToRemove)
            {
                GameState.Instance.rankings.Remove(rank);
            }
            foreach (var rank in GameState.Instance.rankings)
            {
                GameState.Instance.finalRankings.Add(rank.getTag());
            }
            
            GB.GotoSceneName(scena.SceneName);
        }
    }

    private bool isInList(string tag, List<string> list)
    {
        foreach (var name in list)
        {
            if (tag.Equals(name))
                return true;
        }

        return false;
    }

    private bool endGame()
    {
        foreach (var lap in GameState.Instance.laps)
            if (!GameState.Instance.kartTypes[lap.Key].Equals("CPU") && lap.Value <= GameState.Instance.lapsNumberSetting)
                return false;

        return true;
    }

    private void FixedUpdate()
    {
        foreach (var car in cars)
            GameState.Instance.CalcolaScore(CPUSplineRoot.childCount, car.name);
    }

}