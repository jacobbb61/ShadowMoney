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
    private UI_Manager UM;
    GameManager GM;
    public GameObject MainCamera;

    [Header("Attacking")]
    public GameObject BulletV1; //sniper 
    public GameObject BulletV2; //shotgun
    public GameObject BulletHolder;
    public int BaseMeleeDamage;

    [Header("Super Energy")]
    public int SuperEnergyCharges;
    

    [Header("Elements")]
    public int CurrentElement;

    [Header("Player state")]
    public bool CanInput;
    public bool MeleePull;

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
    public float Shoot1AnimTime;
    public float Shoot2AnimTime;
    public float BaseMeleeAnimTime;
    public float ApplyToSelfAnimTime;


    [Header("Unlocked Elements")]
    private bool UnlockedIce;
    private bool UnlockedAir;
    private bool UnlockedVoid;

    private void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");
        Anim = GetComponent<Animator>();
        CanInput = true;
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UM = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Manager>();

        UpdateUnlocked();
    }


    public void UpdateUnlocked()
    {
        if (GM.UnlockedIce) { UnlockedIce = true; }
        if (GM.UnlockedAir) { UnlockedAir = true; }
        if (GM.UnlockedVoid) { UnlockedVoid = true; }
    }

    public void Down(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change&&CanInput==true)
        {     
                CurrentElement = 1;
                UM.CurrentElementHightlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-120);
        }
    }
    public void Right(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedIce)
            {
                CurrentElement = 2;
                UM.CurrentElementHightlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, 0);
            }

        }
    }
    public void Left(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedVoid)
            {
                CurrentElement = 3;
                UM.CurrentElementHightlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 0);
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
                CurrentElement = 4;
                UM.CurrentElementHightlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 120);
            }

        }
    }


    public void RightTrigger(InputAction.CallbackContext context) //shotgun
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput==true)
        {
            Anim.Play("PlayerShoot_2"); 
            StartCoroutine(Shoot(BulletV2, Shoot2AnimTime));
        }
    }
    public void RightBumper(InputAction.CallbackContext context) //sniper
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput == true)
        {
            Anim.Play("PlayerShoot_1");
            StartCoroutine(Shoot(BulletV1, Shoot1AnimTime));
        }
    }


    public void LeftTrigger(InputAction.CallbackContext context) //apply to self
    {
        bool shootL = context.action.triggered;
        if (shootL && CanInput == true && SuperEnergyCharges >= 3)
        {
            StartCoroutine( ApplySelf(CurrentElement, ApplyToSelfAnimTime));     
        }
    }
    public void LeftBumper(InputAction.CallbackContext context)  //melee
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput == true)
        {
            if (SuperEnergyCharges < 9) 
            { 
            StartCoroutine(BaseMeleeAttack());
            }
            else if (SuperEnergyCharges >= 9)
            {              
            StartCoroutine(SuperMeleeAttack());
            }
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
        SuperEnergyCharges -= 3;
        Anim.Play("PlayerApplyToSelf");
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);
        if (CurrentElement == 1) { SelfFire = true; } 
        else if (CurrentElement == 2) { SelfIce = true; }
        else if (CurrentElement == 3) { SelfVoid = true; }
        else if (CurrentElement == 4) { SelfAir = true; }
        CanInput = true;
    }



    IEnumerator BaseMeleeAttack()
    {
        Vector3 fwd = BulletHolder.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(BulletHolder.transform.position, fwd, out hit, 10)) // void effect increases this range?
        {
            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank"))
            {
                MeleePull = true;
                GetComponentInChildren<PlayerCam>().enabled = false;
                Effects_Manager MEM = hit.transform.GetComponent<Effects_Manager>();

                hit.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;




                Anim.Play("PlayerBaseMelee");
                yield return new WaitForSeconds(0.2f);

                if (hit.transform.CompareTag("Nuts")) { hit.transform.GetComponent<Nuts_Manager>().Health -= BaseMeleeDamage;  }
                if (hit.transform.CompareTag("Rizzard")) { hit.transform.GetComponent<Rizzard_Manager>().Health -= BaseMeleeDamage; }
                //if (hit.transform.CompareTag("Footer")) { hit.transform.GetComponent<Footer_Manager>().Health -= BaseMeleeDamage; }
                // if (hit.transform.CompareTag("Tank")) { hit.transform.GetComponent<Tank_Manager>().Health -= BaseMeleeDamage; }

                if (CurrentElement == 1) { MEM.IsBurning = true; }
                else if (CurrentElement == 2) { MEM.IsFrozen = true; }
                // else if (CurrentElement == 3) { SelfVoid = true; }
                // else if (CurrentElement == 4) { SelfAir = true; }
                hit.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                GetComponentInChildren<PlayerCam>().enabled = true;
                MeleePull = false;
                CanInput = true;
            }
            else
            {
                Anim.Play("PlayerBaseMelee");
                yield return new WaitForSeconds(0.2f);
                CanInput = true;
            }

        }
        else 
            {
                Anim.Play("PlayerBaseMelee");
                yield return new WaitForSeconds(0.2f);
                CanInput = true;
            }
    }

    IEnumerator SuperMeleeAttack()
    {
        GetComponentInChildren<PlayerCam>().enabled = false;
        Anim.Play("PlayerSuperMelee");
        yield return new WaitForSeconds(0.75f);
        SuperEnergyCharges = 0;
        MeleePull = true;
        Vector3 fwd = BulletHolder.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(BulletHolder.transform.position, fwd, out hit, 10)) // void effect increases this range?
        {

            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank"))
            {

                Effects_Manager MEM = hit.transform.GetComponent<Effects_Manager>();
                if (hit.transform.CompareTag("Nuts")) { hit.transform.GetComponent<Nuts_Manager>().Health -= 100; }
                if (hit.transform.CompareTag("Rizzard")) { hit.transform.GetComponent<Rizzard_Manager>().Health -= 100; }
                //if (hit.transform.CompareTag("Footer")) { hit.transform.GetComponent<Footer_Manager>().Health -= BaseMeleeDamage; }
                // if (hit.transform.CompareTag("Tank")) { hit.transform.GetComponent<Tank_Manager>().Health -= BaseMeleeDamage; }

               
            }

        } 
        GetComponentInChildren<PlayerCam>().enabled = true;                
        MeleePull = false;             
        CanInput = true;
    }


}
