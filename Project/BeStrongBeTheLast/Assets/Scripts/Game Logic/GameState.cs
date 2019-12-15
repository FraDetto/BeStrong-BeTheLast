/*
MIT License
Copyright (c) 2019 Team Lama: Carrarini Andrea, Cerrato Loris, De Cosmo Andrea, Maione Michele
Author: Frasson Jacopo
Contributors: 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class GameState
{
    internal static GameStateInternal Instance = new GameStateInternal();

    internal static void resetGame() =>
        Instance = new GameStateInternal();


    internal class GameStateInternal
    {
        //CURRENT RACE OPTIONS
        internal string playerChampName, selectedTrackName;
        internal int lapsNumberSetting = 3;
        internal int scoreBiasDeadZone = 5;
        internal float maxScoreBias = 1;
        internal bool activateRubberBanding = true;

        internal Dictionary<string, int> positions = new Dictionary<string, int>();
        internal Dictionary<string, ushort> laps = new Dictionary<string, ushort>();
        internal Dictionary<string, KartController> kartControllers = new Dictionary<string, KartController>();
        internal Dictionary<string, string> kartTypes = new Dictionary<string, string>();

        internal List<RankObj> rankings = new List<RankObj>();
        internal Dictionary<string, RankObj> rankingsDict = new Dictionary<string, RankObj>();
        internal List<string> finalRankings = new List<string>();


        public int getCurrentRanking(string tag)
        {
            var init = rankings.Count == 0;

            foreach (var kartScore in positions)
            {
                RankObj ro;

                if (init)
                {
                    ro = new RankObj();

                    rankings.Add(ro);
                    rankingsDict.Add(kartScore.Key, ro);
                }
                else
                {
                    ro = rankingsDict[kartScore.Key];
                }

                ro.set(kartScore.Value, kartScore.Key, kartControllers[kartScore.Key].getCurrentSplineDistance());
            }

            rankings = rankings.OrderByDescending(c => c.getScore()).ThenBy(c => c.getDist()).ToList();

            foreach (var r in rankings)
                if (r.getTag().Equals(tag))
                    return rankings.IndexOf(r) + 1;

            return kartControllers.Count;
        }

        internal void CalcolaScore(int NumeroSplines, string tag)
        {
            var lap = 0;

            if (laps.ContainsKey(tag))
                lap = laps[tag];

            var kc = kartControllers[tag];
            var splineIndex = kc.CurrentSplineObject.transform.GetSiblingIndex();

            var score = NumeroSplines * lap + splineIndex;

            if (tag != null)
            {
                if (positions.ContainsKey(tag))
                    positions[tag] = score;
                else
                    positions.Add(tag, score);
            }
        }

        public float getScoreBiasBonus(string tag)
        {
            if (activateRubberBanding)
            {
                float scoreSum = 0, avgScore = 0, maxScore = 0, minScore = 0, betterPlayers = 0;
                int countBetterPlayers = 0;
                float myScore = positions[tag];

                foreach (var score in positions)
                    if (!score.Key.Equals(tag) && (score.Value + scoreBiasDeadZone < myScore))
                    {
                        countBetterPlayers++;
                        scoreSum += score.Value;

                        if (score.Value > maxScore)
                            maxScore = score.Value;

                        if (score.Value < minScore || minScore < 1)
                            minScore = score.Value;
                    }

                avgScore = scoreSum / positions.Count;
                betterPlayers = countBetterPlayers / (float)(positions.Count - 1);

                var scoreBias = 0.05f * Mathf.Max(Mathf.Min(myScore - scoreBiasDeadZone - maxScore, 20), 0) +
                             0.03f * Mathf.Max(Mathf.Min(myScore - scoreBiasDeadZone - avgScore, 20), 0) +
                             0.01f * Mathf.Max(Mathf.Min(myScore - scoreBiasDeadZone - minScore, 20), 0) *
                             betterPlayers;

                scoreBias = Mathf.Min(maxScoreBias, scoreBias);

                return scoreBias;
            }
            else
            {
                return 0;
            }
        }
    }

    internal class RankObj
    {
        private int score;
        private string tag;
        private float dist;

        public void set(int score, string tag, float dist)
        {
            this.score = score;
            this.tag = tag;
            this.dist = dist;
        }

        public int getScore() =>
            score;

        public string getTag() =>
            tag;

        public float getDist() =>
            dist;
    }

}