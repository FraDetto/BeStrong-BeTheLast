using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask roadMask;

    [SerializeField]
    private float accelerationFromShot;

    private GameObject bouncing;

    private void Start()
    {
        bouncing = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(bouncing.transform.position, Vector3.down, out hit, Mathf.Infinity, roadMask))
        {
            bouncing.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(bouncing.transform.up, hit.normal, 1f), -bouncing.transform.forward);
            bouncing.transform.Rotate(Vector3.right, 90f);
        }

        transform.Rotate(Vector3.up, -90f * bouncing.GetComponent<BouncingBehaviour>().speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var kartController = other.transform.parent.GetComponentInChildren<KartController>();
            kartController.Accelerate(accelerationFromShot);
            Destroy(this.gameObject);
        }
    }
}
