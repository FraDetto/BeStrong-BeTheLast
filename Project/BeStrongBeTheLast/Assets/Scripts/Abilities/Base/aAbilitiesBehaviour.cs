/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Assets.Scripts.Obstacles.Base;
using UnityEngine;

public class aAbilitiesBehaviour : aCollisionManager
{

    protected RaycastHit hit;
    internal GameObject user;

    protected KartController kartController;


    protected void Start_() =>
        kartController = GB.FindComponentInDadWithName<KartController>(transform, "Controller");

    protected void KillMe()
    {
        if (kartController)
            if (IsSame(kartController.myAbility.myProjectile))
                kartController.myAbility.myProjectile_inAction = false;
            else if (IsSame(kartController.myAbility.mySpecial))
                kartController.myAbility.mySpecial_inAction = false;
            else if (IsSame(kartController.myAbility.myAttractor))
                kartController.myAbility.myAttractor_inAction = false;

        //if (enabled)
        Destroy(gameObject);
    }

    bool IsSame(GameObject o) =>
       o && (name.StartsWith(o.name) || transform.root.name.StartsWith(o.name));

}