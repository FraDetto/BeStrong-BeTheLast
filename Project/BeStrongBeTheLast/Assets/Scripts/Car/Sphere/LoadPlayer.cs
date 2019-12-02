/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Jacopo Frasson
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Cinemachine;
using UnityEngine;

public class LoadPlayer : MonoBehaviour
{
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
        kartController.UsaWrongWay = true;
        RepulsiveWallStraight repulsiveWallStraight = player.GetComponentInChildren<RepulsiveWallStraight>();
        repulsiveWallStraight.AttivaCollisioniConMura = true;
        transform.DetachChildren();
        Destroy(this);
    }
}
