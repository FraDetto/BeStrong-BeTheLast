using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterPortal : PausableMonoBehaviour
{
    public EndManager endScriptCallback;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            endScriptCallback.teleportCar(go);
            endScriptCallback.teleporterSpawned = false;
            Destroy(transform.root.gameObject);
        }
    }
}
