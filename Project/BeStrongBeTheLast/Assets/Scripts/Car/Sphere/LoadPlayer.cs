using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LoadPlayer : MonoBehaviour
{
    private GameObject playerPrefab;
    public GameObject camera;
    private void Start()
    {
        Debug.Log("Models/Real Kart/Prefabs/" + GameState.getInstance().getPlayerChamp());
        GameObject player = Instantiate(Resources.Load("Models/Real Kart/Prefabs/" + GameState.getInstance().getPlayerChamp()) as GameObject, 
            transform.position, Quaternion.identity, transform);
        Transform kart = player.transform.Find("Kart");
        camera.GetComponent<CinemachineVirtualCamera>().Follow = kart;
        camera.GetComponent<CinemachineVirtualCamera>().LookAt = kart;
    }
}
