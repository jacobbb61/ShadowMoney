using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IDamageable
{

    private Animator Anim;
    public CharacterController controller;
    public PlayerCombat PC;
    public GameManager GM;
    public Effects_Manager EM;
    public UI_Manager UM;
    private bool jumped = false;
    private bool dash = false;
    private bool CanWalk = true;
    private bool dashTS;


    

    public Vector3 playerVelocity;
    private Vector3 dashV;
    private Vector2 movementInput = Vector2.zero;

    public float gravityValue = -40f;
    private int DamageReduction = 0;
    

    [Header("Player stats")]
    public float playerSpeed = 10f;
    public float jumpHeight = 6.0f;
    public float dashSpeed;
    public bool groundedPlayer;
    public bool SuperMeleeImmune;
    public float dashT;
    public int Health;
    private float BurnT;
    private float BurnTickTime;
    private float TimeToStopBurn=5f;
    private float FreezeT;
    private float TimeToBreakFreeze=2.5f;
    private float FreezeSpeed=6.5f;

    [Header("Game stats")]
    public bool Paused = false;
    public bool CloseTutorialUI = false;


    void Start()
    {
        Anim = GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        PC = GetComponent<PlayerCombat>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UM = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Manager>();
        EM = GetComponent<Effects_Manager>();
        GM.RespawnPlayer(this.gameObject);
        Health = 100;
        Time.timeScale = 1;
        // transform.position = Vector3.zero;
        if (SceneManager.GetActiveScene().name == "MainMenu") { Cursor.lockState = CursorLockMode.Confined; } else { Cursor.lockState = CursorLockMode.Locked; }
    }

    public void Update()
    {

        HealthUpdate();

        UIUpdate();

        Gravity();

        Move();

        Dash();
        if (PC.MeleePull) { MeleePull(); }


        if (PC.SelfFire) { SelfFireEffect(); }
        if (PC.SelfIce) { SelfIceEffect(); }
        if (PC.SelfVoid) { SelfVoidEffect(); }
        if (PC.SelfAir) { SelfAirEffect(); }
        if (EM.IsBurning) { EnemyFireEffect(); }
        if (EM.IsFrozen) { EnemyIceEffect(); }


    }


    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }
    public void Dash(InputAction.CallbackContext context)
    {
        dash = context.action.triggered;
        
    }

    public void Interact(InputAction.CallbackContext context)
    {

    }




    public void HealthUpdate()
    {
        if (Health <= 0) { UM.OpenYouDied(); }
    }
    public void UIUpdate()
    {
        UM.PlayerHPSLider.value = Health;
        UM.PlayerDashSLider.value = dashT;
        UM.PlayerSuperBar.value = PC.SuperEnergyCharges;

        if (UM.PlayerDashSLider.value == 0) { UM.DashGold.SetActive(true); } else { UM.DashGold.SetActive(false); }
        if (UM.PlayerSuperBar.value == 9) { UM.SuperGold.SetActive(true); } else { UM.SuperGold.SetActive(false); }




        Transform cameraTransform = Camera.main.transform;
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100.0f))
        {
            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank")) 
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <= 50)
                {
                    UM.CrossHairOverEnemy.SetActive(true);
                }
                else
                {
                    UM.CrossHairOverEnemy.SetActive(false);
                }
                if(Vector3.Distance(transform.position, hit.transform.position) <= 10) { UM.CrossHairMeleeRange.SetActive(true); } else { UM.CrossHairMeleeRange.SetActive(false); }
            }
            else
            {
                UM.CrossHairOverEnemy.SetActive(false);
                UM.CrossHairMeleeRange.SetActive(false);
            }
        }
        else
        {
            UM.CrossHairOverEnemy.SetActive(false);
            UM.CrossHairMeleeRange.SetActive(false);
        }


    }
    public void Gravity()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer){
            PC.LandAudio.gameObject.SetActive(true);
            Anim.SetTrigger("Land"); Anim.ResetTrigger("Fall");
            if (playerVelocity.y < 0f) { playerVelocity.y = 0f; PC.JumpAudio.gameObject.SetActive(false); }
        } else if (playerVelocity.y <= -2f) { Anim.SetTrigger("Fall"); Anim.ResetTrigger("Jump"); }

        if (jumped && groundedPlayer)
        {
            Anim.SetTrigger("Jump");
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            PC.JumpAudio.gameObject.SetActive(true);
            PC.LandAudio.gameObject.SetActive(false);
            PC.JumpAudio.pitch = Random.Range(0.9f, 1.1f);
            PC.LandAudio.pitch = Random.Range(0.9f, 1.1f);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 3f))
        {

            if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Wall")) 
            { 
                playerVelocity.y = -5f; 
                
                
            }
        }


    }

    public void AirVent(float force)
    {
        playerVelocity.y = 0;
        playerVelocity.y += Mathf.Sqrt(force * -3.0f * gravityValue);
    }
    
    public void Move()
    {
        if (controller.enabled)
        {
            Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

            if (CanWalk == true) { controller.Move(move * Time.deltaTime * playerSpeed); }

            if (movementInput.x>0 || movementInput.y > 0 && groundedPlayer) { PC.WalkAudio.volume = 0.2f; } else { PC.WalkAudio.volume = 0; }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
    public void Dash()
    {
        if ((dash == true) && (dashT == 0f)) 
        { 
            dashT = -2f; dashTS = true; PC.DashAudio.gameObject.SetActive(true); PC.DashAudio.pitch = Random.Range(0.9f, 1.1f); Anim.SetTrigger("Dash");
            if ( movementInput.x == 0 && movementInput.y == 0) { dashV = transform.forward * dashSpeed; }
            else { dashV = (transform.right * movementInput.x + transform.forward * movementInput.y) * dashSpeed; }
            Debug.Log(dashV);
        }
        if (dashTS == true) { dashT += Time.deltaTime; }
        if ((dashT < -1.5f) && (dashTS == true)) { controller.Move(dashV * Time.deltaTime); gravityValue = -20f; CanWalk = false; }
        if (dashT < -1.5f) { dash = false; gravityValue = -40f; CanWalk = true; }
        if (dashT > 0f) { dashT = 0f; dashTS = false; PC.DashAudio.gameObject.SetActive(false); }
    }
    public void MeleePull()
    {
       // if (Vector3.Distance(transform.position, PC.MeleePullDestination.transform.position) >= 5) { controller.Move(transform.forward * Time.deltaTime * 20); }
    }



    public void SelfFireEffect() 
    {
        //damage buff
        PC.SelfEffectTime += Time.deltaTime;
        if (PC.SelfEffectTime >= 5f) { PC.SelfFire = false;  PC.SelfEffectTime = 0f; }
    }
    public void SelfIceEffect()
    {
        DamageReduction = PC.IceArmourBuff;
        PC.SelfEffectTime += Time.deltaTime;
        if (PC.SelfEffectTime >= 5f) { PC.SelfIce = false;   DamageReduction = 0; PC.SelfEffectTime = 0f; }
    }
    public void SelfVoidEffect()
    {
        jumpHeight = PC.VoidJumpBuff;
        PC.SelfEffectTime += Time.deltaTime;
        if (PC.SelfEffectTime >= 5f) { PC.SelfVoid = false; jumpHeight = 6f; PC.SelfEffectTime = 0f; }
    }
    public void SelfAirEffect()
    {
        gravityValue = PC.AirFallBuff;
        PC.SelfEffectTime += Time.deltaTime;
        if (PC.SelfEffectTime >= 5f) { PC.SelfAir = false; gravityValue = -40f;  PC.SelfEffectTime = 0f; }
    }



    public void EnemyFireEffect()
    {
        PC.EnemyFireParticle.SetActive(true);
        BurnT += Time.deltaTime;       
        if (BurnT >= BurnTickTime) { BurnTickTime++; Health--; }
        if (BurnT >= TimeToStopBurn)
        {
            PC.EnemyFireParticle.SetActive(false);
            BurnT = 0f;
            BurnTickTime = 1f;
            EM.IsBurning = false;
        }
    }
    public void EnemyIceEffect()
    {
        PC.EnemyIceParticle.SetActive(true);
        FreezeT += Time.deltaTime;
        if (FreezeT > 0) { playerSpeed = FreezeSpeed; }
        if (FreezeT >= TimeToBreakFreeze) { playerSpeed = 10f;  FreezeT = 0f; EM.IsFrozen = false; PC.EnemyIceParticle.SetActive(false); }
    }
    public void EnemyVoidEffect(GameObject Bullet)
    {
        Vector3 VoidPullPos;
        VoidPullPos = Bullet.transform.forward * -1;
        controller.Move(VoidPullPos * Time.deltaTime * 50f);
    }
    public void EnemyAirEffect(GameObject Bullet)
    {
        Vector3 AirPushPos;
        AirPushPos = Bullet.transform.forward * +1;
        controller.Move(AirPushPos * Time.deltaTime * 50f);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet")&& SuperMeleeImmune==false)
        {
            Bullet_Manager BM;
            BM = other.GetComponent<Bullet_Manager>();
            Health -= BM.Damage - DamageReduction;
            Debug.Log("Hit");

            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; StartCoroutine(HitFeedback(UM.FireHit)); }
            else if (BEM.IceEffect) { EM.IsFrozen = true; StartCoroutine(HitFeedback(UM.IceHit)); }
            else if (BEM.VoidEffect) { EnemyVoidEffect(other.gameObject); StartCoroutine(HitFeedback(UM.VoidHit)); }
            else if (BEM.AirEffect) { EnemyAirEffect(other.gameObject); StartCoroutine(HitFeedback(UM.AirHit)); }
            else { return; }
        }
        else if (other.CompareTag("HealthDrop"))
        {
            if (Health <= 80) { Health += 20; Destroy(other.gameObject);}
            else if (Health > 80 && Health <100) { Health = 100; Destroy(other.gameObject); }
            StartCoroutine(HealthFeedback(UM.HealthGold));
        }
        else if (other.CompareTag("SuperDrop"))
        {
            if (PC.SuperEnergyCharges < 9) { PC.SuperEnergyCharges++; Destroy(other.gameObject); }
        }
        else if (other.CompareTag("TankMelee") && SuperMeleeImmune == false)
        {
            Health -= 20 - DamageReduction;

            Effects_Manager BEM;
            BEM = other.GetComponentInParent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; }
            else if (BEM.IceEffect) { EM.IsFrozen = true; }
            else if (BEM.VoidEffect) { EnemyVoidEffect(other.gameObject); }
            else if (BEM.AirEffect) { EnemyAirEffect(other.gameObject); }
            else { return; }
        }
    }

    IEnumerator HitFeedback(GameObject Effect)
    {
        Effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Effect.SetActive(false);
    }
    IEnumerator HealthFeedback(GameObject Effect)
    {
        Effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Effect.SetActive(false);
    }

    public void TakeDamage(int amt)
    {
        Health -= amt;

    }

}
