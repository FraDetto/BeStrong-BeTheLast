using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingRay : MonoBehaviour
{
    public float slowingAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
        { 
            var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
            kartController.Accelerate(slowingAmount);
        }
    }
}
