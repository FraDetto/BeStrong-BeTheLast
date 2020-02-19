using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class MainMenu : MonoBehaviour
{
    public SceneField kartSelection;
    public SceneField controls;

    public void PlayGame()
    {
        SceneManager.LoadScene(kartSelection);
    }

    public void Controls()
    {
        SceneManager.LoadScene(controls);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
