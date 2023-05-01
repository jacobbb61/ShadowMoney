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
    public int SuperMeleeDamage; 
    public GameObject MeleePullDestination;
    public Collider SuperMeleeCollider;

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
    public GameObject SelfFirePrefab;
    public GameObject SelfIcePrefab;
    public GameObject SelfVoidPrefab;
    public GameObject SelfAirPrefab;

    [Header("Enemy Applied Effects")]
    public GameObject EnemyFireParticle;
    public GameObject EnemyIceParticle;



    [Header("Animation Times")]
    public float Shoot1AnimTime;
    public float Shoot2AnimTime;
    public float BaseMeleeAnimTime;
    public float ApplyToSelfAnimTime;

    [Header("HandParticles")]
    public GameObject RightHandFire;
    public GameObject LeftHandFire;
    public GameObject RightHandIce;
    public GameObject LeftHandIce;
    public GameObject RightHandVoid;
    public GameObject LeftHandVoid;
    public GameObject RightHandAir;
    public GameObject LeftHandAir;

    [Header("Self Apply Particles")]
    public GameObject SelfFireParticles;
    public GameObject SelfIceParticles;
    public GameObject SelfVoidParticles;
    public GameObject SelfAirParticles;

    [Header("SuperMelee Particles")]
    public GameObject SuperMeleeFireParticles;
    public GameObject SuperMeleeIceParticles;
    public GameObject SuperMeleeVoidParticles;
    public GameObject SuperMeleeAirParticles;

    [Header("Audio")]
    public AudioSource BaseMeleeAudio;
    public AudioSource DashAudio;
    public AudioSource JumpAudio;
    public AudioSource LandAudio;
    public AudioSource WalkAudio;
    public AudioSource ChangeElementFireAudio;
    public AudioSource ChangeElementIceAudio;
    public AudioSource ChangeElementVoidAudio;
    public AudioSource ChangeElementAirAudio;
    public AudioSource ShootElementFireAudio;
    public AudioSource ShootElementIceAudio;
    public AudioSource ShootElementVoidAudio;
    public AudioSource ShootElementAirAudio;

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
        UpdateHandParticle();
        UpdateUnlocked();

    }


    public void UpdateUnlocked()
    {
        
        if (GM.UnlockedIce) { UnlockedIce = true; }
        if (GM.UnlockedAir) { UnlockedAir = true; }
        if (GM.UnlockedVoid) { UnlockedVoid = true; }
        UpdateElementAnim();
    }

    public void Down(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change&&CanInput==true)
        {     
            CurrentElement = 1; UpdateHandParticle();
            UpdateElementAnim();
        }
    }
    public void Right(InputAction.CallbackContext context)
    {
        bool change = context.action.triggered;
        if (change && CanInput == true)
        {
            if (UnlockedIce)
            {
                CurrentElement = 2; UpdateHandParticle();
                UpdateElementAnim();
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
                CurrentElement = 3; UpdateHandParticle();
                UpdateElementAnim();
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
                CurrentElement = 4; UpdateHandParticle();
                UpdateElementAnim();
            }

        }
    }


    public void RightTrigger(InputAction.CallbackContext context) //shotgun
    {
        bool shootR = context.action.triggered;
        if (shootR && CanInput==true)
        {
            Anim.Play("playerHands_shootingShotgun"); 
            StartCoroutine(Shoot(BulletV2, Shoot2AnimTime));
        }
    }
    public void RightBumper(InputAction.CallbackContext context) //sniper
    {
        bool BumpR = context.action.triggered;
        if (BumpR && CanInput == true)
        {
            Anim.Play("playerHands_shootingSniper");
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
        bool BumpL = context.action.triggered;
        if (BumpL && CanInput == true)
        { 
            if (SuperEnergyCharges >= 9 && GM.UnlockedSuperPunch)
            {              
            StartCoroutine(SuperMeleeAttack());
            }
            else  
            { 
            StartCoroutine(BaseMeleeAttack());
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


    }

    public void UpdateHandParticle()
    {

        if (CurrentElement == 1) { RightHandFire.SetActive(true); LeftHandFire.SetActive(true); ChangeElementFireAudio.Play(); } else { RightHandFire.SetActive(false); LeftHandFire.SetActive(false); }
        if (CurrentElement == 2) { RightHandIce.SetActive(true); LeftHandIce.SetActive(true); ChangeElementIceAudio.Play(); } else { RightHandIce.SetActive(false); LeftHandIce.SetActive(false); }
        if (CurrentElement == 3) { RightHandVoid.SetActive(true); LeftHandVoid.SetActive(true); ChangeElementVoidAudio.Play(); } else { RightHandVoid.SetActive(false); LeftHandVoid.SetActive(false); }
        if (CurrentElement == 4) { RightHandAir.SetActive(true); LeftHandAir.SetActive(true); ChangeElementAirAudio.Play(); } else { RightHandAir.SetActive(false); LeftHandAir.SetActive(false); }
    }

    public void UpdateElementAnim()
    {
        if (UM.FireElement.gameObject.activeInHierarchy) { if (CurrentElement == 1) { UM.FireElement.SetBool("ON", true); } else { UM.FireElement.SetBool("ON", false); } }
        if (UM.IceElement.gameObject.activeInHierarchy) { if (CurrentElement == 2 && UnlockedIce) { UM.IceElement.SetBool("ON", true); } else { UM.IceElement.SetBool("ON", false); } }
        if (UM.VoidElement.gameObject.activeInHierarchy) { if (CurrentElement == 3 && UnlockedVoid) { UM.VoidElement.SetBool("ON", true); } else { UM.VoidElement.SetBool("ON", false); } }
        if (UM.AirElement.gameObject.activeInHierarchy) { if (CurrentElement == 4 && UnlockedAir) { UM.AirElement.SetBool("ON", true); } else { UM.AirElement.SetBool("ON", false); } }
    }



    IEnumerator Shoot(GameObject Bullet, float WaitTime)
    {
        CanInput = false;
         yield return new WaitForSeconds(0.3f);
        GameObject BulletShot = Instantiate(Bullet);
        if (SelfFire == true) { BulletShot.GetComponent<Bullet_Manager>().DamageBuff = FireDamageBuff; }
        BulletShot.transform.parent = null;
        BulletShot.transform.position = BulletHolder.transform.position;
        BulletShot.transform.rotation = BulletHolder.transform.rotation;
        BulletShot.transform.tag = "PlayerBullet";

        if (CurrentElement == 1) { BulletShot.GetComponent<Effects_Manager>().FireEffect = true; ShootElementFireAudio.Play(); }
        else if (CurrentElement == 2) { BulletShot.GetComponent<Effects_Manager>().IceEffect = true; ShootElementIceAudio.Play(); }
        else if (CurrentElement == 3) { BulletShot.GetComponent<Effects_Manager>().VoidEffect = true; ShootElementVoidAudio.Play(); }
        else if (CurrentElement == 4) { BulletShot.GetComponent<Effects_Manager>().AirEffect = true; ShootElementAirAudio.Play(); }
        yield return new WaitForSeconds(0.45f);
        CanInput = true;
    }

        IEnumerator ApplySelf(int Effect, float WaitTime)
    {
        SuperEnergyCharges -= 3;
        Anim.Play("playerHands_applyToSelf");
        CanInput = false;
        yield return new WaitForSeconds(WaitTime);


        if (CurrentElement == 1) { SelfFire = true; GameObject Prefab = Instantiate(SelfFirePrefab); Prefab.transform.position = transform.position; }
        else if (CurrentElement == 2) { SelfIce = true; GameObject Prefab = Instantiate(SelfIcePrefab); Prefab.transform.position = transform.position; }
        else if (CurrentElement == 3) { SelfVoid = true; GameObject Prefab = Instantiate(SelfVoidPrefab); Prefab.transform.position = transform.position; }
        else if (CurrentElement == 4) { SelfAir = true; GameObject Prefab = Instantiate(SelfAirPrefab); Prefab.transform.position = transform.position; }
        yield return new WaitForSeconds(WaitTime);
        CanInput = true;
    }



    IEnumerator BaseMeleeAttack()
    {
        CanInput = false;
        BaseMeleeAudio.pitch = Random.Range(0.8f, 1.2f);
        Vector3 fwd = BulletHolder.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Anim.Play("playerHands_meleeDefault");
        if (Physics.Raycast(BulletHolder.transform.position, fwd, out hit, 10)) // void effect increases this range?
        {
            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank"))
            { 
                MeleePullDestination = hit.transform.gameObject;
                MeleePull = true;
                GetComponentInChildren<PlayerCam>().enabled = false;
                Effects_Manager MEM = hit.transform.GetComponent<Effects_Manager>();
                GameObject EnemyHit = hit.transform.gameObject;

                
                

                if (CurrentElement == 1) { MEM.IsBurning = true; }
                else if (CurrentElement == 2) { MEM.IsFrozen = true; }
                // else if (CurrentElement == 3) { SelfVoid = true; }
                // else if (CurrentElement == 4) { SelfAir = true; }
                if (EnemyHit.transform.CompareTag("Nuts")) { EnemyHit.transform.GetComponent<Nuts_Manager>().Health -= BaseMeleeDamage;  }
                if (EnemyHit.transform.CompareTag("Rizzard")) { EnemyHit.transform.GetComponent<Rizzard_Manager>().Health -= BaseMeleeDamage; }
                if (EnemyHit.transform.CompareTag("Footer")) { EnemyHit.transform.GetComponent<Footer_Manager>().Health -= BaseMeleeDamage; }
                if (EnemyHit.transform.CompareTag("Tank")) { EnemyHit.transform.GetComponent<Tank_Manager>().Health -= BaseMeleeDamage; }

                
                MeleePull = false;

                

                GetComponentInChildren<PlayerCam>().enabled = true;
               
            }
            
                
            }
        Anim.Play("playerHands_meleeDefault");
                yield return new WaitForSeconds(0.33f);
                CanInput = true;
    }

    IEnumerator SuperMeleeAttack()
    {
        
        Anim.Play("playerHands_meleeSuper");
       // PM.SuperMeleeImmune = true;
        yield return new WaitForSeconds(1.1f);
        SuperEnergyCharges = 0;

        Vector3 PartPos = Cam.transform.position;
        if (CurrentElement == 1) { SelfFire = true; GameObject Prefab = Instantiate(SuperMeleeFireParticles); Prefab.transform.position = PartPos; }
        else if (CurrentElement == 2) { SelfIce = true; GameObject Prefab = Instantiate(SuperMeleeIceParticles); Prefab.transform.position = PartPos; }
        else if (CurrentElement == 3) { SelfVoid = true; GameObject Prefab = Instantiate(SuperMeleeVoidParticles); Prefab.transform.position = PartPos; }
        else if (CurrentElement == 4) { SelfAir = true; GameObject Prefab = Instantiate(SuperMeleeAirParticles); Prefab.transform.position = PartPos; }


        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 5f), 15);//range

        if (colliders != null)
        {
            foreach (Collider _hit in colliders)
            {
                if (_hit.transform.CompareTag("Nuts")) { _hit.gameObject.GetComponent<Nuts_Manager>().Health -= 60; Debug.Log("Hit nuts"); }
                if (_hit.transform.CompareTag("Rizzard")) { _hit.gameObject.GetComponent<Rizzard_Manager>().Health -= 60; }
                if (_hit.transform.CompareTag("Tank")) { _hit.gameObject.GetComponent<Tank_Manager>().Health -= 60; }
                if (_hit.transform.CompareTag("Footer")) { _hit.gameObject.GetComponent<Footer_Manager>().Health -= 60; }
            }
        }
        
        yield return new WaitForSeconds(0.2f);

              
        MeleePull = false;             
        CanInput = true;
       // PM.SuperMeleeImmune = false;
    }

  

}
