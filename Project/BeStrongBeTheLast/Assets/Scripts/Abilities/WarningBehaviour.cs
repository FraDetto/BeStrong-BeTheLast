/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors: Michele Maione
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using System.Collections;
using UnityEngine;

public class WarningBehaviour : aCollisionManager
{
    public GB.ELato lato;

    private bool blinking;


    private void OnTriggerStay(Collider other)
    {
        if (GB.CompareORTags(other, "Projectile"))
        {
            var R = transform.root.GetChild(0);
            var kartController = R.GetComponent<KartController>();

            if (kartController.canUseProjectile())
                switch (lato)
                {
                    case GB.ELato.Avanti:
                        kartController.Projectile(kartController.frontSpawnpoint);
                        break;
                    case GB.ELato.Dietro:
                        kartController.Projectile(kartController.rearSpawnpoint);
                        break;
                }

            if (!blinking && other.transform.forward != transform.forward)
                if (
                    CheckIs<SingleShotBehaviour>(other, "SingleShot") ||
                    CheckIs<HomingBehaviour>(other, "Homing") ||
                    CheckIs<BouncingBehaviour>(other, "Bouncing")
                )
                {
                    blinking = true;
                    kartController.warning = true;

                    StartCoroutine(Blink(kartController));
                }
        }
    }

    bool CheckIs<BehaviourT>(Collider other, string Name) where BehaviourT : aAbilitiesBehaviour =>
       other.transform.name.StartsWith(Name) && !other.GetComponent<BehaviourT>().user.Equals(transform.root.gameObject);

    IEnumerator Blink(KartController kartController)
    {
        yield return new WaitForSeconds(0.5f);
        kartController.warning = false;
        blinking = false;
    }

}