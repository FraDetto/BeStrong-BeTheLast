using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{

    public GameObject vCam1, vCam2;

    private void OnTriggerEnter(Collider co)
    {
        Debug.Log("Colliso fuori");
        if (co.CompareTag("Player"))
        {
            Debug.Log("Colliso player");
            if (vCam1.active)
            {
                vCam1.SetActive(false);
                vCam2.SetActive(true);
            }
            else
            {
                vCam1.SetActive(true);
                vCam2.SetActive(false);
            }
            
        }
    }
}
