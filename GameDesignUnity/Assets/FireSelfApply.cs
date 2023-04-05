using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireSelfApply : MonoBehaviour, IDamageable
{
    

    public GameObject explosion,emptyExplosion;
    private AudioSource source;


    public int explosionDamage = 5;
    public float force;
    public float verticality;
    [SerializeField] private float areaEffect;


    private void Start()
    {
        //EM = GetComponent<Effects_Manager>();
        Explode();
    }

    public void Explode()
    {
        //Instantiate(explosion, transform.position, quaternion.identity);
        //source.Play();
        
        Vector3 explosive = transform.position;
        
        Collider[] colliders = Physics.OverlapSphere(explosive, areaEffect);
        

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                //Applying Force
                Vector3 direction = hit.transform.position - transform.position;
                Vector3 explosiveForce = new Vector3(direction.x, direction.y + verticality * Random.Range(1f, 2f), direction.z);
                rb.AddForce(explosiveForce * force, ForceMode.Impulse);

                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(explosionDamage);
                }
                
                if (hit.TryGetComponent(out Effects_Manager EM))
                {
                    EM.IsBurning = true;
                }
 
            
                if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }
                if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push(); }
                if (hit.transform.CompareTag("Tank")) { hit.gameObject.GetComponent<Tank_Manager>().Push(); }
            }
        }
        Debug.Log("Exploded");
        Destroy(emptyExplosion);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, areaEffect);
    }
    
    public void TakeDamage(int amt)
    {
        StartCoroutine(TakeDamageDelay(amt));
    }
    
    private IEnumerator TakeDamageDelay(int amt)
    {
        yield return new WaitForSeconds(Random.Range(0.1f,0.3f));
    }
}
