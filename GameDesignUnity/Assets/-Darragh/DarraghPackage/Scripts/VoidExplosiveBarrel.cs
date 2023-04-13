using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoidExplosiveBarrel : MonoBehaviour
{

    public int Health = 1;
    public int currentHealth;
    public GameObject barrel,voidExplosion;
    private AudioSource source;
    public Effects_Manager EM;
    



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
        Instantiate(voidExplosion, transform.position, transform.rotation);
        
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
