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
using UnityEngine.Rendering.PostProcessing;

public abstract class aBSBTLKart : aKartController
{

    [Header("Abilities - Properties")]
    [SerializeField] internal Transform frontSpawnpoint;
    [SerializeField] internal Transform rearSpawnpoint;
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
        public bool myProjectile_inAction, mySpecial_inAction, myAttractor_inAction;

        private aAbilitiesBehaviour myProjectileB, mySpecialB, myAttractorB;

        public GameObject myProjectile, mySpecial, myAttractor;


        public sAbilities(GameObject myProjectile, GameObject mySpecial) : this(myProjectile, mySpecial, null) { }

        public sAbilities(GameObject myProjectile, GameObject mySpecial, GameObject myAttractor)
        {
            myProjectile_inAction = false;
            mySpecial_inAction = false;
            myAttractor_inAction = false;

            myProjectileB = null;
            mySpecialB = null;
            myAttractorB = null;

            this.myProjectile = myProjectile;
            this.mySpecial = mySpecial;
            this.myAttractor = myAttractor;
        }

        public void InitProjectile()
        {
            var this_ = this;
            myProjectile_inAction = true;
            myProjectileB = myProjectile.GetComponent<aAbilitiesBehaviour>();
            myProjectileB.Killed = () => { this_.myProjectile_inAction = false; };
        }

        public void InitSpecial()
        {
            var this_ = this;
            mySpecial_inAction = true;
            mySpecialB = mySpecial.GetComponent<aAbilitiesBehaviour>();
            mySpecialB.Killed = () => { this_.mySpecial_inAction = false; };
        }

