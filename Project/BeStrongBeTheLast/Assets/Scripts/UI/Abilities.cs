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

public class Abilities : MonoBehaviour
{

    public KartController kartController;

    public Slider powerGauge;
    public Text counterText;
    public Text selectedProjectileText;
    public Text selectedSpecialText;
    public Slider driftHeating;
    public Text warningText;
    public GameObject driftHeatingFill;

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
            float driftValue = kartController.driftHeatingValue;
            driftHeating.value = driftValue + 0.128f;
            powerGauge.value = kartController.powerGaugeValue;
            driftHeatingFill.GetComponent<Image>().color = Color.Lerp(coldColor, heatedColor, Mathf.Sqrt(driftValue));




            counterText.color = kartController.canUseCounter() ? enabledColor : disabledColor;
            selectedProjectileText.color = kartController.canUseProjectile() ? enabledColor : disabledColor;
            selectedSpecialText.color = kartController.canUseSpecial() ? enabledColor : disabledColor;

            selectedSpecialText.text = kartController.myAbility.selectedSpecial.name;

            if (kartController.myAbility.selectedProjectile)
                selectedProjectileText.text = kartController.myAbility.selectedProjectile.name;
            else if (kartController.myAbility.selectedAttractor)
                selectedProjectileText.text = kartController.myAbility.selectedAttractor.name;
        }
    }

    private IEnumerator delayedStart()
    {
        while (!kartController.started)
            yield return null;

        counterText.color = disabledColor;

        if (kartController.myAbility.selectedProjectile)
            selectedProjectileText.text = kartController.myAbility.selectedProjectile.name;
        else if (kartController.myAbility.selectedAttractor)
            selectedProjectileText.text = kartController.myAbility.selectedAttractor.name;

        selectedProjectileText.color = disabledColor;

        selectedSpecialText.text = kartController.myAbility.selectedSpecial.name;
        selectedProjectileText.color = disabledColor;

        kartController.GetComponentInChildren<WarningBehaviour>().warningText = warningText;

        started = true;
    }

}