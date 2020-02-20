using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class CreditsMenu : MonoBehaviour
{
    public SceneField startMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("P1Cancel"))
            SceneManager.LoadScene(startMenu);
    }
}
