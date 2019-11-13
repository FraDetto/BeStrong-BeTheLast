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
    private GameObject[] projectiles;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        powerGauge.value = 0;
        index = 0;
        projectiles = new GameObject[] { trishot, homing, bouncing, attracting};
        selectedProjectile = projectiles[index];
        selectedProjectileText.text = selectedProjectile.name;
    }

    // Update is called once per frame
    void Update()
    {
        powerGauge.value += regenSpeed * Time.deltaTime;
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W) && powerGauge.value >= 0.5f)
        {
            Instantiate(selectedProjectile, frontSpawnpoint.position, frontSpawnpoint.rotation);
            powerGauge.value -= 0.5f;
        }
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.S) && powerGauge.value >= 0.5f)
        {
            Instantiate(selectedProjectile, rearSpawnpoint.position, rearSpawnpoint.rotation);
            powerGauge.value -= 0.5f;
        }
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(index == 3)
                index = 0;
            else
                index += 1;
            selectedProjectile = projectiles[index];
            selectedProjectileText.text = selectedProjectile.name;
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(index == 0)
                index = 3;
            else
                index -= 1;
            selectedProjectile = projectiles[index];
            selectedProjectileText.text = selectedProjectile.name;
        }
    }
}
