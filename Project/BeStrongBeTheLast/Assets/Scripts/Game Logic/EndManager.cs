/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using Utilities;

public class EndManager : MonoBehaviour
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
        }

        if (GameState.Instance.laps.ContainsKey("Player") && endGame())
            GB.GotoSceneName(scena.SceneName);
    }

    private bool endGame()
    {
        foreach (var lap in GameState.Instance.laps)
            if (lap.Value <= GameState.Instance.lapsNumberSetting)
                return false;

        return true;
    }

    private void FixedUpdate()
    {
        foreach (var car in cars)
            GameState.Instance.CalcolaScore(CPUSplineRoot.childCount, car.name);
    }

}