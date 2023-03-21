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

    [Header("Custom objects")]
    public bool On;
    public RenderTexture Video;
    public string MainText;
   
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
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (On&&context.action.triggered) { CloseUI(); }
    }


    public void OpenUI()
    {
        MainExitPromptUI.SetActive(false);
        MainUI.SetActive(true);
        MainTextUI.text = MainText;
        MainVideoUI.GetComponent<RawImage>().texture = Video;
        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;
        UIM.GetComponent<PlayerInput>().enabled = false;
        PCam.enabled = false;
        PC.enabled = false;
        StartCoroutine(WaitToExit());
        Time.timeScale = 0.1f; 
    }

    IEnumerator WaitToExit()
    {  
        yield return new WaitForSeconds(0.1f);
        MainExitPromptUI.SetActive(true);
        On = true;
    }

    public void CloseUI()
    {
        MainUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = true;
        Player.GetComponent<PlayerInput>().enabled = true;
        UIM.GetComponent<PlayerInput>().enabled = true;
        PCam.enabled = true;
        PC.enabled = true;
        Time.timeScale = 1f;
        PM.Paused = true;
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
