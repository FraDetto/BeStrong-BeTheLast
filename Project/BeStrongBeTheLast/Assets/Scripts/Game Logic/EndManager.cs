﻿/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
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

        if (EndGame())
        {
            var finalCPUs = new HashSet<string>();
            var objToRemove = new List<GameState.RankObj>();

            foreach (var lap in GameState.Instance.laps)
                if (GameState.Instance.kartTypes[lap.Key].Equals("CPU") && lap.Value <= GameState.Instance.lapsNumberSetting)
                    finalCPUs.Add(lap.Key);

            foreach (var rank in GameState.Instance.rankings)
                if (!finalCPUs.Contains(rank.getTag()))
                    objToRemove.Add(rank);

            foreach (var rank in objToRemove)
                GameState.Instance.rankings.Remove(rank);

            foreach (var rank in GameState.Instance.rankings)
                GameState.Instance.finalRankings.Add(rank.getTag());

            GB.GotoSceneName(scena.SceneName);
        }
    }

    private bool EndGame()
    {
        var PlayerCheHannoFinito = 0;
        var CPUCheHannoFinito = 0;
        var Players = 0;
        var CPUs = 0;

        foreach (var lap in GameState.Instance.laps)
        {
            var terminato = lap.Value >= GameState.Instance.lapsNumberSetting;

            switch (GameState.Instance.kartTypes[lap.Key])
            {
                case "Player":
                    Players++;

                    if (terminato)
                        PlayerCheHannoFinito++;

                    break;
                case "CPU":
                    CPUs++;

                    if (terminato)
                        CPUCheHannoFinito++;

                    break;
            }
        }

        if (Players == 0)
        {
            if (CPUCheHannoFinito < CPUs)
                return false;
        }
        else
        {
            if (PlayerCheHannoFinito < Players)
                return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        foreach (var car in cars)
            GameState.Instance.CalcolaScore(CPUSplineRoot.childCount, car.name);
    }

}