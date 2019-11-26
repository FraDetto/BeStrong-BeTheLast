using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var shortcutMovement = transform.parent.GetComponent<ShortcutMovement>();
            shortcutMovement.forceChangeState();
        }
    }
}
