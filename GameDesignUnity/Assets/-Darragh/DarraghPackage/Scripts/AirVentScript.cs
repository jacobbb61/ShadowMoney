using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVentScript : MonoBehaviour
{
    public float force;
    [SerializeField] public Vector3 areaEffect;
    private bool canVent = true;

    public void Start()
    {
        canVent = true;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (canVent)
        {   
            Debug.Log("Triggered1");
            Vector3 explosive = transform.position;
            Debug.Log("Triggered2");

            Collider[] collider = Physics.OverlapBox(explosive, areaEffect);
            Debug.Log("Triggered3");

            foreach (Collider hit in collider)
            {
                Debug.Log(hit.name);
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb)
                {
                    //Applying Force
                    //rb.AddExplosionForce(power,explosive,areaEffect,explosiveLift);
                    rb.AddForce(Vector3.up * force, ForceMode.Impulse);
                    Debug.Log("Triggered5");

                    if (rb.TryGetComponent(out PlayerManager player))
                    {
                        player.AirVent(force);
                    }
                }
            }

            canVent = false;
            StartCoroutine(EnableVent());
        }

    }

    public IEnumerator EnableVent()
    {
        yield return new WaitForSeconds(1);
        canVent = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, areaEffect);
    }

}
