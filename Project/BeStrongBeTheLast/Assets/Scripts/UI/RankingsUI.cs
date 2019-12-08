using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingsUI : MonoBehaviour
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
        for (int i = 0; i < 8; i++)
        {
            textRows[i].text = (i + 1).ToString() + " - " + rankings[i].getTag();
        }
    }
}
