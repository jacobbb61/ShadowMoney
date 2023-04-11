using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour
{

    Rigidbody rb;
    Vector3 ExPos;

    public BoxCollider TriggerCollider;

    void Start()
    {
        ExPos = transform.position;
        int a = Random.Range(-1, 1);
        int b = Random.Range(-1, 1);
        int c = Random.Range(-1, 1);
        transform.position = new Vector3(transform.position.x + a, transform.position.y + b, transform.position.z + c);
     
        rb = GetComponent<Rigidbody>();


        Destroy(this, 0.1f);
        Destroy(gameObject, 20f);
    }



    private void Update()
    {
        
        if (rb != null)
            rb.AddExplosionForce(0.1f, ExPos, 1, 0.2f, ForceMode.Impulse);
    }

}
