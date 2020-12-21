/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class EndManager : PausableMonoBehaviour
{

    public GameObject[] cars;
    public Transform CPUSplineRoot;

    public SceneField scena;

    public bool continueRaceWithoutPlayers = false;
    public bool corControl;

    //public GameObject portalPrefab;
    internal bool teleporterSpawned = false;
    public int lapNumber = 3;


    private void Start()
    {
        GameState.Instance.lapsNumberSetting = lapNumber;
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
            if(GameState.Instance.laps.ContainsKey(go.name))
            {
                Debug.Log(GameState.Instance.laps[go.name]);
                GameState.Instance.laps[go.name]++;
                Debug.Log(GameState.Instance.laps[go.name]);
            }
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
        foreach (var lap in GameState.Instance.laps)
            if ((!GameState.Instance.kartTypes[lap.Key].Equals("CPU") || continueRaceWithoutPlayers) && lap.Value <= GameState.Instance.lapsNumberSetting)
                return false;

        return true;
    }
    /*
    private TeleporterPortal SpawnPortal(Transform transform)
    {
        var portal = Instantiate(portalPrefab, transform.position, transform.rotation);
        var portalScript = portal.GetComponent<TeleporterPortal>();

        portalScript.endScriptCallback = this;
        teleporterSpawned = true;
        corControl = true;
        return portalScript;
    }

    IEnumerator TimeForPortal(TeleporterPortal portal)
    {
        yield return new WaitForSeconds(5);
        if (corControl)
        {
            corControl = false;
            teleporterSpawned = false;
            Destroy(portal.gameObject);
        }

    }

    public void TeleportCar(GameObject car)
    {
        var controller = car.GetComponentInChildren<KartController>();
        var sphere = controller.sphere;

        int lap, splineIndex, splineScore, splineTot = CPUSplineRoot.childCount;

        splineScore = GameState.Instance.averageSplineScore;
        lap = Mathf.FloorToInt(splineScore / (float)splineTot);
        splineIndex = Mathf.Max(0, (splineScore - lap * splineTot) % splineTot);

        // Debug.Log(splineIndex + "/" + splineTot);

        var splineTransform = CPUSplineRoot.GetChild(splineIndex).transform;

        if (splineIndex >= controller.CurrentSplineObject.transform.GetSiblingIndex())
            GameState.Instance.laps[controller.transform.root.gameObject.name]--;

        // Miky: COSA FA?
        sphere.transform.position = splineTransform.position;
        controller.rotateToSpline = true;
    }
    */

    private void FixedUpdate()
    {
        if (GameState.Instance.activateRubberBanding)
        {
            foreach (var car in cars)
            {
                var kartController = car.GetComponentInChildren<KartController>();
                GameState.Instance.CalcolaScore(CPUSplineRoot.childCount, car.name);

                if (!GameState.Instance.scoreBiasÇounter.ContainsKey(car.name))
                {
                    GameState.Instance.scoreBiasÇounter.Add(car.name, 0);
                }
                else
                {
                    if (GameState.Instance.getScoreBiasBonus(car.name) == 1)
                        GameState.Instance.scoreBiasÇounter[car.name]++;
                    else
                        GameState.Instance.scoreBiasÇounter[car.name] = 0;

                    if (GameState.Instance.scoreBiasÇounter[car.name] > 700) //  era 600
                        if (!teleporterSpawned)
                        {
                            GameState.Instance.scoreBiasÇounter[car.name] = 0;
                            int currentSplineIndex = kartController.CurrentSplineObject.transform.GetSiblingIndex();
                            int splineIndex = (currentSplineIndex + 2) % CPUSplineRoot.childCount;
                            var splineTransform = CPUSplineRoot.GetChild(splineIndex).transform;
                            
                            int splineIndex2 = (currentSplineIndex + 3) % CPUSplineRoot.childCount;
                            //SplineObject splineObj = CPUSplineRoot.GetChild(splineIndex2).GetComponent<SplineObject>();


                            //TeleporterPortal portal = SpawnPortal(splineTransform);



                            //StartCoroutine(TimeForPortal(portal));


                                //********fai partire metodo con tempo per farlo scomparire se nessuno lo prende
                                //********cancella le due righe sotto quindi non passare piu per la collisione con la spline successiva per la distruzione
                                //********metti un bool che si attiva se il kart prende il portale altrimenti dopoo tot secondi scompare
                            //portal.assignedSpline = splineObj;
                            //splineObj.ClosePortal(portal, car);
                        }
                }
            }

            var CPUsAhead = new List<KartController>();
            var CPUsBehind = new List<KartController>();
            var Players = new List<KartController>();

            int numPlayers, numPlayersTemp = 0;

            foreach (var car in GameState.Instance.rankings)
            {
                var tempKartController = GameState.Instance.kartControllers[car.getTag()];

                if (tempKartController.KCType == aKartController.eKCType.Human)
                    Players.Add(tempKartController);
            }

            numPlayers = Players.Count;

            if (numPlayers > 0)
            {
                foreach(var car in GameState.Instance.rankings)
                {
                    var tempKartController = GameState.Instance.kartControllers[car.getTag()];

                    if(numPlayersTemp == 0 && tempKartController.KCType == aKartController.eKCType.CPU)
                        CPUsAhead.Add(tempKartController);
                    else if(numPlayersTemp >= numPlayers && tempKartController.KCType == aKartController.eKCType.CPU)
                        CPUsBehind.Add(tempKartController);
                    else if(tempKartController.KCType == aKartController.eKCType.Human)
                        numPlayersTemp++;
                }
            }
        }
    }

}