using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rizzard_Manager : MonoBehaviour,IDamageable
{
    public NavMeshAgent Agent;
    public GameObject Player;
    Rigidbody myRB;
    RizzardCoverSeeking CS;
    public Effects_Manager EM;
    private bool CanAttack=true;
    private float BaseSpeed;


    [Header("EffectType")]
    public bool Fire;
    public bool Ice;
    public bool Void;
    public bool Air;

    [Header("Stats & other")]
    public int Health;
    public int MaxHealth;
    public GameObject ProjectileDrop;
    public GameObject ElementDrop;
    public GameObject HealthDrop;
    public bool Grounded;

    [Header("Attack")]
    private bool IsAttackingClose;
    private bool IsAttackingFar;
    private bool IsAttackingFar2;
    public float TimeToAttack;
    private float AttackTime;
    public GameObject BulletPoint;
    public GameObject CloseBulletType;
    public GameObject FarBulletType;
    public GameObject Far2BulletType;
    public float CloseRange;
    public float FarRange;
    public int EffectType;

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
        CS = GetComponent<RizzardCoverSeeking>();
        Health = MaxHealth;
        Grounded = true;
        Agent.enabled = true;
    }

    void Update()
    {
        GroundCheck();

    
        CS.boxesAvailable = GameObject.FindGameObjectsWithTag("BoxesAvailable");

        if (CS.boxesAvailable.Length >= 2)
        {
         //   CS.boxSelector();
        }

        if (Agent.enabled && Grounded) { CS.navigation();}

        if (Health <= 0)
        {
            Death();
        }


        if (Vector3.Distance(transform.position, Player.transform.position) <= CloseRange && CanAttack==true)
        {
            IsAttackingClose = true;
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) > CloseRange && Vector3.Distance(transform.position, Player.transform.position) < FarRange && CanAttack == true)
        {
            IsAttackingFar = true;
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) > FarRange + 5 && CanAttack == true)
        {
            IsAttackingFar2 = true;
        }

        if (IsAttackingFar2)
        {
            Attack(Far2BulletType);
            AttackTime += Time.deltaTime;
        }
        if (IsAttackingFar)
        {
            Attack(FarBulletType);
            AttackTime += Time.deltaTime;      
        }
        if (IsAttackingClose) 
        {
            Attack(CloseBulletType); 
            AttackTime += Time.deltaTime; 
        }

        if (EM.IsFrozen) { Frozen(); }
        if (EM.IsBurning) { Burning(); }
    }

    public void GroundCheck()
    {
        if (IsPushed && Grounded == true) { Agent.enabled = true; IsPushed = false; }


        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 2.75f))
        {

            if (hit.transform.tag == "Ground"||  hit.transform.tag == "Wall") { Grounded = true; }

        }
        else { Grounded = false; }


    }

    public void Attack(GameObject Bullet)
    {
        CanAttack = false;
        BulletPoint.transform.LookAt(Player.transform.position);
        if (AttackTime >= TimeToAttack)
        {
            Shoot(Bullet);
            AttackTime = 0;
            CanAttack = true;
            IsAttackingFar = false;
            IsAttackingFar2 = false;
            IsAttackingClose = false;
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
        GameObject A1 = Instantiate(ProjectileDrop);
        A1.transform.position = transform.position;

        GameObject A3 = Instantiate(ElementDrop);
        A3.transform.position = transform.position;

        GameObject A5 = Instantiate(HealthDrop);
        A5.transform.position = transform.position;

        Destroy(this.gameObject);
    }

    IEnumerator DamageNumbers(GameObject Num)
    {
        Num.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        Num.SetActive(false); 
    }
    private void OnTriggerEnter(Collider other)
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

    public void TakeDamage(int amt)
    {
         //insert jujmp code here
        Health -= amt;
    }
}
