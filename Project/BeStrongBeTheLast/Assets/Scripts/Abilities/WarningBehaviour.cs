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
            //Si lo so che fa cagare, si potrebbe sistemare con una bella classe astratta "Abilità" con il campo user, ma la pipeline di chiamate è così complessa ora che non mi sogno neanche di metterci mani.
            //Be my guest ;-)
            if((other.transform.parent.name.StartsWith("Trishot") && !other.GetComponent<SingleShotBehaviour>().trishotBehaviour.user.Equals(transform.root.gameObject)) 
                || (other.name.StartsWith("Homing") && !other.GetComponent<HomingBehaviour>().user.Equals(transform.root.gameObject))
                || (other.name.StartsWith("Bouncing") && !other.GetComponent<BouncingBehaviour>().user.Equals(transform.root.gameObject)))
            {
                if(!blinking)
                {
                    blinking = true;
                    warningText.enabled = true;
                    StartCoroutine(Blink());
                }
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
