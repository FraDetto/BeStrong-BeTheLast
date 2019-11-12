using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private Slider powerGauge;
    [SerializeField] private Text selectedProjectileText;
    [SerializeField] private float regenSpeed;
    [SerializeField] private GameObject trishot;
    [SerializeField] private GameObject homing;
    [SerializeField] private GameObject bouncing;
    [SerializeField] private GameObject attracting;
    [SerializeField] private Transform frontSpawnpoint;
    [SerializeField] private Transform rearSpawnpoint;

    private GameObject selectedProjectile;

    // Start is called before the first frame update
    void Start()
    {
        powerGauge.value = 0;
        selectedProjectile = trishot;
        selectedProjectileText.text = "Trishot";
    }

    // Update is called once per frame
    void Update()
    {
        powerGauge.value += regenSpeed * Time.deltaTime;
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W) && powerGauge.value >= 0.5f)
        {
            Instantiate(selectedProjectile, frontSpawnpoint.position, frontSpawnpoint.rotation);
        }
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.S) && powerGauge.value >= 0.5f)
        {
            Instantiate(selectedProjectile, rearSpawnpoint.position, rearSpawnpoint.rotation);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            selectedProjectile = homing;
            selectedProjectileText.text = "Homing";
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedProjectile = trishot;
            selectedProjectileText.text = "Trishot";
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedProjectile = bouncing;
            selectedProjectileText.text = "Bouncing";
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedProjectile = attracting;
            selectedProjectileText.text = "Attracting";
        }
    }
}