        public void InitAttractor()
        {
            var this_ = this;
            myAttractor_inAction = true;
            myAttractorB = myAttractor.GetComponent<aAbilitiesBehaviour>();
            myAttractorB.Killed = () => { this_.myAttractor_inAction = false; };
        }
    }

    public ePlayer playerType = ePlayer.Kiddo;

    private Dictionary<ePlayer, sAbilities> abilities;

    [Header("Abilities - Types")]
    public GameObject counter;
    public GameObject trishot;
    public GameObject homing;
    public GameObject bouncing;
    public GameObject attracting;
    public GameObject blinding;
    public GameObject annoying;
    public GameObject tanking;
    public GameObject rotating;
    public GameObject rankPanel;

    internal GameObject attractedWeapon;
    protected sAbilities myAbility;

    internal bool attracted;

    internal Transform debuff;

    private bool counterRecharging;
    private bool projectileRecharging;
    private bool specialRecharging;

    internal float powerGaugeValue;

    protected bool iAmBlinded;

    internal bool warning;


    protected new void Start_()
    {
        abilities = new Dictionary<ePlayer, sAbilities>()
        {
            { ePlayer.Bard, new sAbilities(homing, annoying) },
            { ePlayer.EarthRestorer, new sAbilities(bouncing, rotating) },
            { ePlayer.Flapper, new sAbilities(bouncing, tanking) },
            { ePlayer.Hypogeum, new sAbilities(homing, tanking) },
            { ePlayer.Imps, new sAbilities(trishot, annoying) },
            { ePlayer.Kiddo, new sAbilities(null, rotating, attracting) },
            { ePlayer.Politician, new sAbilities(trishot, blinding) },
            { ePlayer.Steamdunker, new sAbilities(null, blinding, attracting) },
        };

        // testing attracting
        //abilities = new Dictionary<ePlayer, sAbilities>()
        //{
        //    { ePlayer.Bard, new sAbilities(null, tanking, attracting) },
        //    { ePlayer.EarthRestorer, new sAbilities(trishot, tanking) },
        //    { ePlayer.Flapper, new sAbilities(trishot, tanking) },
        //    { ePlayer.Hypogeum, new sAbilities(trishot, tanking) },
        //    { ePlayer.Imps, new sAbilities(trishot, tanking) },
        //    { ePlayer.Kiddo, new sAbilities(trishot, tanking) },
        //    { ePlayer.Politician, new sAbilities(trishot, tanking) },
        //    { ePlayer.Steamdunker, new sAbilities(trishot, tanking) },
        //};

        myAbility = abilities[playerType];
        debuff = kartNormal.transform.Find("Debuff");

        base.Start_();
    }

    protected new void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        if (Paused)
            return;

        var input = "P" + playerNumber;
        powerGaugeValue += regenSpeed * (1 + GameState.Instance.getScoreBiasBonus(playerName)) * Time.deltaTime;

        if (powerGaugeValue > 1f)
            powerGaugeValue = 1f;

        if (canUseCounter())
            if (GB.GetButtonDown(input + "Counter") || GB.GetAxis(input + "Counter") != 0)
                Counter();

        if (canUseProjectile())
            if (GB.GetButtonDown(input + "Projectile"))
            {
                if (Input.GetAxis(input + "Vertical") < 0)
                    Projectile(rearSpawnpoint);
                else
                    Projectile(frontSpawnpoint);
            }

        if (canUseSpecial())
            if (GB.GetButtonDown(input + "Special") || GB.GetAxis(input + "Special") != 0)
                Special();

        if (GB.GetButtonDown(input + "MenuA") && rankPanel)
            rankPanel.SetActive(!rankPanel.activeSelf);

        base.Update_(xAxis, jumpBDown, jumpBUp);
    }

    protected void Counter()
    {
        if (canUseCounter())
        {
            Instantiate(counter, transform);

            powerGaugeValue -= 0.25f;
            counterRecharging = true;
            StartCoroutine(CounterCooldown());
        }
    }

    internal void Attractor(Transform spawnPoint)
    {
        if (canUseAttractor())
        {
            Instantiate(myAbility.myAttractor, spawnPoint.position, spawnPoint.rotation, transform);

            myAbility.InitAttractor();

            attracted = false;
            powerGaugeValue -= 0.5f;
            projectileRecharging = true;
            StartCoroutine(ProjectileCooldown());
        }
    }

    internal void Projectile(Transform spawnPoint)
    {
        if (canUseProjectile())
        {
            var weapon = attractedWeapon ?? myAbility.myProjectile;

            Instantiate(weapon, spawnPoint.position, spawnPoint.rotation, transform);

            if (weapon.Equals(attractedWeapon))
            {
                attractedWeapon = null;
            }
            else if (weapon.Equals(myAbility.myProjectile))
            {
                myAbility.InitProjectile();

                powerGaugeValue -= 0.5f;
                projectileRecharging = true;
                StartCoroutine(ProjectileCooldown());
            }
        }
    }

    protected void Special()
    {
        if (canUseSpecial())
        {
            myAbility.InitSpecial();

            Instantiate(myAbility.mySpecial, transform);

            powerGaugeValue -= 0.75f;
            specialRecharging = true;
            StartCoroutine(SpecialCooldown());
        }
    }

    internal bool canUseCounter() =>
        powerGaugeValue >= 0.25f &&
        !counterRecharging;

    internal bool canUseAttractor() =>
        myAbility.myAttractor &&
        !myAbility.myAttractor_inAction &&
        powerGaugeValue >= 0.5f &&
        !projectileRecharging;

    internal bool canUseProjectile() =>
        (myAbility.myProjectile || attractedWeapon) &&
        !myAbility.myProjectile_inAction &&
        powerGaugeValue >= 0.5f &&
        !projectileRecharging;

    internal bool canUseSpecial() =>
        !myAbility.mySpecial_inAction &&
        powerGaugeValue >= 0.75f &&
        !specialRecharging;

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

    internal void blindMe(bool blind)
    {
        iAmBlinded = blind;

        debuff.Find("Blinded").gameObject.SetActive(blind);

        Vignette v;
        if (postProfile.TryGetSettings(out v))
            v.enabled.Override(blind);
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