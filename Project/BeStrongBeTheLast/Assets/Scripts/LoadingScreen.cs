using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;

    public GameObject UI;
    public List<GameObject> characters;
    public GameObject spawnPoints;

    public float pointSpawningTime=0.1f;

    private bool ready=false;

    internal AudioClip songToBeLoaded;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        loadingText.text = "LOADING.";
        StartCoroutine(LoadingPoints());
        StartCoroutine(SetupGame());
    }

    public IEnumerator LoadingPoints()
    {
        while(true)
        {
            if(loadingText.text.Contains("..."))
                loadingText.text = "LOADING.";
            else
                loadingText.text += ".";

            if(ready)
            {
                foreach(var lapmanager in UI.GetComponentsInChildren<LapManager>())
                    lapmanager.Start_();

                foreach(var abilities in UI.GetComponentsInChildren<Abilities>())
                    abilities.Start_();

                var audio = GameManager.Instance.GetComponent<AudioSource>();
                audio.clip = songToBeLoaded;
                audio.Play();

                gameObject.SetActive(false);
                break;
            }
            else yield return new WaitForSeconds(pointSpawningTime);
        }
    }

    public IEnumerator SetupGame()
    {
        while(true)
        {
            UI = GameObject.Find("UI");
            spawnPoints = GameObject.Find("PlayersSpawnPositions");

            foreach(var player in GameObject.FindGameObjectsWithTag("Player"))
                if(!characters.Contains(player.transform.root.gameObject))
                    characters.Add(player.transform.root.gameObject);

            if(UI && spawnPoints && characters.Count == 8)
            {
                var spawnPointCounter = 0;

                var player1kart = characters.Find(character => character.name.Equals(GameManager.Instance.player1choice));
                var p1SpawnPointPos = spawnPoints.transform.GetChild(spawnPointCounter).position;
                player1kart.transform.position = new Vector3(p1SpawnPointPos.x, player1kart.transform.position.y, p1SpawnPointPos.z);
                spawnPointCounter += 1;
                characters.Remove(player1kart);

                var p1kartController = player1kart.GetComponentInChildren<KartController>();
                p1kartController.KCType = aKartController.eKCType.Human;
                p1kartController.playerNumber = 1;

                var p1virtualCamera = UI.transform.GetChild(0).GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
                p1virtualCamera.m_Follow = p1kartController.transform;
                p1virtualCamera.m_LookAt = p1kartController.transform;

                p1kartController.camera_ = UI.transform.GetChild(0).GetComponent<Camera>();
                p1kartController.vCam = UI.transform.GetChild(0).GetComponentInChildren<Cinemachine.CinemachineImpulseSource>();

                var p1canvas = UI.transform.GetChild(2);
                p1canvas.GetComponent<LapManager>().player = player1kart.gameObject;
                p1canvas.GetComponent<Abilities>().kartController = p1kartController;

                if(GameManager.Instance.player2added)
                {
                    UI.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 1);
                    UI.transform.GetChild(1).gameObject.SetActive(true);

                    p1canvas.Find("PowerGaugePanel").GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 225f, 0f);

                    var minimap = UI.transform.Find("MinimapCanvas");
                    minimap.GetChild(0).gameObject.SetActive(false);
                    minimap.GetChild(1).gameObject.SetActive(true);

                    var player2kart = characters.Find(character => character.name.Equals(GameManager.Instance.player2choice));
                    var p2SpawnPointPos = spawnPoints.transform.GetChild(spawnPointCounter).position;
                    player2kart.transform.position = new Vector3(p2SpawnPointPos.x, player2kart.transform.position.y, p2SpawnPointPos.z);
                    spawnPointCounter += 1;
                    characters.Remove(player2kart);

                    var p2kartController = player2kart.GetComponentInChildren<KartController>();
                    p2kartController.KCType = aKartController.eKCType.Human;
                    p2kartController.playerNumber = 2;

                    var p2virtualCamera = UI.transform.GetChild(1).GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
                    p2virtualCamera.m_Follow = p2kartController.transform;
                    p2virtualCamera.m_LookAt = p2kartController.transform;

                    p1kartController.camera_ = UI.transform.GetChild(1).GetComponent<Camera>();
                    p1kartController.vCam = UI.transform.GetChild(1).GetComponentInChildren<Cinemachine.CinemachineImpulseSource>();

                    var p2canvas = UI.transform.GetChild(3);
                    p2canvas.gameObject.SetActive(true);
                    p2canvas.GetComponent<LapManager>().player = player2kart.gameObject;
                    p2canvas.GetComponent<Abilities>().kartController = p2kartController;
                }

                while(characters.Count > 0)
                {
                    var cpuKart = characters[Random.Range(0, characters.Count)];
                    var cpuSpawnPointPos = spawnPoints.transform.GetChild(spawnPointCounter).position;
                    cpuKart.transform.position = new Vector3(cpuSpawnPointPos.x, cpuKart.transform.position.y, cpuSpawnPointPos.z);
                    spawnPointCounter += 1;
                    characters.Remove(cpuKart);

                    var cpuKartController = cpuKart.GetComponentInChildren<KartController>();
                    cpuKartController.KCType = aKartController.eKCType.CPU;
                    GameState.Instance.kartTypes.Remove(cpuKart.name);
                    GameState.Instance.kartTypes.Add(cpuKart.name, "CPU");
                    cpuKartController.playerNumber = 0;
                    cpuKartController.camera_ = null;
                    cpuKartController.vCam = null;
                }

                ready = true;
                break;
            }
            else yield return new WaitForSeconds(pointSpawningTime);
        }
    }
}
