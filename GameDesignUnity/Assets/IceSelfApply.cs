using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class IceSelfApply : MonoBehaviour
{

    public GameObject Particles;
    public GameObject emptyExplosion;
    private AudioSource source;
    
    [SerializeField] private float areaEffect;


    private void Start()
    {
        //EM = GetComponent<Effects_Manager>();
        Explode();
    }

    public void Explode()
    {
        GameObject NP = Instantiate(Particles, transform.position, transform.rotation);
        Destroy(NP, 1f);
        //source.Play();

        Vector3 explosive = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosive, areaEffect);


        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                if (hit.TryGetComponent(out Effects_Manager EM))
                {
                    EM.IsFrozen = true;
                }
            }
        }

        Debug.Log("Exploded");
        Destroy(emptyExplosion);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, areaEffect);
    }

}
