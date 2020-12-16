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

    public Image warningImage;
    public GameObject panelRankings, blinding, panelPause;
    public GameObject shieldIcon, powerIcon;
    public Sprite shieldAct, shieldInact, powerAct, powerInact;
    public GameObject shieldUI, powerUI;
    public ParticleSystem particles;

    private bool particlesPlayed = false, oldCanUseCounter, oldCanUseProj, oldCanUseSpecial;
    public AudioSource abilityReady, specialReady;

    private bool started;

    private void Start()
    {
        var loading = GameObject.Find("Loading");
        if(!loading)
            Start_();
    }

    public void Start_()
    {
        StartCoroutine(delayedStart());
    }

    private void Update()
    {
        /*if (started)
        {
            var newCanUseShield = kartController.canUseShield();
            var newCanUsePower = kartController.canUsePower();

            var shieldReady = shieldUI.transform.GetChild(0).gameObject;
            var powerReady = powerUI.transform.GetChild(0).gameObject;

            var shieldUsed = shieldUI.transform.GetChild(1).gameObject;
            var powerUsed = powerUI.transform.GetChild(1).gameObject;

            if (shieldReady.activeInHierarchy)
                shieldUsed.SetActive(kartController.powerRecharging);

            if (powerReady.activeInHierarchy)
                powerUsed.SetActive(kartController.powerRecharging);

            shieldReady.SetActive(newCanUseShield);
            powerReady.SetActive(newCanUsePower);

            shieldIcon.GetComponent<Image>().sprite = newCanUseShield ? shieldAct : shieldInact;
            powerIcon.GetComponent<Image>().sprite = newCanUsePower ? powerAct : powerInact;

            oldCanUseCounter = kartController.canUseShield();
            oldCanUseProj = kartController.canUsePower();

            warningImage.enabled = kartController.warning;

            blinding.SetActive(kartController.iAmBlinded);
        }*/
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

}