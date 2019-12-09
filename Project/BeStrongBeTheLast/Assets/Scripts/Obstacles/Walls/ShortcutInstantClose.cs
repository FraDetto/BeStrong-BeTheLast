using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutInstantClose : MonoBehaviour
{
    public GameObject wall;
    private ShortcutMovement shortcutMovement;

    private void Start()
    {
        shortcutMovement = wall.GetComponent<ShortcutMovement>();
    }

    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            InstantClose(20);
        }
    }

    private void InstantClose(int timeout)
    {
        transform.GetComponent<Collider>().isTrigger = false;
        shortcutMovement.CloseNow(timeout, this);
    }

    public void Reset()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
}
