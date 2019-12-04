using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBehaviour : MonoBehaviour
{
    private GameObject user;

    [SerializeField]
    private float lengthTimeInSeconds = 10f;

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
                other.transform.root.GetComponentInChildren<KartCollision>().rotatingPush = 2f;
            }
        }
    }
}
