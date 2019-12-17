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

    public Slider powerGauge;
    public Slider driftHeating;
    public Text warningText;
    public GameObject driftHeatingFill, panelRankings;
    public GameObject counterIcon, projectileIcon, specialIcon;
    public Sprite shieldAct, shieldInact, projectileAct, projectileInact, specialAct, specialInact;
    public GameObject cameraParticles1, cameraParticles2, cameraParticles3;
    public ParticleSystem particles;

    private bool particlesPlayed = false, oldCanUseCounter, oldCanUseProj, oldCanUseSpecial;
    public AudioSource abilityReady, specialReady;

    private Color disabledColor = Color.red;
    private Color enabledColor = Color.yellow;
    private Color heatedColor = new Color32(242, 66, 66, 255);
    private Color coldColor = new Color32(0, 57, 171, 255);

    private bool started;


    private void Start()
    {
        StartCoroutine(delayedStart());
    }

    private void Update()
    {
        if (started)
        {
            Color driftHeatingFill_Color;

            float driftValue = kartController.driftHeatingValue;
            float driftValueAdjusted = driftValue * 0.7f + 0.3f;

            driftHeating.value = driftValueAdjusted;
            powerGauge.value = kartController.powerGaugeValue;

            if (driftValue > 0.7)
                driftHeatingFill_Color = heatedColor;
            else if (driftValue < 0.3)
                driftHeatingFill_Color = coldColor;
            else
                driftHeatingFill_Color = Color.Lerp(coldColor, heatedColor, (driftValue - 0.3f) * 2.5f);

            driftHeatingFill.GetComponent<Image>().color = driftHeatingFill_Color;

            if (kartController.canUseCounter() != oldCanUseCounter && kartController.canUseCounter())
                PlayParticle(cameraParticles1);

            if (kartController.canUseProjectile() != oldCanUseProj && kartController.canUseProjectile())
                PlayParticle(cameraParticles2);

            if (kartController.canUseSpecial() != oldCanUseSpecial && kartController.canUseSpecial())
                PlayParticle(cameraParticles3);

            oldCanUseCounter = kartController.canUseCounter();
            oldCanUseProj = kartController.canUseProjectile();
            oldCanUseSpecial = kartController.canUseSpecial();

            counterIcon.GetComponent<Image>().sprite = kartController.canUseCounter() ? shieldAct : shieldInact;
            projectileIcon.GetComponent<Image>().sprite = kartController.canUseProjectile() ? projectileAct : projectileInact;
            specialIcon.GetComponent<Image>().sprite = kartController.canUseSpecial() ? specialAct : specialInact;

            /*
            selectedSpecialText.text = kartController.myAbility.selectedSpecial.name;

            if (kartController.myAbility.selectedProjectile)
                selectedProjectileText.text = kartController.myAbility.selectedProjectile.name;
            else if (kartController.myAbility.selectedAttractor)
                selectedProjectileText.text = kartController.myAbility.selectedAttractor.name;*/
        }
    }

    private void PlayParticle(GameObject cameraParticles)
    {
        disableParticleCameras();
        cameraParticles.SetActive(true);

        if (!particlesPlayed)
        {
            if (particles)
            {
                particles.Stop();
                particles.Play();
            }

            abilityReady.Play();

            particlesPlayed = true;
            StartCoroutine(stopParticles());
        }
    }

    private void disableParticleCameras()
    {
        cameraParticles1.SetActive(false);
        cameraParticles2.SetActive(false);
        cameraParticles3.SetActive(false);
    }

    private IEnumerator delayedStart()
    {
        while (!kartController.started)
            yield return null;

        /*
        counterText.color = disabledColor;

        if (kartController.myAbility.selectedProjectile)
            selectedProjectileText.text = kartController.myAbility.selectedProjectile.name;
        else if (kartController.myAbility.selectedAttractor)
            selectedProjectileText.text = kartController.myAbility.selectedAttractor.name;

        selectedProjectileText.color = disabledColor;

        selectedSpecialText.text = kartController.myAbility.selectedSpecial.name;
        selectedProjectileText.color = disabledColor;
        */
        kartController.GetComponentInChildren<WarningBehaviour>().warningText = warningText;
        kartController.rankPanel = panelRankings;
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