using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingsUI : PausableMonoBehaviour
{
    public Text textPrefab;

    private List<Text> textRows;
    // Start is called before the first frame update
    void Start()
    {
        textRows = new List<Text>();
        
        for (int i = 0; i < 8; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            textRows.Add((Text) Instantiate(textPrefab, transform));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<GameState.RankObj> rankings = GameState.Instance.rankings;
        rankings.Reverse();
        for (int i = 0; i < rankings.Count; i++)
        {
            textRows[i].text = (rankings.Count - i) + " - " + rankings[i].getTag();
        }
    }
}
