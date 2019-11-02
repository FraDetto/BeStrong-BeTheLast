using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroyer : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collisione " + other.tag);
        if (other.GetComponent<BoxMovement>())
        {
            Destroy(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
