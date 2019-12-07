/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] protected Transform frontSpawnpoint;
    [SerializeField] protected Transform rearSpawnpoint;
    [SerializeField] private float regenSpeed;
    [SerializeField] private float counterCooldown;
    [SerializeField] private float projectileCooldown;
    [SerializeField] private float specialCooldown;

    public enum ePlayer
    {
        Hypogeum,
        EarthRestorer,
        Steamdunker,
        Imps,
        Flapper,
        Kiddo,
        Politician,
        Bard
    }

    public struct sAbilities
    {
        public GameObject selectedProjectile, selectedSpecial;

        public sAbilities(GameObject selectedProjectile, GameObject selectedSpecial)
        {
            this.selectedProjectile = selectedProjectile;
            this.selectedSpecial = selectedSpecial;
        }
    }

    public ePlayer playerType = ePlayer.Kiddo;

    private Dictionary<ePlayer, sAbilities> abilities;

    public GameObject counter;
    public GameObject trishot;
    public GameObject homing;
    public GameObject bouncing;
    public GameObject attracting;
    public GameObject blinding;
    public GameObject annoying;
    public GameObject tanking;
    public GameObject rotating;

    internal sAbilities myAbility;

    internal bool attracted;
    internal Transform debuff;

    private int index;
    private bool counterRecharging;
    private bool projectileRecharging;
    private bool specialRecharging;
    private float powerGaugeValue;
    private Color disabledColor = Color.red;
    private Color enabledColor = Color.yellow;
    protected bool iAmBlinded;

    private const byte DifferentPlayersType = 8;

    protected new void Start_()
    {
        abilities = new Dictionary<ePlayer, sAbilities>()
        {
            { ePlayer.Bard, new sAbilities(homing, annoying)},
            { ePlayer.EarthRestorer, new sAbilities(bouncing, rotating)},
            { ePlayer.Flapper, new sAbilities(bouncing, tanking)},
            { ePlayer.Hypogeum, new sAbilities(homing, tanking)},
            { ePlayer.Imps, new sAbilities(trishot, annoying)},
            { ePlayer.Kiddo, new sAbilities(attracting, rotating)},
            { ePlayer.Politician, new sAbilities(trishot, blinding)},
            { ePlayer.Steamdunker, new sAbilities(attracting, blinding)},
        };

        myAbility = abilities[playerType];

        if (counterText)
            counterText.color = disabledColor;

        if (selectedProjectileText)
        {
            selectedProjectileText.text = myAbility.selectedProjectile.name;
            selectedProjectileText.color = disabledColor;
        }

        if (selectedSpecialText)
        {
            selectedSpecialText.text = myAbility.selectedSpecial.name;
            selectedProjectileText.color = disabledColor;
        }

        debuff = transform.Find("Debuff");

        base.Start_();
    }

    protected new void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        powerGaugeValue += regenSpeed * Time.deltaTime;

        if (powerGaugeValue > 1f)
            powerGaugeValue = 1f;

        if (powerGauge)
        {
            powerGauge.value = powerGaugeValue;

            if (canUseCounter())
            {
                counterText.color = enabledColor;

                if (Input.GetButtonDown("Fire3") || Input.GetAxis("AltFire3") != 0)
                    Counter();
            }

            if (canUseProjectile())
            {
                selectedProjectileText.color = enabledColor;

                if (Input.GetButtonDown("Fire1"))
                {
                    var y = Input.GetAxis("Vertical");

                    if (y >= 0)
                        Projectile(frontSpawnpoint);
                    else
                        Projectile(rearSpawnpoint);
                }
            }

            if (canUseSpecial())
            {
                selectedSpecialText.color = enabledColor;
                if (Input.GetButtonDown("Fire2") || Input.GetAxis("AltFire2") != 0)
                    Special();
            }

            var MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if (MouseScrollWheel != 0 && !attracted)
            {
                if (MouseScrollWheel > 0)
                    index = (index == DifferentPlayersType - 1 ? 0 : index + 1);
                else if (MouseScrollWheel < 0)
                    index = (index == 0 ? DifferentPlayersType - 1 : index - 1);

                myAbility = abilities[(ePlayer)index];
            }

            if (selectedProjectileText != null)
                selectedProjectileText.text = myAbility.selectedProjectile.name;

            if (selectedSpecialText != null)
                selectedSpecialText.text = myAbility.selectedSpecial.name;
        }

        base.Update_(xAxis, jumpBDown, jumpBUp);
    }

    protected void Counter()
    {
        Instantiate(counter, transform);

        if (powerGauge)
            disableAll();

        powerGaugeValue -= 0.25f;
        counterRecharging = true;
        StartCoroutine(CounterCooldown());
    }

    protected void Projectile(Transform spawnPoint)
    {
        Instantiate(myAbility.selectedProjectile, spawnPoint.position, spawnPoint.rotation, transform);

        if (powerGauge)
            disableAll();

        if (!attracted)
        {
            powerGaugeValue -= 0.5f;
            projectileRecharging = true;
            StartCoroutine(ProjectileCooldown());
        }
        else
        {
            myAbility.selectedProjectile = attracting;
            attracted = false;
        }
    }

    protected void Special()
    {
        Instantiate(myAbility.selectedSpecial, transform);

        if (powerGauge)
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

    internal void blindMe(bool blind)
    {
        iAmBlinded = blind;

        debuff.Find("Blinded").gameObject.SetActive(blind);

        if (powerGauge)
        {
            blindingFront.enabled = blind;
            blindingRear.enabled = blind;
        }
    }

    internal void annoyMe(float annoyingAmount, bool annoy)
    {
        iAmAnnoyed = annoy;

        debuff.Find("Annoyed").gameObject.SetActive(annoy);
        drifting = annoy;

        if (annoy)
        {
            this.annoyingAmount = annoyingAmount;
            driftHeatingValue = 2f;
        }
        else
        {
            this.annoyingAmount = annoyingAmount;
            driftHeatingValue = -1f;
        }
    }

}