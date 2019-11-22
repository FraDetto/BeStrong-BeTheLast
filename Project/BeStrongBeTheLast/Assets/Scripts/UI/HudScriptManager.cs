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

public class HudScriptManager : MonoBehaviour
{

    //To change the speed in the speedometer
    private Text speedText;

    //To manage the team Health bar
    private Slider healthBar;

    private GameObject controlsClosed, controlsOpen, win, loss;

    private bool GameControlsVisibile = false;
    private bool HoAvutoIlTempoDiLeggere = true;

    internal GeneralCar generalCar;

    void Start()
    {
        //HUD            
        controlsClosed = GameObject.Find("ControlsClosed");
        controlsOpen = GameObject.Find("ControlsOpen");
        controlsOpen.SetActive(GameControlsVisibile);

        speedText = GameObject.FindGameObjectWithTag("SpeedText").GetComponent<Text>();
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

        win = GameObject.FindGameObjectWithTag("Win");
        loss = GameObject.FindGameObjectWithTag("Loss");

        win.SetActive(false);
        loss.SetActive(false);

        setSpeed(0);
        setHealth(0, 0, 0);
    }

    void Update()
    {
        //HUD
        if (Input.GetKey(KeyCode.F1) && HoAvutoIlTempoDiLeggere)
        {
            HoAvutoIlTempoDiLeggere = false;

            controlsOpen.SetActive(!GameControlsVisibile);
            controlsClosed.SetActive(GameControlsVisibile);
            GameControlsVisibile = !GameControlsVisibile;

            StartCoroutine(TempoDiLettura());
        }

        setValues();
    }

    private IEnumerator TempoDiLettura()
    {
        yield return new WaitForSeconds(0.999f);
        HoAvutoIlTempoDiLeggere = true;
    }

    private void setValues()
    {
        var l = 0;
        var s = generalCar?.actualSpeed ?? 0;

        setHealth(0, l, l);
        setSpeed(s);
    }

    private void setHealth(int min, float max, float value)
    {
        healthBar.minValue = min;

        if (max > healthBar.maxValue)
            healthBar.maxValue = max;

        healthBar.value = (loss.active ? 0 : value);
    }

    private void setSpeed(float value)
    {
        //value: m/s
        var realspeed = System.Convert.ToInt16(GB.ms_to_mph(value));

        if (realspeed == 0 || realspeed % 2 == 0)
            speedText.text = realspeed.ToString("0");
    }


}