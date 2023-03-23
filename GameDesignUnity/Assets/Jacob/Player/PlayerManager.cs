using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IDamageable
{


    public CharacterController controller;
    public PlayerCombat PC;
    public GameManager GM;
    public Effects_Manager EM;
    public UI_Manager UM;
    private bool jumped = false;
    private bool dash = false;
    private bool CanWalk = true;
    private bool dashTS;


    

    private Vector3 playerVelocity;
    private Vector3 dashV;
    private Vector2 movementInput = Vector2.zero;

    private float gravityValue = -25f;
    private int DamageReduction = 0;
    

    [Header("Player stats")]
    public float playerSpeed = 10f;
    public float jumpHeight = 6.0f;
    public float dashSpeed;
    public bool groundedPlayer;
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
        controller = gameObject.GetComponent<CharacterController>();
        PC = GetComponent<PlayerCombat>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UM = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Manager>();
        EM = GetComponent<Effects_Manager>();
        //GM.RespawnPlayer();
        Health = 100;
        Time.timeScale = 1;
        
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

        Transform cameraTransform = Camera.main.transform;
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100.0f))
        {
            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank")) 
            {
                UM.CrossHairOverEnemy.SetActive(true);
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

        if (groundedPlayer && playerVelocity.y < 0f) { playerVelocity.y = 0f; } 

        if (jumped && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 3f))
        {

            if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Wall")) { playerVelocity.y = -5f;  }

        }

    }
    public void Move()
    {
        if (controller.enabled)
        {
            Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

            if (CanWalk == true) { controller.Move(move * Time.deltaTime * playerSpeed); }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
    public void Dash()
    {
        if ((dash == true) && (dashT == 0f)) { dashT = -2f; dashTS = true; dashV = transform.forward * dashSpeed; }
        if (dashTS == true) { dashT += Time.deltaTime; }
        if ((dashT < -1.5f) && (dashTS == true)) { controller.Move(dashV * Time.deltaTime); gravityValue = -20f; CanWalk = false; }
        if (dashT < -1.5f) { dash = false; gravityValue = -25f; CanWalk = true; }
        if (dashT > 0f) { dashT = 0f; dashTS = false; }
    }
    public void MeleePull()
    {
        if (Vector3.Distance(transform.position, PC.MeleePullDestination.transform.position) >= 5) { controller.Move(transform.forward * Time.deltaTime * 20); }
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
        if (PC.SelfEffectTime >= 5f) { PC.SelfAir = false; gravityValue = -25f;  PC.SelfEffectTime = 0f; }
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
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet_Manager BM;
            BM = other.GetComponent<Bullet_Manager>();
            Health -= BM.Damage - DamageReduction;
            Debug.Log("Hit");

            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) { EM.IsBurning = true; }
            else if (BEM.IceEffect) { EM.IsFrozen = true; }
            else if (BEM.VoidEffect) { EnemyVoidEffect(other.gameObject); }
            else if (BEM.AirEffect) { EnemyAirEffect(other.gameObject); }
            else { return; }
        }
        else if (other.CompareTag("HealthDrop"))
        {
            if (Health <= 80) { Health += 20; Destroy(other.gameObject);}
            else if (Health > 80 && Health <100) { Health = 100; Destroy(other.gameObject); }           
        }
        else if (other.CompareTag("SuperDrop"))
        {
            if (PC.SuperEnergyCharges < 9) { PC.SuperEnergyCharges++; Destroy(other.gameObject); }
        }
    }

    public void TakeDamage(int amt)
    {
        Health -= amt;
    }

}
