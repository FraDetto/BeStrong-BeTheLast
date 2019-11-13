using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsiveWallStraight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
        { 
            var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
            kartController.Accelerate(2f);
        }
    }
}
