using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrishotBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
