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
    public Transform frontSpawnpoint;
    public Transform rearSpawnpoint;

    [SerializeField] private float powerCooldown = 5;

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

    public ePlayer playerType = ePlayer.Kiddo;

    [Header("Abilities - Types")]
    public GameObject shield;

    internal Transform debuff;

    internal bool powerRecharging;
    public GameObject equippedPower;

    internal bool iAmBlinded;

    internal bool warning;

    internal bool slotMachine;


    protected new void Start_()
    {
        debuff = kartNormal.transform.Find("Debuff");

        base.Start_();
    }

    protected new void Update_(float xAxis, bool jumpBDown, bool jumpBUp)
    {
        if (Paused)
            return;

        base.Update_(xAxis, jumpBDown, jumpBUp);
    }


    protected void Shield()
    {
        if (canUseShield())
        {
            Instantiate(shield, transform);
            powerRecharging = true;
            StartCoroutine(PowerCooldown());
        }
    }

    internal void Power(Transform spawnPoint)
    {
        if (canUsePower())
        {
            if(equippedPower.GetComponent<aAbilitiesBehaviour>().needsAim)
                Instantiate(equippedPower, spawnPoint.position, spawnPoint.rotation, transform);
            else
                Instantiate(equippedPower, transform);

            equippedPower = null;

            powerRecharging = true;
            StartCoroutine(PowerCooldown());
        }
    }

    internal bool canUseShield() =>
        !powerRecharging;

    internal bool canUsePower() =>
        PowerEquipped() && 
        !powerRecharging;

    internal bool PowerEquipped() =>
        equippedPower != null;

    IEnumerator PowerCooldown()
    {
        yield return new WaitForSeconds(powerCooldown);
        powerRecharging = false;
    }

    internal void blindMe(bool blind)
    {
        iAmBlinded = blind;

        debuff.Find("Blinded").gameObject.SetActive(blind);
    }
}