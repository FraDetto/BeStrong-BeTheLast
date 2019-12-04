using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgameScreen : MonoBehaviour
{
    public Text[] results;
    public Button quit, mainMenu;

    private void Start()
    {
        List<GameState.RankObj> finalRankings = GameState.getInstance().getAllRankings();
        if (results.Length == finalRankings.Count)
        {
            for (int i = 0; i < finalRankings.Count; i++)
            {
                results[i].text = finalRankings[i].getTag() + " - " + (i + 1);
            } 
        }
    }
    
    public void quitGame()
    {
        Application.Quit();
    }

    public void resetGame()
    {
        GameState.getInstance().resetGame();
        GB.GotoSceneName("StartMenu");
    }
    
    
}
