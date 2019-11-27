using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Obstacles.Base;
using UnityEngine;

public class KartCollision : aCollisionManager
{
    private KartController thisKartController;
    // Start is called before the first frame update
    void Start()
    {
        thisKartController = transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<KartController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {
            var hitDirection = collider.transform.position - transform.position;

            KartController slow, fast;
            if (thisKartController.currentSplineDistance() <= kartController.currentSplineDistance())
            {
                fast = thisKartController;
                slow = kartController;
                float speedDifference = Math.Abs(fast.currentSpeed - slow.currentSpeed);
                if (speedDifference > 1)
                {
                    Debug.Log("fast " + fast.gameObject.tag + " slow " +  slow.gameObject.tag);
                    Debug.Log("diff " + speedDifference);
                    float forceModifier = (fast.currentSpeed > slow.currentSpeed)?(speedDifference/fast.currentSpeed):(speedDifference/slow.currentSpeed);
                    Debug.Log("mod " + forceModifier);

                    fast.AddForce(200 * forceModifier, ForceMode.Impulse, hitDirection);
                    slow.AddForce(200 * forceModifier, ForceMode.Impulse, -hitDirection);
                    fast.Accelerate(1.1f + 1f * forceModifier);
                    slow.Accelerate(0.9f - 0.5f * forceModifier); 
                }
            }
            else
            {
                slow = transform.parent.GetComponentInChildren<KartController>();
                fast = thisKartController;
            }

            
        }, collider.transform.parent.GetComponentInChildren<KartController>(),"Player", "CPU");
    }
}
