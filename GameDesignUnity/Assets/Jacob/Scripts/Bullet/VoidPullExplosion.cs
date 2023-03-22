using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidPullExplosion : MonoBehaviour
{
    public float Range;
    public float Force;
    public float verticality;
    public Collider[] hitColliders;



    public void Active()
    {

        Vector3 explosive = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosive, Range*3);


        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 direction = hit.transform.position - transform.position;
                Vector3 explosiveForce = new Vector3(direction.x, direction.y, direction.z);
                rb.AddForce((explosiveForce * Force * 1.5f)*-1, ForceMode.Impulse);


            }
            if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }
            if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push(); }
        }

        Destroy(this.gameObject);
    }
}
