using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class MainMenu : MonoBehaviour
{
    public SceneField kartSelection;

    public void PlayGame()
    {
        SceneManager.LoadScene(kartSelection);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
