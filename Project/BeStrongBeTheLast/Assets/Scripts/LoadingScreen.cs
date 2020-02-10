using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    public Transform UIsingle;
    public Transform UImulti;
    public List<Transform> characters;
    public Transform spawnPoints;

    private bool ready=false;

    // Start is called before the first frame update
    void Start()
    {
        var gameManager = GameObject.Find("GameManager");
        if(gameManager)
        {
            loadingText.text = "LOADING";
            this.gameObject.SetActive(true);
            StartCoroutine(LoadingPoints());
            SetupGame();
        }
        else
        {
            if(UIsingle.gameObject.activeInHierarchy)
                UIsingle.GetComponentInChildren<LapManager>().Start_();
            else
                foreach(var lapManager in UImulti.GetComponentsInChildren<LapManager>())
                    lapManager.Start_();

            this.gameObject.SetActive(false);
        }
    }

    public IEnumerator LoadingPoints()
    {
        yield return new WaitForSeconds(1f);

        if(loadingText.text.Contains("..."))
            loadingText.text = "LOADING";
        else
            loadingText.text += ".";

        if(!ready)
            StartCoroutine(LoadingPoints());
        else
        {
            if(GameManager.instance.player2added)
            {
                UIsingle.gameObject.SetActive(false);
                UImulti.gameObject.SetActive(true);
                foreach(var lapManager in UImulti.GetComponentsInChildren<LapManager>())
                    lapManager.Start_();
                foreach(var abilities in UImulti.GetComponentsInChildren<Abilities>())
                    abilities.Start_();
            }
            else
            {
                UIsingle.GetComponentInChildren<LapManager>().Start_();
                UIsingle.GetComponentInChildren<Abilities>().Start_();
            }
            
            this.gameObject.SetActive(false); 
        }
    }

    void SetupGame()
    {
        var spawnPointCounter = 0;
        var player1kart = characters.Find(character => character.name.Equals(GameManager.instance.player1choice));
        player1kart.position = spawnPoints.GetChild(spawnPointCounter).position;
        spawnPointCounter += 1;
        characters.Remove(player1kart);

        var p1kartController = player1kart.GetComponentInChildren<KartController>();
        p1kartController.KCType = aKartController.eKCType.Human;
        p1kartController.playerNumber = 1;

        if(GameManager.instance.player2added)
        {
            var player2kart = characters.Find(character => character.name.Equals(GameManager.instance.player2choice));
            player2kart.position = spawnPoints.GetChild(spawnPointCounter).position;
            spawnPointCounter += 1;
            characters.Remove(player2kart);

            var p2kartController = player2kart.GetComponentInChildren<KartController>();
            p2kartController.KCType = aKartController.eKCType.Human;
            p2kartController.playerNumber = 2;

            var p1VirtualCameras = UImulti.GetChild(0).GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
            foreach(var virtualCamera in p1VirtualCameras)
            {
                virtualCamera.m_Follow = p1kartController.transform;
                virtualCamera.m_LookAt = p1kartController.transform;
            }

            p1kartController.camera_ = UImulti.GetChild(0).GetComponent<Camera>();
            p1kartController.vCam = UImulti.GetChild(0).GetComponentInChildren<Cinemachine.CinemachineImpulseSource>();

            var p2VirtualCameras = UImulti.GetChild(1).GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
            foreach(var virtualCamera in p2VirtualCameras)
            {
                virtualCamera.m_Follow = p2kartController.transform;
                virtualCamera.m_LookAt = p2kartController.transform;
            }

            p1kartController.camera_ = UImulti.GetChild(1).GetComponent<Camera>();
            p1kartController.vCam = UImulti.GetChild(1).GetComponentInChildren<Cinemachine.CinemachineImpulseSource>();

            var lapManagers = UImulti.GetComponentsInChildren<LapManager>();
            lapManagers[0].player = player1kart.gameObject;
            lapManagers[1].player = player2kart.gameObject;

            var abilities = UImulti.GetComponentsInChildren<Abilities>();
            abilities[0].kartController = p1kartController;
            abilities[1].kartController = p2kartController;
        }
        else
        {
            var p1VirtualCameras = UIsingle.GetChild(0).GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
            foreach(var virtualCamera in p1VirtualCameras)
            {
                virtualCamera.m_Follow = p1kartController.transform;
                virtualCamera.m_LookAt = p1kartController.transform;
            }

            UIsingle.GetComponentInChildren<LapManager>().player = player1kart.gameObject;
            UIsingle.GetComponentInChildren<Abilities>().kartController = p1kartController;
            p1kartController.camera_ = UIsingle.GetChild(0).GetComponent<Camera>();
            p1kartController.vCam = UIsingle.GetChild(0).GetComponentInChildren<Cinemachine.CinemachineImpulseSource>();
        }

        while(characters.Count > 0)
        {
            var cpuKart = characters[Random.Range(0, characters.Count)];
            cpuKart.position = spawnPoints.GetChild(spawnPointCounter).position;
            spawnPointCounter += 1;
            characters.Remove(cpuKart);
            var cpuKartController = cpuKart.GetComponentInChildren<KartController>();
            cpuKartController.KCType = aKartController.eKCType.CPU;
            cpuKartController.playerNumber = 0;
            cpuKartController.camera_ = null;
            cpuKartController.vCam = null;
        }

        ready = true;
    }
}
