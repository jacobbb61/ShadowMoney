using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    GameManager GM;
    GameObject Player;
    GameObject UI;
    PlayerManager PM;
    PlayerCombat PC;
    UI_Manager UM;


    public int Order;
    public GameObject Highlight;

    Vector2 StickInput;
    bool reset;
    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.FindGameObjectWithTag("UI");
        PM = Player.GetComponent<PlayerManager>();
        PC = Player.GetComponent<PlayerCombat>();
        UM = UI.GetComponent<UI_Manager>();
    }



    public void Move(InputAction.CallbackContext context)
    {

        StickInput = context.ReadValue<Vector2>();

        if (StickInput.y > 0.3 && reset == true) { Order--; reset = false; }
        if (StickInput.y < -0.3 && reset == true) { Order++; reset = false; }
        if (StickInput.y == 0) { reset = true; }
        MoveHighlight(0,100);
    }

    public void Abutton(InputAction.CallbackContext context)
    {
         if (Order == 1) { UM.UnPause(); }//resume
         if (Order == 2) { }//options
         if (Order == 3) { SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }//exit
    }

    public void MoveHighlight(float X, float Y)
    {
        if (Order == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (Order == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (Order == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (Order == 4) { Order = 1; }
        else if (Order == 0) { Order = 3; }
    }


}
