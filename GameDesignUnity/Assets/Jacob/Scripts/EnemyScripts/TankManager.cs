using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tank_Manager : MonoBehaviour
{
    NavMeshAgent Agent;
    GameObject Player;
    Rigidbody myRB;
    Animator Anim;
    public Effects_Manager EM;

    [Header("EffectType")]
    public bool Fire;
    public bool Ice;
    public bool Void;
    public bool Air;


    [Header("Stats & other")]
    public int Health;
    public int MaxHealth;
    public GameObject SuperEnergyDrop;
    public GameObject HealthDrop;
    public GameObject GroundChecker;
    public bool Grounded;

    private float BaseSpeed;


    [Header("Attack")]
    public float TimeToAttack;
    public float AttackTime;
    public float range;
    public GameObject BulletType;
    public GameObject BulletPoint;

    private bool CanAttack = true;
    private bool IsAttacking;

    [Header("Freeze")]
    public float TimeToBreakFreeze;
    private float FrozenTime;
    public GameObject FrozenParticles;

    [Header("Burn")]
    public float TimeToStopBurning;
    private float TickTime = 1;
    private float BurningTime;
    public GameObject FireParticles;

    [Header("Air Push")]
    public bool IsPushed;

    [Header("Damage Numbers")]
    public GameObject One;
    public GameObject Five;
    public GameObject FiveBuff;
    public GameObject Twenty;
    public GameObject Forty;


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        myRB = GetComponent<Rigidbody>();
        BaseSpeed = Agent.speed;
        EM = GetComponent<Effects_Manager>();
        Anim = GetComponentInChildren<Animator>();

        Health = MaxHealth;
        Grounded = true;
        Agent.enabled = true;
        if (Fire == false && Ice == false && Void == false && Air == false) { Fire = true; }
    }
    void Update()
    {
        GroundCheck();
        if (Agent.enabled && Grounded)
        {
            Agent.SetDestination(Player.transform.position);
        }

        if (Health <= 0)
        {
            Death();
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= range && CanAttack == true)
        {
            IsAttacking = true;
        }

        if (IsAttacking) { Attack(); }
        if (EM.IsFrozen) { Frozen(); }
        if (EM.IsBurning) { Burning(); }


    }





    public void GroundCheck()
    {
        if (IsPushed && Grounded == true) { Agent.enabled = true; IsPushed = false; }


        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.transform.position, -Vector3.up, out hit, 0.5f))
        {

            if (hit.transform.tag == "Ground" || hit.transform.tag == "Wall") { Grounded = true; }

        }
        else { Grounded = false; }


    }
    public void Attack()
    {
        AttackTime += Time.deltaTime;
        BulletPoint.transform.LookAt(Player.transform.position);
        if (AttackTime >= TimeToAttack)
        {
            Shoot(BulletType);
            AttackTime = 0;
            IsAttacking = false;

        }
    }
    public void Shoot(GameObject Bullet)
    {
        GameObject NewBullet = Instantiate(Bullet, BulletPoint.transform.position, BulletPoint.transform.rotation);
        NewBullet.transform.tag = "EnemyBullet";
        ApplyBulletEffect(NewBullet);
        if (NewBullet.GetComponent<Bullet_Manager>().BulletType3) { Bullet.GetComponent<BoxCollider>().enabled = true; } else { Bullet.GetComponent<SphereCollider>().enabled = true; }
    }
    public void ApplyBulletEffect(GameObject Bullet)
    {
        if (Fire) { Bullet.GetComponent<Effects_Manager>().FireEffect = true; }
        if (Ice) { Bullet.GetComponent<Effects_Manager>().IceEffect = true; }
        if (Void) { Bullet.GetComponent<Effects_Manager>().VoidEffect = true; }
        if (Air) { Bullet.GetComponent<Effects_Manager>().AirEffect = true; }
    }
    public void Frozen()
    {
        FrozenParticles.SetActive(true);
        CanAttack = false;
        Agent.speed = 0;
        FrozenTime += Time.deltaTime;
        if (FrozenTime >= TimeToBreakFreeze)
        {
            FrozenParticles.SetActive(false);
            FrozenTime = 0;
            CanAttack = true;
            Agent.speed = BaseSpeed;
            EM.IsFrozen = false;
        }
    }
    public void Burning()
    {
        FireParticles.SetActive(true);
        BurningTime += Time.deltaTime;
        if (BurningTime >= TickTime) { TickTime++; Health--; StartCoroutine(DamageNumbers(One)); }
        if (BurningTime >= TimeToStopBurning)
        {
            FireParticles.SetActive(false);
            BurningTime = 0;
            TickTime = 1;
            EM.IsBurning = false;
        }
    }

    public void Push()
    {
        Agent.enabled = false;
        Grounded = false;
        GroundCheck();
        StartCoroutine(PushReset());
    }
    public IEnumerator PushReset()
    {
        yield return new WaitForSeconds(0.5f);
        IsPushed = true;
        yield return new WaitForSeconds(1.5f);
        myRB.velocity = Vector3.zero;
        myRB.angularVelocity = Vector3.zero;
    }
    public void Death()
    {
        Agent.enabled = false;
        GameObject A1 = Instantiate(HealthDrop);
        A1.transform.position = transform.position;

        GameObject A2 = Instantiate(SuperEnergyDrop);
        A2.transform.position = transform.position;

        GameObject A3 = Instantiate(HealthDrop);
        A3.transform.position = transform.position;

        GameObject A4 = Instantiate(SuperEnergyDrop);
        A4.transform.position = transform.position;

        Destroy(this.gameObject);
    }

    public IEnumerator DamageNumbers(GameObject Num)
    {
        Num.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        Num.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("PlayerBullet"))
        {
            Bullet_Manager BM;
            BM = other.GetComponent<Bullet_Manager>();
            Health -= BM.Damage + BM.DamageBuff;
            if (BM.Damage == 5) { StartCoroutine(DamageNumbers(Five)); }
            if (BM.DamageBuff == 5) { StartCoroutine(DamageNumbers(FiveBuff)); }
            if (BM.Damage == 20) { StartCoroutine(DamageNumbers(Twenty)); }
            if (BM.Damage == 40) { StartCoroutine(DamageNumbers(Forty)); }



            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; }
            if (BEM.IceEffect) { EM.IsFrozen = true; }
            if (BEM.VoidEffect) { }
            if (BEM.AirEffect) { }
        }
        else { return; }

    }

}
