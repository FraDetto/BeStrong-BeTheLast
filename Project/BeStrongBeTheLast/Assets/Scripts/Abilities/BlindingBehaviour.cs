using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindingBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject blinded;

    private GameObject[] players;
    private GameObject[] bots;
    private GameObject user;

    // Start is called before the first frame update
    void Start()
    {
        user = transform.parent.gameObject;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
            if(player != user)
            {
                AbilityController ac = player.GetComponent<AbilityController>();
                if(ac != null)
                {
                    Instantiate(blinded, player.transform);
                    ac.blindingFront.enabled = true;
                    ac.blindingRear.enabled = true;
                }
            }
        bots = GameObject.FindGameObjectsWithTag("CPU");
        foreach(GameObject bot in bots)
        {
            //TO DO: Make them drive bad
        }
        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(30f);
        foreach(GameObject player in players)
            if(player != user)
            {
                AbilityController ac = player.GetComponent<AbilityController>();
                if(ac != null)
                {
                    Destroy(player.transform.Find("Blinded(Clone)").gameObject);
                    ac.blindingFront.enabled = false;
                    ac.blindingRear.enabled = false;
                }
            }
        bots = GameObject.FindGameObjectsWithTag("CPU");
        foreach(GameObject bot in bots)
        {
            //TO DO: Make their drive return to normal
        }

        if(enabled)
            Destroy(gameObject);
    }
}
