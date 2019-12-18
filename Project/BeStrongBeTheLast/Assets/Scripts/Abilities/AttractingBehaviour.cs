/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Riccardo Lombardi
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using UnityEngine;

public class AttractingBehaviour : aAbilitiesBehaviour
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private float shrinkingSpeed;

    private GameObject projectile;

    private bool attracting;

    [SerializeField]
    private float lengthTimeInSeconds = 5f;


    void Start()
    {
        Start_();
        StartCoroutine(Lifetime());
    }

    private void Update()
    {
        if (attracting && !kartController.attracted && projectile)
        {
            projectile.transform.LookAt(transform);
            projectile.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            projectile.transform.localScale -= Vector3.one * shrinkingSpeed * Time.deltaTime;

            if (Vector3.Distance(projectile.transform.position, transform.position) <= 2f)
            {
                Destroy(projectile);
                kartController.attracted = true;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) =>
        Colliding(other);

    private void Colliding(Collider other)
    {
        if (!attracting && other.CompareTag("Projectile"))
        {
            if (other.name.StartsWith("Homing"))
            {
                other.GetComponent<HomingBehaviour>().enabled = false;
                kartController.attractedWeapon = kartController.homing;
            }
            else if (other.name.StartsWith("Trishot"))
            {
                other.GetComponent<TrishotBehaviour>().enabled = false;

                foreach (var singleShot in other.GetComponentsInChildren<SingleShotBehaviour>())
                    singleShot.enabled = false;

                kartController.attractedWeapon = kartController.trishot;
            }
            else if (other.name.StartsWith("Bouncing"))
            {
                other.GetComponent<BouncingBehaviour>().enabled = false;
                kartController.attractedWeapon = kartController.bouncing;
            }

            projectile = other.gameObject;
            attracting = true;
        }
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lengthTimeInSeconds);
        Destroy(gameObject);
    }

}