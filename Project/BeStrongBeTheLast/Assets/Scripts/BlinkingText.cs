using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    private GameObject button;
    private bool running;
    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetChild(0).gameObject;
        StartCoroutine(Blink());
    }

    private void OnEnable()
    {
        if(!running)
            StartCoroutine(Blink());
    }

    private void OnDisable()
    {
        running = false;
    }

    public IEnumerator Blink()
    {
        running = true;
        while(true)
        {
            yield return new WaitForSeconds(0.375f);
            GetComponent<Text>().enabled = false;
            button.SetActive(false);
            yield return new WaitForSeconds(0.375f);
            GetComponent<Text>().enabled = true;
            button.SetActive(true);
        }
    }
}
