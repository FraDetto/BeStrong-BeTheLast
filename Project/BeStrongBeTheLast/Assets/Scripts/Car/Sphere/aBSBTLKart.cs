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
using System.Collections;

public abstract class aBSBTLKart : aKartController
{

    [Header("Abilities - UI")]
    [SerializeField] private Slider powerGauge;
    [SerializeField] private Text counterText;
    [SerializeField] private Text selectedProjectileText;
    [SerializeField] private Text selectedSpecialText;

    public Image blindingFront;
    public Image blindingRear;

    [Header("Abilities - Properties")]
    [SerializeField] private Transform frontSpawnpoint;
    [SerializeField] private Transform rearSpawnpoint;
    [SerializeField] private float regenSpeed;
    [SerializeField] private float counterCooldown;
    [SerializeField] private float projectileCooldown;
    [SerializeField] private float specialCooldown;

    public GameObject counter;
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
    private bool counterRecharging;
    private bool projectileRecharging;
    private bool specialRecharging;
    private float powerGaugeValue;
    private Color disabledColor = Color.red;
    private Color enabledColor = Color.yellow;

    protected new void Start_()
    {
        projectiles = new GameObject[] { trishot, homing, bouncing, attracting };
        selectedProjectile = projectiles[index];

        if(counterText)
            counterText.color = disabledColor;

        if(selectedProjectileText)
        {
            selectedProjectileText.text = selectedProjectile.name;
            selectedProjectileText.color = disabledColor;
        }
            

        specials = new GameObject[] { blinding, annoying, tanking, rotating };
        selectedSpecial = specials[index];

        if(selectedSpecialText)
        {
            selectedSpecialText.text = selectedSpecial.name;
            selectedProjectileText.color = disabledColor;
        }
            
        debuff = transform.Find("Debuff");

        base.Start_();
    }

    protected new void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        powerGaugeValue += regenSpeed * Time.deltaTime;
        if(powerGaugeValue > 1f)
            powerGaugeValue = 1f;

        if (powerGauge)
        {
            
            powerGauge.value = powerGaugeValue;

            if(canUseCounter())
            {
                counterText.color = enabledColor;

                if(Input.GetMouseButtonDown(2))
                    Counter();
            }

            if(canUseProjectile())
            {
                selectedProjectileText.color = enabledColor;

                if(Input.GetMouseButtonDown(0))
                {
                    var y = Input.GetAxis("Vertical");

                    if(y > 0)
                        Projectile(frontSpawnpoint);
                    else if(y < 0)
                        Projectile(rearSpawnpoint);
                }
            }   

            if(canUseSpecial())
            {
                selectedSpecialText.color = enabledColor;
                if(Input.GetMouseButtonDown(1))
                    Special();
            }
                

            var MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if(MouseScrollWheel != 0 && !attracted)
            {
                if(MouseScrollWheel > 0)
                    index = (index == 3 ? 0 : index + 1);
                else if(MouseScrollWheel < 0)
                    index = (index == 0 ? 3 : index - 1);

                selectedProjectile = projectiles[index];
                selectedSpecial = specials[index];
            }

            if(selectedProjectileText != null)
                selectedProjectileText.text = selectedProjectile.name;

            if(selectedSpecialText != null)
                selectedSpecialText.text = selectedSpecial.name;
        }

        base.Update_(xAxis, jumpBDown, jumpBUp);
    }

    protected void Counter()
    {
        Instantiate(counter, transform);

        if(powerGauge)
            disableAll();

        powerGaugeValue -= 0.25f;
        counterRecharging = true;
        StartCoroutine(CounterCooldown());
    }

    protected void Projectile(Transform spawnPoint)
    {
        Instantiate(selectedProjectile, spawnPoint.position, spawnPoint.rotation, transform);

        if(powerGauge)
            disableAll();
        
        if(!attracted)
        {
            powerGaugeValue -= 0.5f;
            projectileRecharging = true;
            StartCoroutine(ProjectileCooldown());
        }
        else
        {
            selectedProjectile = attracting;
            attracted = false;
        }
    }

    protected void Special()
    {
        Instantiate(selectedSpecial, transform);

        if(powerGauge)
            disableAll();
            
        powerGaugeValue -= 0.75f;
        specialRecharging = true;
        StartCoroutine(SpecialCooldown());
    }

    protected bool canUseCounter() =>
        powerGaugeValue >= 0.25f && !counterRecharging;

    protected bool canUseProjectile() =>
      (powerGaugeValue >= 0.5f && !projectileRecharging) || attracted;

    protected bool canUseSpecial() =>
       powerGaugeValue >= 0.75f && !specialRecharging;

    IEnumerator CounterCooldown()
    {
        yield return new WaitForSeconds(counterCooldown);
        counterRecharging = false;
    }

    IEnumerator ProjectileCooldown()
    {
        yield return new WaitForSeconds(projectileCooldown);
        projectileRecharging = false;
    }

    IEnumerator SpecialCooldown()
    {
        yield return new WaitForSeconds(specialCooldown);
        specialRecharging = false;
    }

    protected void disableAll()
    {
        counterText.color = disabledColor;
        selectedProjectileText.color = disabledColor;
        selectedSpecialText.color = disabledColor;
    }
}