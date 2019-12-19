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

public class CounterBehaviour : aAbilitiesBehaviour
{
    [SerializeField] private float lengthTimeInSeconds = 1f;

    public SphereCollider sphereCollider;

    internal float raggioDiAzione =>
        sphereCollider.radius;

    internal float diametroDiAzione =>
        raggioDiAzione * 2;

    private void Start()
    {
        var player = transform.root.GetComponentInChildren<KartController>().playerType;
        if (player.Equals(aBSBTLKart.ePlayer.EarthRestorer))
            transform.GetChild(0).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Bard))
            transform.GetChild(1).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Hypogeum))
            transform.GetChild(2).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Politician))
            transform.GetChild(3).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Flapper))
            transform.GetChild(4).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Steamdunker))
            transform.GetChild(5).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Imps))
            transform.GetChild(0).gameObject.SetActive(true);
        else if (player.Equals(aBSBTLKart.ePlayer.Kiddo))
            transform.GetChild(0).gameObject.SetActive(true);

        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CPU"))
            if (!other.transform.root.gameObject.Equals(transform.root.gameObject))
                other.transform.root.GetComponentInChildren<KartCollision>().countered = true;
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lengthTimeInSeconds);

        KillMe();
    }

}