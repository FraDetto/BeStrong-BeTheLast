/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayer : PausableMonoBehaviour
{

    [Range(0, 8)]
    public byte PlayerNumber;

    private GameObject playerOnTheSceneOrCPUToFollow;
    public KartController kartOnTheSceneOrCPUToFollow;
    public GameObject UI;

    public List<CinemachineImpulseSource> cameras = new List<CinemachineImpulseSource>();
    public SplineObject firstSpline;


    private void Start()
    {
        if (PlayerNumber > 0 && GameState.Instance.playersChampName.Count > 0)
        {
            playerOnTheSceneOrCPUToFollow = Instantiate(
                Resources.Load("Models/Prefabs/Characters/" + GameState.Instance.playersChampName[PlayerNumber - 1]) as GameObject,
                transform.position,
                Quaternion.identity
            );

            playerOnTheSceneOrCPUToFollow.gameObject.name = GameState.Instance.playersChampName[PlayerNumber - 1];

            kartOnTheSceneOrCPUToFollow = playerOnTheSceneOrCPUToFollow.GetComponentInChildren<KartController>();
            kartOnTheSceneOrCPUToFollow.KCType = aKartController.eKCType.Human;
            kartOnTheSceneOrCPUToFollow.Paused = true;
            kartOnTheSceneOrCPUToFollow.playerNumber = PlayerNumber;
            UI.GetComponentInChildren<LapManager>().player = playerOnTheSceneOrCPUToFollow;
            UI.GetComponentInChildren<Abilities>().kartController = kartOnTheSceneOrCPUToFollow;
            EndManager endManager =  firstSpline.transform.parent.GetComponentInChildren<EndManager>();
            endManager.cars[5 + PlayerNumber] = playerOnTheSceneOrCPUToFollow;
        }


        CinemachineImpulseSource camera1 = null;

        foreach (var camera in cameras)
        {
            var cinemachineVirtualCamera = camera.GetComponent<CinemachineVirtualCamera>();

            cinemachineVirtualCamera.Follow = kartOnTheSceneOrCPUToFollow.transform;
            cinemachineVirtualCamera.LookAt = kartOnTheSceneOrCPUToFollow.transform;

            if (!camera1 && camera.enabled)
                camera1 = camera;
        }

        kartOnTheSceneOrCPUToFollow.CurrentSplineObject = firstSpline;
        kartOnTheSceneOrCPUToFollow.vCam = camera1;
        kartOnTheSceneOrCPUToFollow.UsaWrongWay = true;
    }

}