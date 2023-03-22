using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerCombat : MonoBehaviour
{
    private GameObject Cam;
    private PlayerManager PM;
    private Animator Anim;
    GameManager GM;

    [Header("Bullets")]
    public GameObject BulletV1; //sniper 
    public GameObject BulletV2; //shotgun
    public GameObject BulletHolder; 


    [Header("Elements")]
    public int CurrentElement;

    [Header("Player state")]
    public bool CanInput;

    [Header("Player Applied Effects")]
    public bool SelfFire;
    public int FireDamageBuff;
    public bool SelfIce;
    public int IceArmourBuff;
    public bool SelfVoid;
    public float VoidJumpBuff;
    public bool SelfAir;
    public float AirFallBuff;
    public float SelfEffectTime;
    public GameObject SelfFireParticle;
    public GameObject SelfIceParticle;
    public GameObject SelfVoidParticle;
    public GameObject SelfAirParticle;

    [Header("Enemy Applied Effects")]
    public GameObject EnemyFireParticle;
    public GameObject EnemyIceParticle;



    [Header("Animation Times")]
    public float Shoot1;
    public float Shoot2;
    public float ApplyToSelf;


    [Header("Unlocked Elements")]
    private bool UnlockedFire;
    private bool UnlockedIce;
    private bool UnlockedAir;
    private bool UnlockedVoid;

    private void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");
        Anim = GetComponent<Animator>();
        CanInput = true;
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UpdateUnlocked();
    }


    public void UpdateUnlocked()
    {


        if (GM.UnlockedFire) { UnlockedFire = true; }
        if (GM.UnlockedIce) { UnlockedIce = true; }
        if (GM.UnlockedAir) { UnlockedAir = true; }
        if (GM.UnlockedVoid) { UnlockedVoid = true; }
    }

    public void Down(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change&&CanInput==true)
        {     
            if (UnlockedFire)
            {
                CanInput = false;
                CurrentElement = 1;
            } 
        }
    }
    public void Left(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedIce)
            {
                CanInput = false;
                CurrentElement = 2;
            }

        }
    }
    public void Right(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedVoid)
            {
                CanInput = false;
                CurrentElement = 3;
            }

        }
    }
    public void Up(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedAir)
            {
                CanInput = false;
                CurrentElement = 4;
            }

        }
    }


    public void RightTrigger(InputAction.CallbackContext context) //shotgun
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput==true)
        {
            Anim.Play("PlayerShoot_1"); 
            StartCoroutine(Shoot(BulletV2, Shoot1));
        }
    }
    public void RightBumper(InputAction.CallbackContext context) //sniper
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput == true)
        {
            Anim.Play("PlayerShoot_1");
            StartCoroutine(Shoot(BulletV1, Shoot1));
        }
    }


    public void LeftTrigger(InputAction.CallbackContext context) //apply to self
    {
        bool shootL = context.action.triggered;
        if (shootL && CanInput == true)
        {
            ApplySelf(CurrentElement, ApplyToSelf);
            
        }
    }
    public void LeftBumper(InputAction.CallbackContext context)  //melee
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput == true)
        {

        }
    }

    public void ApplyEffect(GameObject Bullet, float WaitTime)
    {
        
        Effects_Manager NBEM;
        NBEM = Bullet.GetComponent<Effects_Manager>();
        if (CurrentElement == 0) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 1) { NBEM.FireEffect = true; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 2) { NBEM.FireEffect = false; NBEM.IceEffect = true; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 3) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = true; NBEM.AirEffect = false; }
        if (CurrentElement == 4) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = true; }

        CanInput = true;
    }

    IEnumerator Shoot(GameObject Bullet, float WaitTime)
    {
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        GameObject BulletShot = Instantiate(Bullet);
        if (SelfFire == true) { BulletShot.GetComponent<Bullet_Manager>().DamageBuff = FireDamageBuff; }
        BulletShot.transform.parent = null;
        BulletShot.transform.position = BulletHolder.transform.position;
        BulletShot.transform.rotation = BulletHolder.transform.rotation;
        BulletShot.transform.tag = "PlayerBullet";

        if (CurrentElement == 1) { BulletShot.GetComponent<Effects_Manager>().FireEffect = true; }
        else if (CurrentElement == 2) { BulletShot.GetComponent<Effects_Manager>().IceEffect = true; }
        else if (CurrentElement == 3) { BulletShot.GetComponent<Effects_Manager>().VoidEffect = true; }
        else if (CurrentElement == 4) { BulletShot.GetComponent<Effects_Manager>().AirEffect = true; }

        CanInput = true;
    }

    IEnumerator ApplySelf(int Effect, float WaitTime)
    {
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        if (CurrentElement == 1) { SelfFire = true; } 
        else if (CurrentElement == 2) { SelfIce = true; }
        else if (CurrentElement == 3) { SelfVoid = true; }
        else if (CurrentElement == 4) { SelfAir = true; }
        CanInput = true;
    }
}
