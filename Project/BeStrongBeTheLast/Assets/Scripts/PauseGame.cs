using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using UnityEngine.EventSystems;

public class PauseGame : MonoBehaviour
{
    public SceneField startMenu;
    private bool controls;
    public GameObject controlsPanel;
    public GameObject buttons;
    public GameObject AtoConfirm;
    public GameObject BtoBack;
    public GameObject eventSytem;
    public Text title;

    private void Update()
    {
        if(Input.GetButtonDown("P1Pause") || Input.GetButtonDown("P2Pause"))
            Resume();
        if((Input.GetButtonDown("P1Cancel") || Input.GetButtonDown("P2Cancel")) && !controls)
            Resume();
        else if(Input.GetButtonDown("P1Cancel") || Input.GetButtonDown("P2Cancel"))
        {
            SetControls(false);
            StartCoroutine(selectButton());
        }

    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        foreach(var pausable in FindObjectsOfType<PausableMonoBehaviour>())
            pausable.Paused = true;

        StartCoroutine(selectButton());
    }

    public IEnumerator selectButton()
    {
        var es = eventSytem.GetComponent<EventSystem>();
        es.SetSelectedGameObject(null);
        yield return null;
        es.SetSelectedGameObject(es.firstSelectedGameObject);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        foreach(var pausable in FindObjectsOfType<PausableMonoBehaviour>())
            pausable.Paused = false;

        SetControls(false);
        gameObject.SetActive(false);
    }

    public void Controls()
    {
        SetControls(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        GameState.resetGame();
        var audio = GameManager.Instance.GetComponent<AudioSource>();
        audio.clip = GameManager.Instance.menuTrack;
        audio.volume = 1f;
        audio.Play();
        SceneManager.LoadScene(startMenu);
    }

    private void SetControls(bool enabled)
    {
        controls = enabled;
        controlsPanel.SetActive(enabled);
        BtoBack.SetActive(enabled);
        buttons.SetActive(!enabled);
        AtoConfirm.SetActive(!enabled);
        if(enabled)
            title.text = "Controls";
        else
            title.text = "Pause";
    }
}
