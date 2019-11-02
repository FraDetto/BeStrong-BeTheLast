using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    public GameObject boxDestroyer;
    // Start is called before the first frame update
    void Start()
    {
        boxDestroyer = FindObjectOfType<BoxDestroyer>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Force);
    }
}
