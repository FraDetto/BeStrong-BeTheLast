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
        Debug.Log(collider);
        onCollisionWithTags(collider, (kartController) =>
        {
            var hitDirection = collider.transform.position - transform.position;

            KartController slow, fast;
            if (thisKartController.currentSplineDistance() <= kartController.currentSplineDistance())
            {
                fast = thisKartController;
                slow = kartController;
                Debug.Log("fast " + fast.gameObject.tag + " slow " +  slow.gameObject.tag);
                if((fast.currentSpeed - slow.currentSpeed) > 5)
                fast.AddForce(200 * (fast.currentSpeed - slow.currentSpeed)/5, ForceMode.Impulse, hitDirection);
                slow.AddForce(200 * (fast.currentSpeed - slow.currentSpeed)/5, ForceMode.Impulse, -hitDirection);
                fast.Accelerate(1.08f);
                slow.Accelerate(0.8f);
            }
            else
            {
                slow = transform.parent.GetComponentInChildren<KartController>();
                fast = thisKartController;
            }

            
        }, collider.transform.parent.GetComponentInChildren<KartController>(),"Player", "CPU");
    }
}
