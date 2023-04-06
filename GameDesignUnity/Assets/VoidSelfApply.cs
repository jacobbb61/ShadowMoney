using UnityEngine;

public class VoidSelfApply : MonoBehaviour
{
    public GameObject Particles;
    public GameObject emptyExplosion;
    private AudioSource source;
    public float Force;
 
    
    [SerializeField] public float areaEffect;

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
                Vector3 direction = hit.transform.position - transform.position;
                Vector3 explosiveForce = new Vector3(direction.x, direction.y, direction.z);
                rb.AddForce((explosiveForce * Force * 1.5f)*-1, ForceMode.Impulse);
            }
            
            if (hit.transform.CompareTag("Nuts")) { hit.gameObject.GetComponent<Nuts_Manager>().Push(); }
            if (hit.transform.CompareTag("Rizzard")) { hit.gameObject.GetComponent<Rizzard_Manager>().Push(); }
            if (hit.transform.CompareTag("Tank")) { hit.gameObject.GetComponent<Tank_Manager>().Push(); }
        }
        Debug.Log("Exploded");
        Destroy(emptyExplosion);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, areaEffect);
    }
}
