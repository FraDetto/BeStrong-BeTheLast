using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutInstantClose : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            transform.GetComponent<Collider>().isTrigger = false;
        }
    }
}
