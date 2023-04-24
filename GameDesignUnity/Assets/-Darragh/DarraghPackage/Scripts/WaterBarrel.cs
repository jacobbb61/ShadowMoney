using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBarrel : MonoBehaviour
{

    public int Health = 1;
    public int currentHealth;
    public GameObject barrel,waterPuddle;
    private AudioSource source;
    public Effects_Manager EM;

    private RaycastHit hit;
    private float distance = 100f;
    private Vector3 targetLocation;

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

        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {
            targetLocation = hit.point;
        }
    }

    public void Explode()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {
            targetLocation = hit.point;
        }
        targetLocation += new Vector3(0, transform.localScale.y / 2, 0);

        var puddle = Instantiate(waterPuddle, targetLocation, transform.rotation * Quaternion.Euler(0,0,0));
        puddle.transform.rotation = Quaternion.Euler(Vector3.zero);

        Destroy(gameObject);
    }
    

    private void Update()
    {
        if (currentHealth <= 0)
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

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerBullet"))
        {
            Bullet_Manager BM;
            BM = other.GetComponent<Bullet_Manager>();

            currentHealth -= BM.Damage + BM.DamageBuff;



            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; }
            if (BEM.IceEffect) { EM.IsFrozen = true;}
            if (BEM.VoidEffect) { }
            if (BEM.AirEffect) { }
        }else { return; }

    }
}
