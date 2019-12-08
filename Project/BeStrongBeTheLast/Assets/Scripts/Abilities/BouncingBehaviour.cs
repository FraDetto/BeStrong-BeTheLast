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

public class BouncingBehaviour : aAbilitiesBehaviour
{

    [SerializeField]
    private LayerMask wallMask;

    public float speed;

    private GameObject rolling;

    RaycastHit hit;

    internal GameObject user;

    [SerializeField]
    private float lengthTimeInSeconds = 10f;

    void Start()
    {
        Start_();

        rolling = transform.GetChild(0).gameObject;
        StartCoroutine(Lifetime());

        user = transform.root.gameObject;
        transform.parent = null;
    }

    void Update()
    {
        if (rolling == null)
        {
            Destroy(gameObject);
        }
        else
        {
            var p1 = rolling.transform.position + rolling.GetComponent<CapsuleCollider>().center + Vector3.up * -rolling.GetComponent<CapsuleCollider>().height * 0.5f;
            var p2 = p1 + Vector3.up * rolling.GetComponent<CapsuleCollider>().height;

            if (Physics.CapsuleCast(p1, p2, rolling.GetComponent<CapsuleCollider>().radius, transform.TransformDirection(Vector3.forward), out hit, 1f, wallMask))
                transform.forward = Vector3.Reflect(transform.forward, hit.normal);

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lengthTimeInSeconds);

        if (enabled)
            Destroy(gameObject);
    }

}