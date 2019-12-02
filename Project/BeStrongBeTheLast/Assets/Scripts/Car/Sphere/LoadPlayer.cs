using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LoadPlayer : MonoBehaviour
{
    private GameObject playerPrefab;
    public CinemachineImpulseSource camera1, camera2, camera3, camera4;
    public SplineObject firstSpline;
    private void Start()
    {
        GameObject player = Instantiate(Resources.Load("Models/Real Kart/Prefabs/" + GameState.getInstance().getPlayerChamp()) as GameObject, 
            transform.position, Quaternion.identity, transform);
        player.gameObject.name = "Player";
        Transform kart = player.transform.Find("Kart");
        camera1.GetComponent<CinemachineVirtualCamera>().Follow = kart;
        camera1.GetComponent<CinemachineVirtualCamera>().LookAt = kart;
        camera2.GetComponent<CinemachineVirtualCamera>().Follow = kart;
        camera2.GetComponent<CinemachineVirtualCamera>().LookAt = kart;
        camera3.GetComponent<CinemachineVirtualCamera>().Follow = kart;
        camera3.GetComponent<CinemachineVirtualCamera>().LookAt = kart;
        camera4.GetComponent<CinemachineVirtualCamera>().Follow = kart;
        camera4.GetComponent<CinemachineVirtualCamera>().LookAt = kart;
        KartController kartController = player.GetComponentInChildren<KartController>();
        kartController.CurrentSplineObject = firstSpline;
        kartController.vCam = camera1;
        transform.DetachChildren();
        Destroy(this);
    }
}
