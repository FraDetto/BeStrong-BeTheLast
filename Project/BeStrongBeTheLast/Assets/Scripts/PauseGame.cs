using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class PauseGame : MonoBehaviour
{
    public SceneField startMenu;

    private void Update()
    {
        if(Input.GetButtonDown("P1Pause") || Input.GetButtonDown("P1Cancel") || Input.GetButtonDown("P2Pause") || Input.GetButtonDown("P2Cancel"))
            Resume();
    }

    private void OnEnable()
    {
        foreach(var pausable in FindObjectsOfType<PausableMonoBehaviour>())
            pausable.Paused = true;
    }

    public void Resume()
    {
        foreach(var pausable in FindObjectsOfType<PausableMonoBehaviour>())
            pausable.Paused = false;

        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(startMenu);
    }
}
