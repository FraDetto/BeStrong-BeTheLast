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
    public Image warningImage;
    public GameObject driftHeatingFill, panelRankings, blinding;
    public GameObject counterIcon, projectileIcon, specialIcon;
    public Sprite shieldAct, shieldInact, projectileAct, projectileInact, specialAct, specialInact;
    public GameObject counterUI, projectileUI, specialUI;
    public ParticleSystem particles;
    public Material blindMat;
    public float maxDissolveAmount=0.5f;
    public float dissolveAmount=0f;

    private bool particlesPlayed = false, oldCanUseCounter, oldCanUseProj, oldCanUseSpecial;
    public AudioSource abilityReady, specialReady;

    //private Color heatedColor = new Color32(242, 66, 66, 255);
    //private Color coldColor = new Color32(0, 57, 171, 255);

    private bool started;


    private void Start()
    {
        StartCoroutine(delayedStart());
    }

    private void Update()
    {
        if (started)
        {
            //Color driftHeatingFill_Color;

            float driftValue = kartController.driftHeatingValue;
            float driftValueAdjusted = driftValue * 0.7f + 0.3f;

            driftHeating.value = driftValueAdjusted;
            powerGauge.value = kartController.powerGaugeValue;

            /* if (driftValue > 0.7)
                 driftHeatingFill_Color = heatedColor;
             else if (driftValue < 0.3)
                 driftHeatingFill_Color = coldColor;
             else
                 driftHeatingFill_Color = Color.Lerp(coldColor, heatedColor, (driftValue - 0.3f) * 2.5f);

             driftHeatingFill.GetComponent<Image>().color = driftHeatingFill_Color; */

            driftHeatingFill.transform.GetChild(0).gameObject.SetActive(kartController.driftCooldown);


            var newCanUseCounter = kartController.canUseCounter();
            var newCanUseProj = kartController.canUseProjectile() || kartController.canUseAttractor();
            var newCanUseSpecial = kartController.canUseSpecial();

            /*if(newCanUseCounter && newCanUseCounter != oldCanUseCounter)
                counterUI.transform.GetChild(0).gameObject.SetActive(true);

            if (newCanUseProj && newCanUseProj != oldCanUseProj)
                projectileUI.transform.GetChild(0).gameObject.SetActive(true);

            if (newCanUseSpecial && newCanUseSpecial != oldCanUseSpecial)
                specialUI.transform.GetChild(0).gameObject.SetActive(true);*/
            
            counterUI.transform.GetChild(0).gameObject.SetActive(newCanUseCounter);
            projectileUI.transform.GetChild(0).gameObject.SetActive(newCanUseProj);
            specialUI.transform.GetChild(0).gameObject.SetActive(newCanUseSpecial); 

            counterIcon.GetComponent<Image>().sprite = newCanUseCounter ? shieldAct : shieldInact;
            projectileIcon.GetComponent<Image>().sprite = newCanUseProj ? projectileAct : projectileInact;
            specialIcon.GetComponent<Image>().sprite = newCanUseSpecial ? specialAct : specialInact;

            oldCanUseCounter = kartController.canUseCounter();
            oldCanUseProj = kartController.canUseProjectile();
            oldCanUseSpecial = kartController.canUseSpecial();

            warningImage.enabled = kartController.warning;

            blinding.SetActive(kartController.iAmBlinded);

            /* Non funziona come dovrebbe. 

            var dissolveDiff = maxDissolveAmount - dissolveAmount;
            blindMat.SetFloat("_DissolvePower", kartController.EffectDistributionFormula(dissolveAmount, dissolveDiff));

            blinding.GetComponent<Image>().material = blindMat;

            */
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
        counterUI.transform.GetChild(0).gameObject.SetActive(false);
        projectileUI.transform.GetChild(0).gameObject.SetActive(false);
        specialUI.transform.GetChild(0).gameObject.SetActive(false);
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