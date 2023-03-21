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
    public int ProjectileAmmoNum;
    public int ElementAmmoNum;
    public GameObject BulletV1;
    public GameObject BulletV2;
    public GameObject BulletV3;
    public GameObject BulletV4;
    public GameObject CurrentBullet;
    public GameObject BulletHolder;
    GameObject NewBullet;

    [Header("Elements")]
    public int CurrentElement;
    public int InputOrder =1;

    [Header("Player state")]
    public bool ChoosingProjectile;
    public bool ChoosingElement;
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

    [Header("Projectile Cost")]
    private int Type1ProjectileCost = 0;
    private int Type2ProjectileCost = 20;
    private int Type3ProjectileCost = 15;
    private int Type4ProjectileCost = 40;

    [Header("Element Cost")]
    private int Type1ElementCost = 25;
    private int Type2ElementCost = 20;
    private int Type3ElementCost = 10;
    private int Type4ElementCost = 15;

    [Header("Animation Times")]
    public float Summon1;
    public float Summon2;
    public float Summon3;
    public float Summon4;
    public float ApplyFire;
    public float ApplyIce;
    public float ApplyVoid;
    public float ApplyAir;
    public float Shoot1;
    public float Shoot2;
    public float Shoot3;
    public float Shoot4;
    public float ApplyToSelfFire;
    public float ApplyToSelfIce;
    public float ApplyToSelfVoid;
    public float ApplyToSelfAir;

    [Header("Unlocked Projectiles")]
    private bool UnlockedProjectileType1;
    private bool UnlockedProjectileType2;
    private bool UnlockedProjectileType3;
    private bool UnlockedProjectileType4;


    [Header("Unlocked Elements")]
    private bool UnlockedFire;
    private bool UnlockedIce;
    private bool UnlockedAir;
    private bool UnlockedVoid;

    private void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");
        Anim = GetComponent<Animator>();
        ChoosingProjectile = true;
        ChoosingElement = false;
        CanInput = true;
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UpdateUnlocked();
    }


    public void UpdateUnlocked()
    {
        if (GM.UnlockedProjectileType1) { UnlockedProjectileType1 = true; }
        if (GM.UnlockedProjectileType2) { UnlockedProjectileType2 = true; }
        if (GM.UnlockedProjectileType3) { UnlockedProjectileType3 = true; }
        if (GM.UnlockedProjectileType4) { UnlockedProjectileType4 = true; }

        if (GM.UnlockedFire) { UnlockedFire = true; }
        if (GM.UnlockedIce) { UnlockedIce = true; }
        if (GM.UnlockedAir) { UnlockedAir = true; }
        if (GM.UnlockedVoid) { UnlockedVoid = true; }
    }

    public void Abutton(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change&&CanInput==true)
        {     
            if (ChoosingElement && ElementAmmoNum >= Type1ElementCost && UnlockedFire)
            {
                CanInput = false;
                CurrentElement = 1;
                Anim.Play("PlayerApply_Fire");
                StartCoroutine(ApplyEffect(NewBullet, ApplyFire));
                ElementAmmoNum -= Type1ElementCost;
            } 
            if (ChoosingProjectile && ProjectileAmmoNum >= Type1ProjectileCost && UnlockedProjectileType1)
            {
                CanInput = false;
                CurrentBullet = BulletV1;
                Anim.Play("PlayerSummon_1");
                StartCoroutine(ChosenProjectile(CurrentBullet, Summon1));
                ProjectileAmmoNum -= Type1ProjectileCost;
            }
 
        }
    }
    public void Bbutton(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (ChoosingElement && ElementAmmoNum >= Type2ElementCost && UnlockedIce)
            {
                CanInput = false;
                CurrentElement = 2;
                Anim.Play("PlayerApply_Ice");
                StartCoroutine(ApplyEffect(NewBullet, ApplyIce));
                ElementAmmoNum -= Type2ElementCost;
            }
            if (ChoosingProjectile && ProjectileAmmoNum >= Type2ProjectileCost && UnlockedProjectileType2)
            {
                CanInput = false;
                CurrentBullet = BulletV2;
                Anim.Play("PlayerSummon_2");
                StartCoroutine(ChosenProjectile(CurrentBullet, Summon2));
                ProjectileAmmoNum -= Type2ProjectileCost;
            }

        }
    }
    public void Xbutton(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {     
            if (ChoosingElement && ElementAmmoNum >= Type3ElementCost && UnlockedVoid)
            {
                CanInput = false;
                CurrentElement = 3;
                Anim.Play("PlayerApply_Void");
                StartCoroutine(ApplyEffect(NewBullet, ApplyVoid));
                ElementAmmoNum -= Type3ElementCost;
            }
            if (ChoosingProjectile && ProjectileAmmoNum >= Type3ProjectileCost && UnlockedProjectileType3)
            {
                CanInput = false;
                CurrentBullet = BulletV3;
                Anim.Play("PlayerSummon_3");
                StartCoroutine(ChosenProjectile(CurrentBullet, Summon3));
                ProjectileAmmoNum -= Type3ProjectileCost;
            }

        }
    }
    public void Ybutton(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {       
            if (ChoosingElement && ElementAmmoNum >= Type4ElementCost && UnlockedAir)
            {
                CanInput = false;
                CurrentElement = 4;
                Anim.Play("PlayerApply_Air");
                StartCoroutine(ApplyEffect(NewBullet, ApplyAir));
                ElementAmmoNum -= Type4ElementCost;
            }
            if (ChoosingProjectile && ProjectileAmmoNum >= Type4ProjectileCost && UnlockedProjectileType4)
            {
                CanInput = false;
                CurrentBullet = BulletV4;
                Anim.Play("PlayerSummon_4");
                StartCoroutine(ChosenProjectile(CurrentBullet, Summon4));
                ProjectileAmmoNum -= Type4ProjectileCost;
            }

        }
    }


    public void RightTrigger(InputAction.CallbackContext context)
    {
        bool shootR = context.action.triggered;
        if (shootR&&ChoosingProjectile==false&&CanInput==true)
        {
            if (CurrentBullet == BulletV1) { Anim.Play("PlayerShoot_1"); StartCoroutine(Shoot(NewBullet, Shoot1));}
            if (CurrentBullet == BulletV2) { Anim.Play("PlayerShoot_2"); StartCoroutine(Shoot(NewBullet, Shoot2));}
            if (CurrentBullet == BulletV3) { Anim.Play("PlayerShoot_3"); StartCoroutine(Shoot(NewBullet, Shoot3));}
            if (CurrentBullet == BulletV4) { Anim.Play("PlayerShoot_4"); StartCoroutine(Shoot(NewBullet, Shoot4));}
            
            ChoosingElement = false;
            ChoosingProjectile = true;
        }
    }
    public void LeftTrigger(InputAction.CallbackContext context)
    {
        bool shootL = context.action.triggered;
        if (shootL && ChoosingElement == false && ChoosingProjectile == false && SelfEffectTime==0f && CanInput == true)
        {
            if (CurrentBullet == BulletV1) { Anim.Play("PlayerApplyToSelf_Fire"); StartCoroutine(ApplySelf(1,NewBullet, ApplyToSelfFire)); }
            if (CurrentBullet == BulletV2) { Anim.Play("PlayerApplyToSelf_Ice"); StartCoroutine(ApplySelf(2,NewBullet, ApplyToSelfIce)); }
            if (CurrentBullet == BulletV3) { Anim.Play("PlayerApplyToSelf_Void"); StartCoroutine(ApplySelf(3,NewBullet, ApplyToSelfVoid)); }
            if (CurrentBullet == BulletV4) { Anim.Play("PlayerApplyToSelf_Air"); StartCoroutine(ApplySelf(4,NewBullet, ApplyToSelfAir)); }
            ChoosingElement = false;
            ChoosingProjectile = true;
        }
    }


  


    IEnumerator ChosenProjectile(GameObject Bullet, float WaitTime)
    {
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        NewBullet = Instantiate(Bullet, BulletHolder.transform.position, BulletHolder.transform.rotation);
        NewBullet.GetComponent<Projectile_Manager>().IsMoving = false;        
        NewBullet.transform.parent = BulletHolder.transform;
        NewBullet.transform.tag = "PlayerBullet";
        ChoosingProjectile = false;
        ChoosingElement = true;
        NewBullet.GetComponent<Effects_Manager>().None();
        CanInput = true;
    }

    IEnumerator ApplyEffect(GameObject Bullet, float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        Effects_Manager NBEM;
        NBEM = Bullet.GetComponent<Effects_Manager>();
        if (CurrentElement == 0) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 1) { NBEM.FireEffect = true; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 2) { NBEM.FireEffect = false; NBEM.IceEffect = true; NBEM.VoidEffect = false; NBEM.AirEffect = false; }
        if (CurrentElement == 3) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = true; NBEM.AirEffect = false; }
        if (CurrentElement == 4) { NBEM.FireEffect = false; NBEM.IceEffect = false; NBEM.VoidEffect = false; NBEM.AirEffect = true; }
        ChoosingProjectile = false;
        ChoosingElement = false;
        CanInput = true;
    }

    IEnumerator Shoot(GameObject Bullet, float WaitTime)
    {
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        if (CurrentBullet == BulletV3) { Bullet.GetComponent<BoxCollider>().enabled = true; } else { Bullet.GetComponent<SphereCollider>().enabled = true; }
        if (SelfFire == true) { Bullet.GetComponent<Bullet_Manager>().DamageBuff = FireDamageBuff; }
        Bullet.GetComponent<Projectile_Manager>().IsMoving=true;
        Bullet.transform.parent = null;
        ChoosingElement = false;
        ChoosingProjectile = true;
        CurrentBullet = null;
        CanInput = true;
    }

    IEnumerator ApplySelf(int Effect, GameObject Bullet, float WaitTime)
    {
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        Destroy(Bullet);
        if (CurrentElement == 1) { SelfFire = true; } 
        else if (CurrentElement == 2) { SelfIce = true; }
        else if (CurrentElement == 3) { SelfVoid = true; }
        else if (CurrentElement == 4) { SelfAir = true; }
        CanInput = true;
    }
}
