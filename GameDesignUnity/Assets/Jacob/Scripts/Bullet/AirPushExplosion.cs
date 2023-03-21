using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushExplosion : MonoBehaviour
{
    public float Range;
    public float Force;
    public float verticality;
    public Collider[] hitColliders;



    public void Active()
    {

        Vector3 explosive = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosive, Range);


        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                 Vector3 direction = hit.transform.position - transform.position;
                 Vector3 explosiveForce = new Vector3(direction.x * Random.Range(1f, 2f), direction.y + verticality * Random.Range(1f, 2f), direction.z * Random.Range(1f, 2f));
                 rb.AddForce(explosiveForce * Force * 1.5f, ForceMode.Impulse);      


            }
            if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }

            if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push();  }
        }

        Destroy(this.gameObject);
    }



}
