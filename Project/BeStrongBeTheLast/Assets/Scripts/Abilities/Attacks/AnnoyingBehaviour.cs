/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;

public class AnnoyingBehaviour : aAbilitiesBehaviour
{
    public float maxAnnoyingAmount = 1f; //Here 1 means that no annoying will be applied to your steer...
    public float annoyingAmount = 0.5f; //...In the worst scenario you will get 0.5 which means half your steering axis


    void Start()
    {
        Start_();

        // user = transform.parent.gameObject;

        AnnoyCars(allCars, annoyingAmount, true);

        StartCoroutine(Lifetime());
    }

    private void AnnoyCars(List<KartController> cars, float annoyingAmount, bool annoyed)
    {
        var annoyingDiff = maxAnnoyingAmount - annoyingAmount;
        foreach (var car in cars)
            if (car != kartController)
                if (annoyed)
                    car.annoyMe(car.EffectDistributionFormula(annoyingAmount, annoyingDiff), annoyed);
                else
                    car.annoyMe(1f, annoyed);
    }

    protected override void LifeTimeElapsed() =>
        AnnoyCars(allCars, annoyingAmount, false);

}