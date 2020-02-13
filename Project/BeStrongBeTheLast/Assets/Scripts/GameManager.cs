using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool player2added = false;
    public string player1choice;
    public string player2choice;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
