using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{

    public int Health = 1;
    public int currentHealth;
    public GameObject barrel, explosion,explosionRadius;
    
    private AudioSource source;
    public Effects_Manager EM;
    public float power = 10f;
    public float explosiveLift = 100f;

    public int explosionDamage = 20;
    public float force;
    public float verticality;
    



    [SerializeField] private float areaEffect;

    [Header("Freeze")]

    public float TimeToBreakFreeze;
    private float FrozenTime;
    public GameObject FrozenParticles;

    private void Start()
    {
        barrel.SetActive(true);
        currentHealth = Health;
        EM = GetComponent<Effects_Manager>();
        source = GetComponent<AudioSource>();
    }

    public void Explode()
    {
        Destroy(gameObject);
        // explosion.SetActive(true);
        Instantiate(explosion, transform.position, quaternion.identity);
        Instantiate(explosionRadius, transform.position, quaternion.identity);
       //source.Play();
        
        Vector3 explosive = transform.position;
        
        Collider[] colliders = Physics.OverlapSphere(explosive, areaEffect);
        

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                //Applying Force
                //rb.AddExplosionForce(power,explosive,areaEffect,explosiveLift);
                Vector3 direction = hit.transform.position - transform.position;
                Vector3 explosiveForce = new Vector3(direction.x, direction.y + verticality * Random.Range(1f, 2f), direction.z);
                rb.AddForce(explosiveForce * force, ForceMode.Impulse);

                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(explosionDamage);
                }
                if (EM.IsBurning)
                {
                    if (hit.TryGetComponent(out Effects_Manager EM))
                    {
                        EM.IsBurning = true;
                    }
                }
            
                if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }
                if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push(); }
                if (hit.transform.CompareTag("Tank")) { hit.gameObject.GetComponent<Tank_Manager>().Push(); }
            }
        }
    }
    

    private void Update()
    {
        if (EM.IsBurning)
        {
            Explode();
        }
        if (EM.IsFrozen)
        {
            Frozen();
        }
    }
    public void Frozen()
    {
        FrozenParticles.SetActive(true);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        FrozenTime += Time.deltaTime;
        if (FrozenTime >= TimeToBreakFreeze)
        {
            FrozenParticles.SetActive(false);
            FrozenTime = 0;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            EM.IsFrozen = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, areaEffect);
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerBullet"))
        {
            Bullet_Manager BM;
            BM = other.GetComponent<Bullet_Manager>();



            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; }
            if (BEM.IceEffect) { EM.IsFrozen = true;}
            if (BEM.VoidEffect) { }
            if (BEM.AirEffect) { }
        }else { return; }

    }

    public void TakeDamage(int amt)
    {
        StartCoroutine(TakeDamageDelay(amt));
    }

    private IEnumerator TakeDamageDelay(int amt)
    {
        yield return new WaitForSeconds(Random.Range(0.1f,0.3f));
        Health -= amt;
    }
}


