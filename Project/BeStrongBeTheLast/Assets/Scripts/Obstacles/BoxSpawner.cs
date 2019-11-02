using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject BoxAccelerating, BoxSlowing;
    public bool GreenTrueRedFalse;
    int startSpawn = 0;
    int startColour;

    // Start is called before the first frame update
    void Start()
    {
        startColour = Random.Range(0,10);
        if(startColour < 5)
        {
            GreenTrueRedFalse = true;
        }
        else
        {
            GreenTrueRedFalse = false;
        }
       
        StartCoroutine(spawnBox());
    }

   IEnumerator spawnBox()
    {
        GameObject go;
        if (startSpawn == 0)
        {
            int waitTime = Random.Range(1, 3);
            yield return new WaitForSeconds(waitTime);
        }
        
        while (true)
        {
            if (GreenTrueRedFalse)
            {
                go = Instantiate(BoxAccelerating, transform.position, Quaternion.identity);
                go.transform.Rotate(0, -90, 0);
                //GreenTrueRedFalse = false;
                startColour = Random.Range(0, 10);
                if (startColour < 5)
                {
                    GreenTrueRedFalse = true;
                }
                else
                {
                    GreenTrueRedFalse = false;
                }
            }
            else
            {
                go = Instantiate(BoxSlowing, transform.position, Quaternion.identity);
                go.transform.Rotate(0, -90, 0);
                //GreenTrueRedFalse = true;
                startColour = Random.Range(0, 10);
                if (startColour < 5)
                {
                    GreenTrueRedFalse = true;
                }
                else
                {
                    GreenTrueRedFalse = false;
                }
            }
            yield return new WaitForSeconds(1.6f);
        }
       
    }
}
