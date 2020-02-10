using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class TrackSelection : MonoBehaviour
{
    public SceneField kartSelection;
    public SceneField futureTrack;
    public SceneField natureTrack;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("P1Cancel"))
            SceneManager.LoadScene(kartSelection);
    }

    public void FutureTrack()
    {
        SceneManager.LoadScene(futureTrack);
    }

    public void NatureTrack()
    {
        SceneManager.LoadScene(natureTrack);
    }
}
