using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehaviour : MonoBehaviour
{
    private Collider collider;
    private MeshRenderer mesh;

    [SerializeField]
    private float respawnTime = 15f;

    private void Start()
    {
        collider = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("CPU"))
        {
            other.transform.root.GetComponentInChildren<KartController>().powerGaugeValue += 0.33f;
            collider.enabled = false;
            mesh.enabled = false;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        collider.enabled = true;
        mesh.enabled = true;
    }
}
