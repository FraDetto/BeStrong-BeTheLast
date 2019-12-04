using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterBehaviour : MonoBehaviour
{
    private GameObject user;
    
    // Start is called before the first frame update
    void Start()
    {
        user = transform.root.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("CPU"))
        {
            if(!other.transform.root.gameObject.Equals(user))
            {
                other.transform.root.GetComponentInChildren<KartCollision>().countered = true;
            }
        }
    }
}
