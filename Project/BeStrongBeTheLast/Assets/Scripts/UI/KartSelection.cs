/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Frasson Jacopo, Riccardo Lombardi
Contributors: 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Utilities;

public class KartSelection : PausableMonoBehaviour
{
    public SceneField startMenu;
    public SceneField trackSelection;
    private bool player1selected = false;
    private bool player2selected = false;
    public GameObject player1confirm;
    public GameObject player2confirm;
    public GameObject player2join;
    public GameObject player2wait;
    public GameObject p1kart;
    public GameObject p2kart;
    public GameObject p1controls;
    public GameObject p2controls;

    private Transform player1selection;
    private Transform player2selection;

    private void Start()
    {
        p1kart.GetComponent<Text>().text = "";
        p2kart.GetComponent<Text>().text = "";
    }

    private void Update()
    {
        if(Input.GetButtonDown("P1Cancel") && !player1selected)
        {
            GameManager.Instance.player2added = false;
            SceneManager.LoadScene(startMenu);
        }
        else if(Input.GetButtonDown("P1Cancel") && player1selected)
        {
            player1selected = false;
            p1kart.SetActive(false);
            player1confirm.SetActive(true);
            player2wait.SetActive(true);
            player2confirm.SetActive(false);
            player1selection.GetComponent<Button>().interactable = true;
            p1controls.SetActive(!player1selected);
            p2controls.SetActive(player1selected);
        }

        if(Input.GetButtonDown("P2Special") && !GameManager.Instance.player2added)
        {
            GameManager.Instance.player2added = true;
            player2join.SetActive(false);
            player2wait.SetActive(true);
            p2kart.SetActive(true);
        }
        else if(Input.GetButtonDown("P2Special") && GameManager.Instance.player2added)
        {
            GameManager.Instance.player2added = false;
            player2join.SetActive(true);
            player2wait.SetActive(false);
            p2kart.SetActive(false);
        }

        if(player1selected && (!GameManager.Instance.player2added || player2selected))
        {
            GameManager.Instance.player1choice = p1kart.GetComponent<Text>().text;

            if(player2selected)
                GameManager.Instance.player2choice = p2kart.GetComponent<Text>().text;

            SceneManager.LoadScene(trackSelection);
        }
    }

    public void SelectKart(Transform champ)
    {
        if(!player1selected)
        {
            player1selected = true;
            player1selection = champ;
            p1kart.SetActive(true);
            p1kart.GetComponent<Text>().text = player1selection.GetChild(0).GetComponent<Text>().text;
            player1selection.GetComponent<Button>().interactable = false;
            player1confirm.SetActive(false);
            player2wait.SetActive(false);
            player2confirm.SetActive(true);
            p1controls.SetActive(!player1selected);
            p2controls.SetActive(player1selected);
        }
        else
        {
            player2selected = true;
            player2selection = champ;
            player2confirm.SetActive(false);
            p2kart.SetActive(true);
            p2kart.GetComponent<Text>().text = player2selection.GetChild(0).GetComponent<Text>().text;
            player2selection.GetComponent<Button>().interactable = false;
        }
        
    }

    /*private byte GetNumberOfPlayerByTrackName(string trackName)
    {
        switch (trackName)
        {
            case "Real Futuristic Track":
                return 2;
            default:
                return 1;
        }
    }

    // Fase 1 Track Selection
    public void SelectTrack(string trackName)
    {
        var numP = GetNumberOfPlayerByTrackName(trackName);

        GameState.Instance.selectedTrackName = trackName;
        GameState.Instance.playersChampName = new string[numP];

        //GB.GotoSceneName("KartSelection");
        LoadTrack();
    }

    // Fase 2 Player Selection
    public void AddThisPlayer(string champName, byte numP) =>
        GameState.Instance.playersChampName[numP] = champName;

    // Fase 3 Start game
    public void LoadTrack()
    {
        var track = "Scenes/Testing/Guida Arcade Sphere/" + GameState.Instance.selectedTrackName;
        SceneManager.LoadScene(track, LoadSceneMode.Single);
    } */

}