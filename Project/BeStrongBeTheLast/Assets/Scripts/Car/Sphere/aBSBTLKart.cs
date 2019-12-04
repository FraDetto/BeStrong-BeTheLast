/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.UI;

public abstract class aBSBTLKart : aKartController
{

    [Header("Abilities - UI")]
    [SerializeField] private Slider powerGauge;
    [SerializeField] private float regenSpeed;
    [SerializeField] private Text selectedProjectileText;
    [SerializeField] private Text selectedSpecialText;

    public Image blindingFront;
    public Image blindingRear;

    [Header("Abilities - Properties")]
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

    internal GameObject selectedProjectile;
    internal GameObject selectedSpecial;
    internal bool attracted;
    internal Transform debuff;

    private GameObject[] projectiles;
    private GameObject[] specials;
    private int index;

    private float powerGaugeValue;

    protected new void Start_()
    {
        projectiles = new GameObject[] { trishot, homing, bouncing, attracting };
        selectedProjectile = projectiles[index];

        if (selectedProjectileText)
            selectedProjectileText.text = selectedProjectile.name;

        specials = new GameObject[] { blinding, annoying, tanking, rotating };
        selectedSpecial = specials[index];

        if (selectedSpecialText)
            selectedSpecialText.text = selectedSpecial.name;

        debuff = transform.Find("Debuff");

        base.Start_();
    }

    protected new void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        if (powerGauge)
        {
            powerGaugeValue += regenSpeed * Time.deltaTime;
            powerGauge.value = powerGaugeValue;

            if (Input.GetMouseButtonDown(0))
                if (canUseProjectile())
                {
                    var y = Input.GetAxis("Vertical");

                    if (y > 0)
                        Projectile(frontSpawnpoint);
                    else if (y < 0)
                        Projectile(rearSpawnpoint);
                }

            if (Input.GetMouseButtonDown(1) && canUseSpecial())
                Special();
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

        if (selectedSpecialText != null)
            selectedSpecialText.text = selectedSpecial.name;

        base.Update_(xAxis, jumpBDown, jumpBUp);
    }

    protected void Projectile(Transform spawnPoint)
    {
        Instantiate(selectedProjectile, spawnPoint.position, spawnPoint.rotation, transform);

        if (!attracted)
            powerGaugeValue -= 0.5f;
        else
        {
            selectedProjectile = attracting;
            attracted = false;
        }
    }

    protected void Counter()
    {

    }

    protected void Special()
    {
        Instantiate(selectedSpecial, transform);
        powerGaugeValue -= 0.75f;
    }

    protected bool canUseCounter() =>
        powerGaugeValue >= 0.25f;

    protected bool canUseProjectile() =>
      powerGaugeValue >= 0.5f || attracted;

    protected bool canUseSpecial() =>
       powerGaugeValue >= 0.75f;

}