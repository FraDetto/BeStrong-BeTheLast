/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WarningBehaviour : aCollisionManager
{
    public Text warningText;
    public GB.ELato lato;

    private bool blinking;


    private void OnTriggerStay(Collider other)
    {
        onCollisionWithTags(other, (kartController) =>
        {
            if (kartController.myAbility.myProjectile.Equals(kartController.attracting))
                switch (lato)
                {
                    case GB.ELato.Avanti:
                        kartController.Projectile(kartController.frontSpawnpoint);
                        break;
                    case GB.ELato.Dietro:
                        kartController.Projectile(kartController.rearSpawnpoint);
                        break;
                }

            if (warningText)
            {
                //Si lo so che fa cagare, si potrebbe sistemare con una bella classe astratta "Abilità" con il campo user, ma la pipeline di chiamate è così complessa ora che non mi sogno neanche di metterci mani.
                //Be my guest ;-)
                if ((other.transform.name.StartsWith("SingleShot") && !other.GetComponent<SingleShotBehaviour>().user.Equals(transform.root.gameObject))
                    || (other.name.StartsWith("Homing") && !other.GetComponent<HomingBehaviour>().user.Equals(transform.root.gameObject))
                    || (other.name.StartsWith("Bouncing") && !other.GetComponent<BouncingBehaviour>().user.Equals(transform.root.gameObject)))
                    if (other.transform.forward != transform.forward)
                        if (!blinking)
                        {
                            blinking = true;
                            warningText.enabled = true;

                            StartCoroutine(Blink());
                        }
            }
        }, "Projectile");
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.5f);
        warningText.enabled = false;
        blinking = false;
    }

}