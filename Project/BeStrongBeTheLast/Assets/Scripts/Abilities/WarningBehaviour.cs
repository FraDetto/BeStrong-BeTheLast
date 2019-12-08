using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningBehaviour : MonoBehaviour
{
    public Text warningText;

    private bool blinking;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Projectile"))
        {
            if(!blinking)
            {
                blinking = true;
                warningText.enabled = true;
                StartCoroutine(Blink());
            }
        }
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.5f);
        warningText.enabled = false;
        blinking = false;
    }
}
