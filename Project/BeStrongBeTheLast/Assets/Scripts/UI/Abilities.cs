/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Maione Michele
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : PausableMonoBehaviour
{

    public KartController kartController;

    public GameObject panelRankings, blinding, panelPause;
    public GameObject shieldIcon, powerIcon;
    public Sprite shieldAct;
    public GameObject shieldUI, powerUI;
    public ParticleSystem particles;
    public Sprite[] powers;
    public float slotTimer = 0.25f;
    private int powerIndex, prevIndex;
    public GameObject cooldown;
    private float seconds = 5f;

    private bool particlesPlayed = false, oldCanUseCounter, oldCanUseProj, oldCanUseSpecial;
    public AudioSource abilityReady, specialReady;

    private Color grayed;

    private bool started;

    private void Start()
    {
        var loading = GameObject.Find("Loading");
        if(!loading)
            Start_();
    }

    public void Start_()
    {
        grayed = new Color(0.1f, 0.1f, 0.1f);
        StartCoroutine(delayedStart());
        StartCoroutine(SlotMachine());
        StartCoroutine(Cooldown());
    }

    private void Update()
    {
        if (started)
        {
            var newCanUseShield = kartController.canUseShield();
            var newCanUsePower = kartController.canUsePower();

            var shieldReady = shieldUI.transform.GetChild(0).gameObject;
            var powerReady = powerUI.transform.GetChild(0).gameObject;

            var shieldUsed = shieldUI.transform.GetChild(1).gameObject;
            var powerUsed = powerUI.transform.GetChild(1).gameObject;

            shieldReady.SetActive(newCanUseShield);
            powerReady.SetActive(newCanUsePower);

            shieldIcon.GetComponent<Image>().color = newCanUseShield ? Color.white : grayed;

            powerIcon.GetComponent<Image>().color = newCanUseShield ? Color.white : grayed;

            oldCanUseCounter = kartController.canUseShield();
            oldCanUseProj = kartController.canUsePower();

            blinding.SetActive(kartController.iAmBlinded);
        }
    }

    private void PlayParticle(GameObject abilityUI, AudioSource sound)
    {
        disableParticleCameras();

        if (!particlesPlayed)
        {
            if (particles)
            {
                particles.Stop();
                particles.Play();
            }

            sound.Play();

            particlesPlayed = true;
            StartCoroutine(stopParticles());
        }
    }

    private void disableParticleCameras()
    {
        shieldUI.transform.GetChild(0).gameObject.SetActive(false);
        powerUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator delayedStart()
    {
        while (!kartController.started)
            yield return null;

        kartController.rankPanel = panelRankings;
        kartController.pausePanel = panelPause;
        started = true;
    }

    private IEnumerator stopParticles()
    {
        yield return new WaitForSeconds(3);

        if (particles)
            particles.Stop();

        particlesPlayed = false;
        disableParticleCameras();
    }

    private IEnumerator SlotMachine()
    {
        while(true)
        {
            if(kartController.slotMachine)
            {
                do
                    powerIndex = Random.Range(0, 6);
                while(powerIndex == prevIndex);

                prevIndex = powerIndex;

                powerIcon.GetComponent<Image>().enabled = true;
                powerIcon.GetComponent<Image>().sprite = powers[powerIndex];
                yield return new WaitForSeconds(slotTimer);
            }

            if(kartController.PowerEquipped())
            {
                powerIcon.GetComponent<Image>().sprite = kartController.equippedPower.GetComponent<aAbilitiesBehaviour>().icon;
                yield return null;
            }
            else
            {
                powerIcon.GetComponent<Image>().enabled = false;
                yield return null;
            }
        } 
    }

    private IEnumerator Cooldown()
    {
        while(true)
        {
            if(kartController.powerRecharging)
            {
                if(cooldown.GetComponent<Text>().text.Equals(""))
                    seconds = 5;

                cooldown.GetComponent<Text>().text = seconds.ToString();
                seconds -= 1;
                yield return new WaitForSeconds(1f);
            }
            else
            {
                cooldown.GetComponent<Text>().text = "";
                yield return null;
            }
        }
    }
}