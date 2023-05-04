using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    PlayerManager PM;
    PlayerCam PCam;
    PlayerCombat PC;
    GameObject Player;
    UI_Manager UM;
    GameObject UIM;
    GameManager GM;

    [Header("Custom objects")]
    public bool On;
    public RenderTexture Video;
    public string MainText;
    public bool T1;
    public bool T2;
    public bool T3;
    public bool T4;

    //got from ui manager
    private GameObject MainUI;
    private TextMeshProUGUI MainTextUI;
    private GameObject MainVideoUI;
    private GameObject MainExitPromptUI;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PM = Player.GetComponent<PlayerManager>();
        PC = Player.GetComponent<PlayerCombat>();
        PCam = Player.GetComponentInChildren<PlayerCam>();
        UIM = GameObject.FindGameObjectWithTag("UI");
        UM = UIM.GetComponent<UI_Manager>();
        MainUI = UM.MainTutorialUI;
        MainTextUI = UM.MainTextUI;
        MainVideoUI = UM.MainVideoUI;
        MainExitPromptUI = UM.MainExitPromptUI;
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        memory();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (On&&context.action.triggered) { CloseUI(); }
    }

    public void memory()
    {
        if (T1) { if (GM.Tutorial1 == true) { Destroy(this.gameObject); } }
        if (T2) { if (GM.Tutorial2 == true) { Destroy(this.gameObject); } }
        if (T3) { if (GM.Tutorial3 == true) { Destroy(this.gameObject); } }
        if (T4) { if (GM.Tutorial4 == true) { Destroy(this.gameObject); } }
    }
    public void OpenUI()
    {
        MainExitPromptUI.SetActive(false);
        MainUI.SetActive(true);
        MainTextUI.text = MainText;
        MainVideoUI.GetComponent<RawImage>().texture = Video;
        Time.timeScale = 0.0001f;
        StartCoroutine(WaitToExit());
        PM.SuperMeleeImmune = true;
 
    }

    IEnumerator WaitToExit()
    {  
        yield return new WaitForSeconds(0.0001f);
        MainExitPromptUI.SetActive(true);
        On = true;
    }

    public void CloseUI()
    {
        PM.SuperMeleeImmune = false;
        MainUI.SetActive(false);
        Time.timeScale = 1f;
        if (T1) { if (GM.Tutorial1 == false) { GM.Tutorial1 = true; } }
        if (T2) { if (GM.Tutorial2 == false) { GM.Tutorial2 = true; } }
        if (T3) { if (GM.Tutorial3 == false) { GM.Tutorial3 = true; } }
        if (T4) { if (GM.Tutorial4 == false) { GM.Tutorial4 = true; } }
        this.gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenUI();
        }
    }
}
