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
            Vector3 explosive = transform.position;           
            Collider[] collider = Physics.OverlapBox(explosive, areaEffect);

            foreach (Collider hit in collider)
            {
                Debug.Log(hit.name);
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb)
                {
                    //Applying Force
                    //rb.AddExplosionForce(power,explosive,areaEffect,explosiveLift);
                    rb.AddForce(Vector3.up * force, ForceMode.Impulse);
                    

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
