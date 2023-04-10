using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushExplosion : MonoBehaviour
{

    public Collider[] hitColliders;



    public void Active()
    {

        Vector3 explosive = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosive, 15);//range


        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                 Vector3 direction = hit.transform.position - transform.position;
                 Vector3 explosiveForce = new Vector3(direction.x * Random.Range(1f, 2f), direction.y + 4 * Random.Range(1f, 2f), direction.z * Random.Range(1f, 2f)); //verticality
                 rb.AddForce(explosiveForce * 4, ForceMode.Impulse);      //force


            }
            if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }
            if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push();  }
            if (hit.transform.CompareTag("Tank")) { hit.gameObject.GetComponent<Tank_Manager>().Push();  }
        }

        Destroy(this.gameObject);
    }



}
