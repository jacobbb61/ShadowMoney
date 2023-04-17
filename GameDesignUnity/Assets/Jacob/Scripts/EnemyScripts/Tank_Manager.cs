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
    public float WalkSpeed;
    public GameObject SuperEnergyDrop;
    public GameObject HealthDrop;
    public GameObject GroundChecker;
    public bool Grounded;

    private float BaseSpeed;


    [Header("Attack")]
    public float TimeToAttack;
    public float AttackTime;
    public float CloseRange;
    public float FarRange;
    public GameObject FarBulletType;
    public GameObject BulletPoint;

    public bool CanAttack = true;
    public bool IsAttacking;
    public bool SwapMelee;

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


    [Header("Current Element Particles")]
    public GameObject CurrentlyFireParticles;
    public GameObject CurrentlyIceParticles;
    public GameObject CurrentlyVoidParticles;
    public GameObject CurrentlyAirParticles;

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
        if (Fire) { CurrentlyFireParticles.SetActive(true); }
        if (Ice) { CurrentlyIceParticles.SetActive(true); }
        if (Void) { CurrentlyVoidParticles.SetActive(true); }
        if (Air) { CurrentlyAirParticles.SetActive(true); }
        CanAttack = false;
        StartCoroutine(Spawned());
    }
    void Update()
    {
        GroundCheck();
        if (Agent.enabled && Grounded && IsPushed==false)
        {
            Agent.SetDestination(Player.transform.position);
        }
        else
        {
            Push();
        }

        if (Health <= 0 && Health <1000)
        {
            Death();
        }

     
       if (Vector3.Distance(transform.position, Player.transform.position) <= CloseRange && CanAttack == true)
        {
            SwapMelee = !SwapMelee;
            if (SwapMelee) { StartCoroutine(CloseAttack1()); } else { StartCoroutine(CloseAttack2()); }        
            IsAttacking = true;
            CanAttack = false;
        } 
        else if (Vector3.Distance(transform.position, Player.transform.position) >= FarRange && CanAttack == true)
        {
            StartCoroutine(FarAttack());
            IsAttacking = true;
            CanAttack = false;
        }







   
        if (EM.IsFrozen) { Frozen(); }
        if (EM.IsBurning) { Burning(); }


    }





    public void GroundCheck()
    {
        if (IsPushed && Grounded == true) { Agent.enabled = true; IsPushed = false;  }


        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.transform.position, -Vector3.up, out hit, 1.5f))
        {

            if (hit.transform.tag == "Ground" || hit.transform.tag == "Wall") { Grounded = true; }

        }
        else { Grounded = false; Agent.enabled = false; }


    }
    public IEnumerator Spawned()
    {
        CanAttack = false;
        Agent.speed = 0f;
        Agent.angularSpeed = 0;
        yield return new WaitForSeconds(1.5f);
        
        Agent.speed = WalkSpeed;
        Agent.angularSpeed = 90;
        yield return new WaitForSeconds(1.5f);
        CanAttack = true;
    }

    public IEnumerator CloseAttack1()
    {
        CanAttack = false;
        Agent.speed = 0.1f;
        Agent.angularSpeed = 200;
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        Anim.Play("TankCloseAttack1");
        yield return new WaitForSeconds(0.5f);
        Agent.speed = 0f;
        Agent.angularSpeed = 0;
        yield return new WaitForSeconds(1.5f);
        IsAttacking = false;
        Agent.speed = WalkSpeed;
        Agent.angularSpeed = 90;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        CanAttack = true;
        myRB.constraints = RigidbodyConstraints.None;
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public IEnumerator CloseAttack2()
    {
        CanAttack = false;
        Agent.speed = 0.1f;
        Agent.angularSpeed = 200;
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        Anim.Play("TankCloseAttack2");
        yield return new WaitForSeconds(0.5f);
        Agent.speed = 0f;
        Agent.angularSpeed = 0;
        yield return new WaitForSeconds(2f);
        IsAttacking = false;
        Agent.speed = WalkSpeed;
        Agent.angularSpeed = 90;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        CanAttack = true;
        myRB.constraints = RigidbodyConstraints.None;
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator FarAttack()
    {
        CanAttack = false;
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        Anim.Play("TankFarAttack");

        yield return new WaitForSeconds(0.3f);

        Shoot(FarBulletType);

        yield return new WaitForSeconds(1.2f);
        IsAttacking = false;
        yield return new WaitForSeconds(Random.Range(2,4));
        CanAttack = true;
        myRB.constraints = RigidbodyConstraints.None;
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
    }


        public void Shoot(GameObject Bullet)
    { 
        BulletPoint.transform.LookAt(Player.transform.position);
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
        Anim.speed = 0f;
        FrozenParticles.SetActive(true);
        CanAttack = false;
        Agent.speed = 0;
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


    }
    public void Death()
    {
        Anim.Play("TankDeath");
        Agent.enabled = false;
        Health = 10000;
        GameObject A1 = Instantiate(HealthDrop);
        A1.transform.position = transform.position;

        GameObject A2 = Instantiate(SuperEnergyDrop);
        A2.transform.position = transform.position;

        GameObject A3 = Instantiate(HealthDrop);
        A3.transform.position = transform.position;

        GameObject A4 = Instantiate(SuperEnergyDrop);
        A4.transform.position = transform.position;

        GameObject A5 = Instantiate(HealthDrop);
        A5.transform.position = transform.position;

        GameObject A6 = Instantiate(SuperEnergyDrop);
        A6.transform.position = transform.position;

        GameObject A7 = Instantiate(HealthDrop);
        A7.transform.position = transform.position;

        GameObject A8 = Instantiate(SuperEnergyDrop);
        A8.transform.position = transform.position;

        GameObject A9 = Instantiate(HealthDrop);
        A9.transform.position = transform.position;

        GameObject A10 = Instantiate(SuperEnergyDrop);
        A10.transform.position = transform.position;

        Destroy(this.gameObject,1.5f);
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
