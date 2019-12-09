using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutInstantClose : MonoBehaviour
{
    public GameObject wall;
    private ShortcutMovement shortcutMovement;
    public int timeoutReset;
    public SplineObject shortcutSpline, mainSpline;
    private float oldShortcutSplineChance, oldMainSplineChance;
    private bool splineDisabled = false;

    private void Start()
    {
        shortcutMovement = wall.GetComponent<ShortcutMovement>();
        shortcutMovement.setTrigger(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject.transform.root.gameObject;
        if (GB.CompareORTags(go, "Player", "CPU"))
        {
            InstantClose(timeoutReset);
        }
    }

    private void InstantClose(int timeout)
    {
        transform.GetComponent<Collider>().isTrigger = false;
        shortcutMovement.CloseNow(timeout);
        disableShortcutSpline();
    }

    public void disableShortcutSpline()
    {
        if (!splineDisabled)
        {
            oldShortcutSplineChance = shortcutSpline.probability;
            oldMainSplineChance = mainSpline.probability;
            shortcutSpline.probability = 0;
            mainSpline.probability = 1;
            splineDisabled = true;
        }
    }

    public void resetShortcutSpline()
    {
        shortcutSpline.probability = oldShortcutSplineChance;
        mainSpline.probability = oldMainSplineChance;
        splineDisabled = false;
    }
    
    public void Reset()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        resetShortcutSpline();
    }
}
