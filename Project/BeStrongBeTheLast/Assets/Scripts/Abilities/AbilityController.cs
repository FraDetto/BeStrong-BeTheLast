/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{

    [SerializeField] private Slider powerGauge;
    [SerializeField] private float regenSpeed;
    [SerializeField] private Text selectedProjectileText;
    [SerializeField] private Text selectedSpecialText;
    [SerializeField] private Transform frontSpawnpoint;
    [SerializeField] private Transform rearSpawnpoint;

    public GameObject trishot;
    public GameObject homing;
    public GameObject bouncing;
    public GameObject attracting;
    public GameObject blinding;
    public GameObject annoying;
    public GameObject tanking;
    public GameObject rotating;
    public Image blindingFront;
    public Image blindingRear;
    public GameObject selectedProjectile;
    public GameObject selectedSpecial;

    private GameObject[] projectiles;
    private GameObject[] specials;
    private int index;

    public bool attracted;


    void Start()
    {
        if (powerGauge != null)
            powerGauge.value = 0;

        projectiles = new GameObject[] { trishot, homing, bouncing, attracting };
        selectedProjectile = projectiles[index];

        if (selectedProjectileText != null)
            selectedProjectileText.text = selectedProjectile.name;

        specials = new GameObject[] { blinding, annoying, tanking, rotating};
        selectedSpecial = specials[index];

        if(selectedSpecialText != null)
            selectedSpecialText.text = selectedSpecial.name;
    }

    void Update()
    {
        if (powerGauge != null)
        {
            powerGauge.value += regenSpeed * Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
                if (powerGauge.value >= 0.5f || attracted)
                {
                    if (Input.GetKey(KeyCode.W))
                        SpawnProjectile(frontSpawnpoint);
                    else if (Input.GetKey(KeyCode.S))
                        SpawnProjectile(rearSpawnpoint);
                }

            if(Input.GetMouseButtonDown(1) && powerGauge.value >= 0.75f)
            {
                Instantiate(selectedSpecial, transform);
                powerGauge.value -= 0.75f;
            }

        }

        var MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (MouseScrollWheel != 0 && !attracted)
        {
            if (MouseScrollWheel > 0)
                index = (index == 3 ? 0 : index + 1);
            else if (MouseScrollWheel < 0)
                index = (index == 0 ? 3 : index - 1);

            selectedProjectile = projectiles[index];
            selectedSpecial = specials[index];
        }

        if (selectedProjectileText != null)
            selectedProjectileText.text = selectedProjectile.name;

        if(selectedSpecialText != null)
            selectedSpecialText.text = selectedSpecial.name;
    }

    private void SpawnProjectile(Transform spawnPoint)
    {
        Instantiate(selectedProjectile, spawnPoint.position, spawnPoint.rotation, transform);

        if (!attracted)
            powerGauge.value -= 0.5f;
        else
        {
            selectedProjectile = attracting;
            attracted = false;
        }
    }
}