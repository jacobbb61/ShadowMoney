using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Footer_Manager : MonoBehaviour
{

    NavMeshAgent Agent;
    GameObject Player;
    Rigidbody myRB;
    Animator Anim;
    public Effects_Manager EM;
    public Collider TeleportBounds;

    [Header("EffectType")]
    public bool Fire;
    public bool Ice;
    public bool Void;
    public bool Air;


    [Header("Stats & other")]
    public int Health;
    public int MaxHealth;
    public int WalkSpeed;
    public GameObject SuperEnergyDrop;
    public GameObject HealthDrop;
    public GameObject GroundChecker;
    public GameObject Assets;
    public bool Grounded;

    private float BaseSpeed;
    public Vector3 TelePos;


    [Header("Attack")]
    public float TimeToAttack;
    public float AttackTime;
    public float CloseRange;
    public float FarRange;
    public GameObject BulletType;
    public GameObject BulletPoint;

    public bool CanAttack = true;
    public bool IsAttacking;

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
        TelePos = transform.position;
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
        if (Agent.enabled && Grounded && IsPushed == false)
        {
            Agent.SetDestination(Player.transform.position);
        }

        if (Health <= 0 && Health <10000)
        {
            Death();
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= CloseRange && CanAttack == true && EM.IsFrozen == false)
        {
            StartCoroutine(Teleport());
            IsAttacking = true;
            CanAttack = false;
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) >= FarRange && CanAttack == true)
        {
            StartCoroutine(FarAttack());
            IsAttacking = true;
            CanAttack = false;
        }





        Assets.transform.LookAt(Player.transform.position);

        if (IsAttacking) { Agent.speed = 0; }
        else
        {
            
            Agent.speed = WalkSpeed;
        }
        if (EM.IsFrozen) { Frozen(); } 
        if (EM.IsBurning) { Burning(); }


    }





    public void GroundCheck()
    {
        if (IsPushed && Grounded == true) { Agent.enabled = true; IsPushed = false; }


        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.transform.position, -Vector3.up, out hit, 1.5f))
        {

            if (hit.transform.tag == "Ground" || hit.transform.tag == "Wall") { Grounded = true; }

        }
        else { Grounded = false; }


    }
    public IEnumerator Teleport()
    {

        Anim.Play("Headfoot-Teleport");

        yield return new WaitForSeconds(1f);
        GetComponent<SphereCollider>().enabled = false;
        AttemptTeleport();
        transform.position = TelePos;

        yield return new WaitForSeconds(1f);
        GetComponent<SphereCollider>().enabled = true;

        yield return new WaitForSeconds(1f);
        IsAttacking = false;
        CanAttack = true;

    }
    public void AttemptTeleport()
    {
        Vector3 Location;
        Location = RandomPointInBounds(TeleportBounds.bounds);
        RaycastHit hit;
        if (Physics.Raycast(Location, -Vector3.up, out hit))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                TelePos = hit.transform.position; 
            }
            Debug.DrawLine(Location, hit.point, Color.cyan);
        }
        else
        {
            AttemptTeleport();
            Debug.Log("try again");
        }
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    public IEnumerator FarAttack()
    {

        Anim.Play("Headfoot-shoot");

        yield return new WaitForSeconds(0.65f);

        Shoot(BulletType);

        yield return new WaitForSeconds(2.5f);
        IsAttacking = false;
        CanAttack = true;
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
        Health = 100000;
        Anim.speed = 1;
        Anim.Play("Headfoot-death");
        Agent.enabled = false;
        GameObject A1 = Instantiate(HealthDrop);
        A1.transform.position = BulletPoint.transform.position;

        GameObject A2 = Instantiate(SuperEnergyDrop);
        A2.transform.position = BulletPoint.transform.position;

        GameObject A3 = Instantiate(HealthDrop);
        A3.transform.position = BulletPoint.transform.position;

        GameObject A4 = Instantiate(SuperEnergyDrop);
        A4.transform.position = BulletPoint.transform.position;

        Destroy(this.gameObject,1f);
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
