using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ArrowPanelOnSpline : MonoBehaviour
{
    public Image container;
    public Sprite imageToShow;

    private void OnTriggerEnter(Collider co)
    {
        if (container!=null)
        {
            if (co.CompareTag("Player"))
            {
                StartCoroutine(compareImage());
            }
        }            
    }

    IEnumerator compareImage() {

        container.sprite = imageToShow;
        container.enabled = true;
        yield return new WaitForSeconds(2);

        container.enabled = false;
    }

}
