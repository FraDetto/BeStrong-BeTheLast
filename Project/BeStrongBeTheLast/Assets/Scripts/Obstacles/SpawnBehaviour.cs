using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehaviour : PausableMonoBehaviour
{
    public enum BarrelType
    {
        Accelerating, Decelerating
    }

    public BarrelType barrelType = BarrelType.Accelerating;

    public GameObject acceleratingBarrel;
    public GameObject deceleratingBarrel;

    private GameObject spawnedBarrel;

    private bool respawning;

    [SerializeField]
    private float lengthTimeInSeconds = 30f;

    // Start is called before the first frame update
    void Start()
    {
        if(barrelType.Equals(BarrelType.Accelerating))
            spawnedBarrel = Instantiate(acceleratingBarrel, transform.position, transform.rotation);
        else
            spawnedBarrel = Instantiate(deceleratingBarrel, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawnedBarrel)
        {
            if(!respawning)
                StartCoroutine(RespawnTime());
        }
    }

    IEnumerator RespawnTime()
    {
        respawning = true;
        yield return new WaitForSeconds(lengthTimeInSeconds);
        if(barrelType.Equals(BarrelType.Accelerating))
            spawnedBarrel = Instantiate(acceleratingBarrel, transform.position, transform.rotation);
        else
            spawnedBarrel = Instantiate(deceleratingBarrel, transform.position, transform.rotation);
        respawning = false;
    }
}
