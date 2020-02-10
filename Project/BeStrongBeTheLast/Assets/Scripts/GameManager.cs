using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    internal bool player2added = false;
    internal string player1choice;
    internal string player2choice;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
