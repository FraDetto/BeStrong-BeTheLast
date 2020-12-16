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
using System.Collections.Generic;
using UnityEngine;

public abstract class aAbilitiesBehaviour : aCollisionManager
{

    protected RaycastHit hit;
    // internal GameObject user;

    protected KartController kartController;
    protected List<KartController> players, bots, allCars;

    internal GameObject user;

    public float lengthTimeInSeconds = 5;
    public bool needsAim;

    public bool UseScorBiasBonus = true;


    protected void Start_()
    {
        kartController = GB.FindComponentInDadWithName<KartController>(transform, "Controller");

        players = GB.GetOnlyWithComponentWithTag<KartController>("Player");
        bots = GB.GetOnlyWithComponentWithTag<KartController>("CPU");

        allCars = new List<KartController>();
        allCars.AddRange(players);
        allCars.AddRange(bots);

        if (lengthTimeInSeconds > 0f)
            StartCoroutine(Lifetime());
    }

    protected abstract void LifeTimeElapsed();

    protected IEnumerator Lifetime()
    {
        var sec = lengthTimeInSeconds;

        if (UseScorBiasBonus && kartController)
            sec *= 1 + GameState.Instance.getScoreBiasBonus(kartController.PlayerName);

        yield return new WaitForSeconds(sec);

        LifeTimeElapsed();

        KillMe();
    }

    protected void KillMe()
    {
        /*RIC: Ho commentato sta roba perché dava fastidio al trishot. L'intento era quello di evitare che uno potesse usare due volte la stessa abilità, ma questo non è possibile se 
         * A) il regen del mana viene impostato alla velocità reale (0.01) e non valori di test;
         * B) il cooldown è sempre più alto della durata massima di una special (contando anche il bias dato dalla posizione);
         */

        /*if (kartController)
            if (IsSame(kartController.myAbility.myProjectile))
                kartController.myAbility.myProjectile_inAction = false;
            else if (IsSame(kartController.myAbility.mySpecial))
                kartController.myAbility.mySpecial_inAction = false;
            else if (IsSame(kartController.myAbility.myAttractor))
                kartController.myAbility.myAttractor_inAction = false; */

        //if (enabled)
        Destroy(gameObject);
    }

    bool IsSame(GameObject o) =>
       o && (name.StartsWith(o.name) || transform.root.name.StartsWith(o.name));

}