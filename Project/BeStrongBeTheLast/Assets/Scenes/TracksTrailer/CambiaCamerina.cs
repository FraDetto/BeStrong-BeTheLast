using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiaCamerina : MonoBehaviour
{
    public GameObject cameraToChangeWithQ;
    public GameObject cameraToChangeWithW;
    public GameObject cameraToChangeWithE;

    void Start()
    {
        cameraToChangeWithQ.SetActive(true);
        cameraToChangeWithW.SetActive(false);
        cameraToChangeWithE.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            cameraToChangeWithQ.SetActive(true);
            cameraToChangeWithW.SetActive(false);
            cameraToChangeWithE.SetActive(false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            cameraToChangeWithQ.SetActive(false);
            cameraToChangeWithW.SetActive(true);
            cameraToChangeWithE.SetActive(false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cameraToChangeWithQ.SetActive(false);
            cameraToChangeWithW.SetActive(false);
            cameraToChangeWithE.SetActive(true);
        }
    }
}
