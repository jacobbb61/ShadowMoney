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
    public GameObject GroundChecker;
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
        CS = GetComponent<RizzardCoverSeeking>();
        Health = MaxHealth;
        Grounded = true;
        Agent.enabled = true;
        if (Fire == false && Ice == false && Void == false && Air == false) { Fire = true; }
        if (Fire) { CurrentlyFireParticles.SetActive(true); }
        if (Ice) { CurrentlyIceParticles.SetActive(true); }
        if (Void) { CurrentlyVoidParticles.SetActive(true); }
        if (Air) { CurrentlyAirParticles.SetActive(true); }
    }

    void Update()
    {
        GroundCheck();

    
       /* CS.boxesAvailable = GameObject.FindGameObjectsWithTag("BoxesAvailable");

        if (CS.boxesAvailable.Length >= 2)
        {
         //   CS.boxSelector();
        }
       
        if (Agent.enabled && Grounded) { CS.navigation();}
*/
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
        if (IsPushed && Grounded == true) { Agent.enabled = true; IsPushed = false; myRB.constraints = RigidbodyConstraints.FreezeAll; }


        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.transform.position, -Vector3.up, out hit, 0.5f))
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

    public void TakeDamage(int amt)
    {
         //insert jujmp code here
        Health -= amt;
    }
}
