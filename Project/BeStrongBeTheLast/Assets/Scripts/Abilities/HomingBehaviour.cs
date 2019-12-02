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

public class HomingBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask roadMask;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float accelerationFromShot;

    private GameObject target;
    private GameObject user;

    private int colliderCounter;


    void Start()
    {
        StartCoroutine(Lifetime());
    }

    void Update()
    {
        if (target != null)
            transform.LookAt(target.transform);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, roadMask))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.up, hit.normal, 1f), -transform.forward);
            transform.Rotate(Vector3.right, 90f);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
        {
            if (colliderCounter == 0 && target == null)
            {
                if (user == null)
                    user = other.gameObject;
                else if (!other.gameObject.Equals(user))
                    target = other.gameObject;
            }
            else if (colliderCounter == 1 && user != null && !other.gameObject.Equals(user))
            {
                var kartController = other.transform.parent.GetComponentInChildren<aKartController>();
                kartController.Accelerate(accelerationFromShot);
                Destroy(this.gameObject);
            }
            colliderCounter += 1;
        }
        else if(other.gameObject.layer.Equals(12))
        {
            if(colliderCounter == 1)
                Destroy(this.gameObject);

            colliderCounter += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU") || other.gameObject.layer.Equals(12))
            colliderCounter -= 1;
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(7.5f);
        if (enabled)
            Destroy(this.gameObject);
    }

}