using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingMobSpawner : MonoBehaviour
{
    public Vector3 spawnPos;
    public GameObject wanderingMobPrefab;
    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.GetChild(0).transform.position;
    }

    // Update is called once per frame
    public void SpawnNew()
    {
        GameObject mob = Instantiate(wanderingMobPrefab, spawnPos, Quaternion.identity);
        mob.transform.parent = gameObject.transform;
    }
}
