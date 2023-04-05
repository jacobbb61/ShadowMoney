using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class EndOfLevel : MonoBehaviour
{
    GameManager GM;
    GameObject Player;
    GameObject UI;
    PlayerManager PM;
    PlayerCombat PC;
    UI_Manager UM;
    Animator Anim;

    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.FindGameObjectWithTag("UI");
        PM = Player.GetComponent<PlayerManager>();
        PC = Player.GetComponent<PlayerCombat>();
        UM = UI.GetComponent<UI_Manager>();
        Anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ExitAnim());
            other.gameObject.SetActive(false);
            GM.Music.SetActive(false);
        }
    }

    IEnumerator ExitAnim()
    {
        Anim.Play("BallistaFire");
        yield return new WaitForSeconds(13f);
        OpenUI();
    }

    void OpenUI()
    {
        GM.Base.volume = 0.1f; GM.Base1.volume = 0.1f; GM.Base2.volume = 0.1f; //audio muffle 
        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;
        Player.GetComponentInChildren<PlayerCam>().enabled = false;
        PC.enabled = false;
        UM.OpenEndOfLevel();
    }

}
