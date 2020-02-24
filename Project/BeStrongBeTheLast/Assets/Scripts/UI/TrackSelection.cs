using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class TrackSelection : MonoBehaviour
{
    public SceneField kartSelection;
    public SceneField futureTrack;
    public SceneField natureTrack;

    public GameObject player1choice;
    public GameObject player2choice;

    public GameObject loading;

    public AudioClip futureSong;
    public AudioClip natureSong;

    private void Start()
    {
        player1choice.GetComponent<Text>().text = GameManager.Instance.player1choice;
        if(GameManager.Instance.player2added)
            player2choice.GetComponent<Text>().text = GameManager.Instance.player2choice;
        else
        {
            player2choice.transform.parent.gameObject.SetActive(false);
            //player1choice.transform.parent.Translate(Vector3.right * 300f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("P1Cancel"))
            SceneManager.LoadScene(kartSelection);
    }

    public void FutureTrack()
    {
        loading.SetActive(true);
        loading.GetComponent<LoadingScreen>().songToBeLoaded = futureSong;
        GameManager.Instance.GetComponent<AudioSource>().volume = 0.5f;
        SceneManager.LoadScene(futureTrack);
    }

    public void NatureTrack()
    {
        loading.SetActive(true);
        loading.GetComponent<LoadingScreen>().songToBeLoaded = natureSong;
        GameManager.Instance.GetComponent<AudioSource>().volume = 0.75f;
        SceneManager.LoadScene(natureTrack);
    }
}
