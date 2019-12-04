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

internal static class GameState
{
    internal static GameStateInternal Instance = new GameStateInternal();

    internal static GameStateInternal getInstance()
    {
        return Instance;
    }

    internal static void resetGame()
    {
        Instance = new GameStateInternal();
    }


    internal class GameStateInternal
    {

        //CURRENT RACE OPTIONS
        private string playerChampName, selectedTrackName;
        private int lapsNumberSetting = 3;

        //qui si potrebbe unificare tutto dentro ad una List<RankObj>
        private Dictionary<string, int> positions = new Dictionary<string, int>();
        private Dictionary<string, ushort> laps = new Dictionary<string, ushort>();
        private Dictionary<string, KartController> kartControllers = new Dictionary<string, KartController>();
        List<RankObj> rankings = new List<RankObj>();


        public bool setPlayerChamp(string name)
        {
            playerChampName = name;
            return true;
            //Add check to see if prefab available, if not return false
        }

        public string getPlayerChamp()
        {
            return playerChampName;
        }

        public bool setCurrentTrack(string name)
        {
            selectedTrackName = name;
            return true;
            //Add check to see if prefab available, if not return false
        }

        public string getCurrentTrack()
        {
            return selectedTrackName;
        }

        public int getLapsNumberSetting()
        {
            return lapsNumberSetting;
        }

        public Dictionary<string, int> getPositions()
        {
            return positions;
        }

        public Dictionary<string, ushort> getLaps()
        {
            return laps;
        }

        public Dictionary<string, KartController> getKartControllers()
        {
            return kartControllers;
        }

        public int[] getCurrentRanking(string tag)
        {
            int[] ranks = new int[2];
            //ranks[1] = positions.Count;
            ranks[1] = 8;
            rankings = new List<RankObj>();

            foreach (var kartScore in positions)
            {
                rankings.Add(new RankObj(kartScore.Value, kartScore.Key, kartControllers[kartScore.Key].getCurrentSplineDistance()));
            }

            rankings = rankings.OrderBy(c => c.getScore()).ThenByDescending(c => c.getDist()).ToList();

            foreach (var r in rankings)
            {
                if (r.getTag().Equals(tag))
                {
                    ranks[0] = rankings.IndexOf(r) + 1;
                    break;
                }
            }
            return ranks;
        }

        public List<RankObj> getAllRankings()
        {
            return rankings;
        }

    }

    internal class RankObj
    {
        private int score;
        private string tag;
        private float dist;

        public RankObj(int score, string tag, float dist)
        {
            this.score = score;
            this.tag = tag;
            this.dist = dist;
        }

        public int getScore()
        {
            return score;
        }
        public string getTag()
        {
            return tag;
        }
        public float getDist()
        {
            return dist;
        }
    }

}