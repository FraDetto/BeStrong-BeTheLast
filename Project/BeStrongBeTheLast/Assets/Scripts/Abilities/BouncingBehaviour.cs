using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallMask;

    public float speed;

    private GameObject rolling;

    // Start is called before the first frame update
    void Start()
    {
        rolling = transform.GetChild(0).gameObject;
        StartCoroutine(Lifetime());
    }

    // Update is called once per frame
    void Update()
    {
        if(rolling != null)
        {
            RaycastHit hit;
            Vector3 p1 = rolling.transform.position + rolling.GetComponent<CapsuleCollider>().center + Vector3.up * -rolling.GetComponent<CapsuleCollider>().height *0.5f;
            Vector3 p2 = p1 + Vector3.up * rolling.GetComponent<CapsuleCollider>().height /**1.5f*/;
            if(Physics.CapsuleCast(p1, p2, rolling.GetComponent<CapsuleCollider>().radius /**1.5f*/, Vector3.forward, out hit, 1f, wallMask))
            {
                transform.forward = Vector3.Reflect(transform.forward, hit.normal);
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }    
        else
            Destroy(this.gameObject);
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(10f);
        if(enabled)
            Destroy(this.gameObject);
    }
}
