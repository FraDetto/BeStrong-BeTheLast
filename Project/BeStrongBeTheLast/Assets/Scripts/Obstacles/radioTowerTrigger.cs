using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radioTowerTrigger : MonoBehaviour
{
    
    public float acceleration;
    public int respawnElecTime;
    public GameObject[] particlesToDis;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.root.GetComponentInChildren<KartController>().Accelerate(acceleration);
        StartCoroutine(disableElectricity());
    }

    IEnumerator disableElectricity()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        foreach(GameObject part in particlesToDis)
        {
            part.SetActive(false);
        }
        yield return new WaitForSeconds(respawnElecTime);

        foreach (GameObject part in particlesToDis)
        {
            part.SetActive(true);
        }
        this.gameObject.GetComponent<Collider>().enabled = false;
    }
}
