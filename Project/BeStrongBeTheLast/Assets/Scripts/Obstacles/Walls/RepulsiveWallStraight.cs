using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsiveWallStraight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
        kartController.AddForce(2000f, ForceMode.Impulse, -transform.forward +  transform.right);
        kartController.Accelerate(2f);
        //use WRONGWAY code here when ready
    }
}
