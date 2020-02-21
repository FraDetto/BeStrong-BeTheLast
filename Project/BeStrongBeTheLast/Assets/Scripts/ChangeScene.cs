using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class ChangeScene : MonoBehaviour
{
    public SceneField nextScene;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextScene());
    }

    public IEnumerator NextScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextScene);
    }
}
