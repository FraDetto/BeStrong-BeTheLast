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

public class TankingBehaviour : MonoBehaviour
{
    private Rigidbody rigidbody_;
    private Transform normal;
    private Vector3 originalScale;

    [SerializeField]
    private float lengthTimeInSeconds = 15f;

    // Start is called before the first frame update
    void Start()
    {
        normal = transform.parent.GetChild(0);
        originalScale = normal.localScale;
        rigidbody_ = transform.root.GetComponentInChildren<Rigidbody>();
        normal.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 1.5f, originalScale.z * 1.5f);
        normal.Translate(Vector3.up * 0.25f);

        rigidbody_.mass *= 2;
        rigidbody_.drag += 1;

        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lengthTimeInSeconds);

        rigidbody_.mass /= 2;
        rigidbody_.drag -= 1;
        normal.localScale = originalScale;
        normal.Translate(Vector3.down * 0.25f);

        if (enabled)
            Destroy(gameObject);
    }

}