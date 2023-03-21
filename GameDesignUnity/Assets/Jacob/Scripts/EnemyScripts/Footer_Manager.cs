using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Footer_Manager : MonoBehaviour
{
    NavMeshAgent Agent;
    GameObject Player;
    public GameObject activeCM;
    private bool CanAttack = true;
    private float BaseSpeed;
    private Transform TeleportLocation;


    [Header("Attack")]
    public bool IsTeleporting;
    public bool IsAttacking;
    public float TimeToAttack;
    public float AttackTime;
    public bool Attacking = false;
    public float Range;
    public int EffectType;
    public GameObject BulletType;
    public GameObject BulletPoint;


    [Header("Freeze")]
    public bool IsFrozen;
    public float TimeToBreakFreeze;
    public float FrozenTime;


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        BaseSpeed = Agent.speed;
        
    }

    void Update()
    {
        if (Agent.enabled) { Agent.SetDestination(Player.transform.position); }
       

        if (Vector3.Distance(transform.position, Player.transform.position) <= Range && CanAttack == true)
        {
            IsTeleporting = true;
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) >= Range && CanAttack == true)
        {
            IsAttacking = true;
        }
        if (Vector3.Distance(transform.position, Player.transform.position) >= Range) { Agent.speed = BaseSpeed; } else { Agent.speed = 0.5f; }

        if (IsAttacking)
        {
            Attack(BulletType);
            AttackTime += Time.deltaTime;
        }
        if (IsTeleporting)
        {
            //Teleport();
            AttackTime += Time.deltaTime;
        }

        if (IsFrozen) { Frozen(); }
    }

    public void Attack(GameObject Bullet)
    {
        CanAttack = false;
        BulletPoint.transform.LookAt(Player.transform.position);
        Agent.speed = 0.5f;
        if (AttackTime >= TimeToAttack)
        {
            Shoot(Bullet);
            AttackTime = 0;
            CanAttack = true;
            IsAttacking = false;
        }
    }
    public void Shoot(GameObject Bullet)
    {
        Instantiate(Bullet, BulletPoint.transform.position, BulletPoint.transform.rotation);
        Bullet.transform.tag = "EnemyBullet";
    }

    public void Teleport()
    {
        Agent.enabled = false;
        if (AttackTime < 0.5) { FurthestPoint(); }
        if (AttackTime >= 0.5 && AttackTime < 1) { transform.position = TeleportLocation.position; }
        if (AttackTime >= 1) 
        { 
            Agent.enabled = true;
            AttackTime = 0;
            IsTeleporting = false; 
        }
    }
    public void FurthestPoint()
    {
        float disfrom1;
        float disfrom2;
        float disfrom3;
        float disfrom4;

        disfrom1 = Vector3.Distance(transform.position, activeCM.GetComponent<CombatRoom_Manager>().Pos1.transform.position);
        disfrom2 = Vector3.Distance(transform.position, activeCM.GetComponent<CombatRoom_Manager>().Pos2.transform.position);
        disfrom3 = Vector3.Distance(transform.position, activeCM.GetComponent<CombatRoom_Manager>().Pos3.transform.position);
        disfrom4 = Vector3.Distance(transform.position, activeCM.GetComponent<CombatRoom_Manager>().Pos4.transform.position);

        float x1, x2, lar;
        x1 = (disfrom1 > disfrom2 ? disfrom1 : disfrom2);
        x2 = (disfrom3 > disfrom4 ? disfrom3 : disfrom4);
        lar = (x1 > x2 ? x1 : x2);

        Debug.Log(lar);

        if (lar == disfrom1) { TeleportLocation = activeCM.GetComponent<CombatRoom_Manager>().Pos1.transform; }
        else if (lar == disfrom2) { TeleportLocation = activeCM.GetComponent<CombatRoom_Manager>().Pos2.transform; }
        else if (lar == disfrom3) { TeleportLocation = activeCM.GetComponent<CombatRoom_Manager>().Pos3.transform; }
        else if (lar == disfrom4) { TeleportLocation = activeCM.GetComponent<CombatRoom_Manager>().Pos4.transform; }

    }

    public void Frozen()
    {
        CanAttack = false;
        Agent.speed = 0;
        FrozenTime += Time.deltaTime;

        if (FrozenTime >= TimeToBreakFreeze)
        {
            FrozenTime = 0;
            CanAttack = true;
            Agent.speed = BaseSpeed;
            IsFrozen = false;
        }
    }
}
