using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrishotBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    private GameObject centralShot;
    private GameObject rightShot;
    private GameObject leftShot;

    private void Start()
    {
        centralShot = transform.GetChild(0).gameObject;
        rightShot = transform.GetChild(1).gameObject;
        leftShot = transform.GetChild(2).gameObject;
        StartCoroutine(Lifetime());
    }

    // Update is called once per frame
    void Update()
    {
        if(centralShot != null)
            centralShot.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(rightShot != null)
            rightShot.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(leftShot != null)
            leftShot.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(centralShot == null && rightShot == null && leftShot == null)
            Destroy(this.gameObject);
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
