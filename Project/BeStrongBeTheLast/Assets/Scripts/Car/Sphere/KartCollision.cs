using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Obstacles.Base;
using UnityEngine;

public class KartCollision : aCollisionManager
{
    private KartController thisKartController;

    public enum Mode
    {
        left,
        right,
        rear
    }

    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        thisKartController = transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<KartController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        onCollisionWithTags(collider, (kartController) =>
        {
            if (collider.transform.parent.gameObject != transform.parent.transform.parent.transform.parent.transform
                    .parent.transform.parent.gameObject)
            {
                var hitDirection = collider.transform.position - transform.position;
                KartController slow, fast;
                fast = thisKartController;
                slow = kartController;
                float speedDifference = Math.Abs(fast.currentSpeed - slow.currentSpeed);
                float forceModifier = (fast.currentSpeed > slow.currentSpeed)?(speedDifference/fast.currentSpeed):(speedDifference/slow.currentSpeed);
                if (mode == Mode.rear)
                {
                    if (thisKartController.currentSplineDistance() <= kartController.currentSplineDistance())
                    {
                        if (speedDifference > 1)
                        {
                            fast.AddForce(200 * forceModifier, ForceMode.Impulse, hitDirection);
                            slow.AddForce(200 * forceModifier, ForceMode.Impulse, -hitDirection);
                            fast.Accelerate(1.1f + 1f * forceModifier);
                            slow.Accelerate(0.9f - 0.5f * forceModifier); 
                        }
                    }
                }else if (mode == Mode.left || mode == Mode.right)
                {
                    kartController.AddForce(2000 + 1000 * forceModifier, ForceMode.Impulse, hitDirection);
                }
            }
        }, collider.transform.parent.GetComponentInChildren<KartController>(),"Player", "CPU");
    }
}
