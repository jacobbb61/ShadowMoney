using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nuts_Manager : MonoBehaviour
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

    [Header("Nuts Type Stats")]
    public int NutsBaseSpeed;
    public int NutsBaseMovementRange;
    public int NutsBaseAttackRange;
    public int NutsProjectileSpeed;
    public int NutsProjectileMovementRange;
    public int NutsProjectileAttackRange;
    public GameObject NutsProjectileAssets;
    public GameObject NutsMeleeAssets;
    public Animator NutsProjectileAnimator;
    public Animator NutsMeleeAnimator;

    [Header("Stats & other")]
    public bool IsProjectileNuts;
    public int Health;
    public int MaxHealth;
    public GameObject SuperEnergyDrop;
    public GameObject HealthDrop;
    public GameObject GroundChecker;
    public bool Grounded;
    public GameObject LowHealth;
    
    private float BaseSpeed;


    [Header("Attack")]
    public float TimeToAttack;
    public GameObject BulletType;
    public GameObject BulletPoint;
    
    private bool CanAttack=true;
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

    [Header("Current Element Particles P")]
    public GameObject CurrentlyFireParticlesP;
    public GameObject CurrentlyIceParticlesP;
    public GameObject CurrentlyVoidParticlesP;
    public GameObject CurrentlyAirParticlesP;

    [Header("Current Element Particles M")]
    public GameObject CurrentlyFireParticlesM;
    public GameObject CurrentlyIceParticlesM;
    public GameObject CurrentlyVoidParticlesM;
    public GameObject CurrentlyAirParticlesM;


    [Header("Damage Numbers")]
    public GameObject One;
    public GameObject Five;
    public GameObject Ten;
    public GameObject Fifteen;
    public GameObject Forty;
    public GameObject FortyFive;
    public GameObject Fifty;


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
        if (Fire) { CurrentlyFireParticlesP.SetActive(true); CurrentlyFireParticlesM.SetActive(true); }
        if (Ice) { CurrentlyIceParticlesP.SetActive(true); CurrentlyIceParticlesM.SetActive(true); }
        if (Void) { CurrentlyVoidParticlesP.SetActive(true); CurrentlyVoidParticlesM.SetActive(true); }
        if (Air) { CurrentlyAirParticlesP.SetActive(true); CurrentlyAirParticlesM.SetActive(true); }

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        Agent.enabled = false;
        if (IsProjectileNuts) 
        { 
            NutsProjectileAssets.SetActive(true);
            NutsMeleeAssets.SetActive(false);
            Anim = NutsProjectileAnimator;
        }
        else
        {
            NutsProjectileAssets.SetActive(false);
            NutsMeleeAssets.SetActive(true);
            Anim = NutsMeleeAnimator;
        }
        yield return new WaitForSeconds(0.1f);
        Agent.enabled = true;
    }

    void Update()
    {
        GroundCheck();
        if (Agent.enabled && Grounded) 
        { 
            if (IsProjectileNuts) { MovementAndAttack(NutsProjectileSpeed, NutsProjectileAttackRange, NutsProjectileMovementRange); } 
            else
            {
                MovementAndAttack(NutsBaseSpeed, NutsBaseAttackRange, NutsBaseMovementRange);
            }
        }
        else
        {
            Push();
        }
        if (Health <= 5)
        {
            LowHealth.SetActive(true);
        }

        if (Health <= 0 && Health > -100)
        {
            Death();
        }

         
        if (EM.IsFrozen) { Frozen(); }
        if (EM.IsBurning) { Burning(); }

       
    }

    void MovementAndAttack(int Speed, int AttackRange, int MovementRange)
    { 
        Agent.SetDestination(Player.transform.position);
        
        if (Vector3.Distance(transform.position, Player.transform.position) <= MovementRange && IsAttacking == false)
        {
            Agent.speed = Speed; Anim.SetBool("Walk", true);
            Agent.angularSpeed = 2000f;
   
        }
        else { Agent.speed = 0.01f; Anim.SetBool("Walk", false); }

        if (Vector3.Distance(transform.position, Player.transform.position) <= AttackRange && CanAttack == true)
        {
            StartCoroutine(Attack());
            Anim.SetBool("Walk", false);
            Anim.SetTrigger("Attack");
        }
    }




    public void GroundCheck()
    {    
        if (IsPushed && Grounded==true) { Agent.enabled = true; IsPushed = false;  }
     

        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.transform.position, -Vector3.up, out hit,0.5f))
        {
            
            if (hit.transform.tag == "Ground" || hit.transform.tag == "Wall") { Grounded = true; myRB.mass = 5; }
            Debug.Log(hit.transform.tag);
        } else { Grounded = false; Agent.enabled = false; myRB.mass = 80; myRB.AddForce(Vector3.down, ForceMode.Force); }

       
    }
    public IEnumerator Attack()
    {
        IsAttacking = true;
        CanAttack = false;
        Agent.speed = 1f;
        Agent.angularSpeed = 200f;
        Anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.1f);
        Agent.speed = 0f;
        BulletPoint.transform.LookAt(Player.transform.position);
        yield return new WaitForSeconds(0.8f);
       
        Shoot(BulletType);
        yield return new WaitForSeconds(0.25f);
        IsAttacking = false;
        yield return new WaitForSeconds(0.75f);
        CanAttack = true;
            

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
        Anim.speed = 0f;
        FrozenTime += Time.deltaTime;
        if (FrozenTime >= TimeToBreakFreeze)
        {
            FrozenParticles.SetActive(false);
            FrozenTime = 0;
            Anim.speed = 1f;
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
        myRB.mass = 80;
        myRB.constraints = RigidbodyConstraints.None;
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
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
        Health = -100;
        Anim.SetTrigger("Death");
        Agent.enabled = false;
        Anim.speed = 1;
        Destroy(this.gameObject,1f);
        GameObject A1 = Instantiate(HealthDrop);
        A1.transform.position = transform.position;

        GameObject A3 = Instantiate(SuperEnergyDrop);
        A3.transform.position = transform.position;
  


        
        
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
            int PreHealth = Health;
            
            Health -= BM.Damage + BM.DamageBuff; 




            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; if (Fire) { Health -= 5; } }
            if (BEM.IceEffect) { EM.IsFrozen = true; if (Ice) { Health -= 5; } }
            if (BEM.VoidEffect) { if (Void) { Health -= 5; } }
            if (BEM.AirEffect) { if (Air) { Health -= 5; } }

            int PostHealth = Health;
            int HealthTaken = PreHealth - PostHealth;
            switch (HealthTaken)
            {
                case 5: StartCoroutine(DamageNumbers(Five)); break;
                case 10: StartCoroutine(DamageNumbers(Ten)); break;
                case 15: StartCoroutine(DamageNumbers(Fifteen)); break;
                case 40: StartCoroutine(DamageNumbers(Forty)); break;
                case 45: StartCoroutine(DamageNumbers(FortyFive)); break;
                case 50: StartCoroutine(DamageNumbers(Fifty)); break;
                default:
                    break;
            }
        }
        else { return; }

    }

}
