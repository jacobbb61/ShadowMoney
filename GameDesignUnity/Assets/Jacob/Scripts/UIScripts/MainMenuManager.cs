using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{

    GameManager GM;

    public int Order;
    public int LevelOrder;
    public int State;

    [Header("States")]
    public GameObject MainState;
    public GameObject LevelSelect;
    public GameObject Options;

    [Header("Hightlights")]
    public GameObject MainStateHighlight;
    public GameObject LevelSelectHighlight;

    [Header("Options")]
    public int OpOrder;
    public GameObject MainOptions;
    public GameObject OpHighlight;
    public GameObject OpTutorialON;
    public GameObject OpTutorialOFF;
    public GameObject OpsMusicON;
    public GameObject OpMusicOFF;
    public TextMeshProUGUI OpSensX;
    public TextMeshProUGUI OpSensY;


    public TextMeshProUGUI StartGame;


    Vector2 StickInput;
    bool reset;
    bool Buttonreset;

    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        State = 1;
        Order = 1;
        Time.timeScale = 1f;
        if (GM.OptionsMusic) { OpsMusicON.SetActive(true); OpMusicOFF.SetActive(false); } else { OpsMusicON.SetActive(false); OpMusicOFF.SetActive(true); }
        if (GM.OptionsTutorial) { OpTutorialON.SetActive(true); OpTutorialOFF.SetActive(false); } else { OpTutorialON.SetActive(false); OpTutorialOFF.SetActive(true); }
        OpSensX.text = GM.SensX.ToString();
        OpSensY.text = GM.SensY.ToString();
        if (GM.GameStarted == true) { StartGame.text = "Continue"; } else { StartGame.text = "StartGame"; }
    }
    public void Move(InputAction.CallbackContext context)
    {
        Buttonreset = true;
        StickInput = context.ReadValue<Vector2>();
        if (State ==1 )
        {
            if (StickInput.y > 0.5 && reset == true) { Order--; reset = false; }
            if (StickInput.y < -0.5 && reset == true) { Order++; reset = false; }
            if (StickInput.y == 0) { reset = true; }
             MoveHighlight(0, 200, MainStateHighlight); 

        }
        if (State == 2)
        {
            if (StickInput.y > 0.5 && reset == true) { LevelOrder--; reset = false; }
            if (StickInput.y < -0.5 && reset == true) { LevelOrder++; reset = false; }
            if (StickInput.y == 0) { reset = true; }
            MoveLevelHighlight(750, 445, LevelSelectHighlight); 
        }

        if (State == 3)
        {
            if (StickInput.y > 0.5 && reset == true) { OpOrder--; reset = false; }
            if (StickInput.y < -0.5 && reset == true) { OpOrder++; reset = false; }
            if (StickInput.y == 0) { reset = true; }
            MoveOptionsHighlight(0, 150, OpHighlight);
            MoveOptions();
        }
    }

    public void Abutton(InputAction.CallbackContext context)
    {
        if (State == 1) { MainSelectButtonA(); }
        if (State == 2 && Buttonreset==true) { LevelSelectButtonA(); }
    }
    public void Bbutton(InputAction.CallbackContext context)
    {
        if (State == 2) { LevelSelectButtonB(2); }
        if (State == 3) { LevelSelectButtonB(3); }
    }

    public void MoveHighlight(float X, float Y, GameObject Highlight)
    {
        if (Order == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (Order == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (Order == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (Order == 4 ) { Order = 1; }
        else if (Order == 0 ) { Order = 3; }
    }

    public void MainSelectButtonA()
    {
        
        if (Order == 1) 
        { 
            GM.GameStarted = true;
            if (GM.LatestLevel == 1) { LoadScene("Level1"); }
            if (GM.LatestLevel == 2) { LoadScene("Level2"); }
            if (GM.LatestLevel == 3) { LoadScene("Level3"); }       
        } 
        if (Order == 2) { MainState.SetActive(false); LevelSelect.SetActive(true); State = 2; Buttonreset = false; }
        if (Order == 3) { MainState.SetActive(false); Options.SetActive(true); State = 3; }
    }

    public void LevelSelectButtonA()
    {
         if (LevelOrder == 1) { LoadScene("Level1"); }
         if (LevelOrder == 2) { LoadScene("Level2"); }
         if (LevelOrder == 3) { LoadScene("Level3"); }
        GM.L1_DoorA = false;
        GM.L1_DoorB = false;
        GM.L2_DoorA = false;
        GM.L2_DoorB = false;
        GM.L3_DoorA = false;
        GM.L3_DoorB = false;
    }
    public void LevelSelectButtonB(int num)
    {
        MainState.SetActive(true); 
        LevelSelect.SetActive(false);
        Options.SetActive(false);
        State = 1;
        Order = num;
    }
    public void MoveLevelHighlight(float X, float Y, GameObject Highlight)
    {
        if (LevelOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (LevelOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (LevelOrder == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (LevelOrder == 4) { LevelOrder = 1; }
        else if (LevelOrder == 0) { LevelOrder = 3; }
    }

    public void MoveOptions()
    {
        if (OpOrder == 1)
        {
            if (StickInput.x < -0.3 && reset == true) { GM.OptionsTutorial = true; OpTutorialON.SetActive(true); OpTutorialOFF.SetActive(false); reset = false; }
            if (StickInput.x > 0.3 && reset == true) { GM.OptionsTutorial = false; OpTutorialON.SetActive(false); OpTutorialOFF.SetActive(true); reset = false; }
            if (StickInput.x == 0) { reset = true; }
            GM.UpdateTutorials();
        }
        if (OpOrder == 2)
        {
            if (StickInput.x < -0.3 && reset == true) { GM.OptionsMusic = true; OpsMusicON.SetActive(true); OpMusicOFF.SetActive(false); reset = false; }
            if (StickInput.x > 0.3 && reset == true) { GM.OptionsMusic = false; OpsMusicON.SetActive(false); OpMusicOFF.SetActive(true); reset = false; }
            if (StickInput.x == 0) { reset = true; }
            GM.MusicState();
        }
        if (OpOrder == 3)
        {
            if (StickInput.x < -0.3 && reset == true && GM.SensX > 1) { GM.SensX--; reset = false; }
            if (StickInput.x > 0.3 && reset == true && GM.SensX < 9) { GM.SensX++; reset = false; }
            if (StickInput.x == 0) { reset = true; }
            OpSensX.text = GM.SensX.ToString();
            GM.PCam.UpdateSensX(GM.SensX);
        }
        if (OpOrder == 4)
        {
            if (StickInput.x < -0.3 && reset == true && GM.SensY > 1) { GM.SensY--; reset = false; }
            if (StickInput.x > 0.3 && reset == true && GM.SensY < 9) { GM.SensY++; reset = false; }
            if (StickInput.x == 0) { reset = true; }
            OpSensY.text = GM.SensY.ToString();
            GM.PCam.UpdateSensY(GM.SensY);
        }
    }
    public void MoveOptionsHighlight(float X, float Y, GameObject Highlight)
    {
        if (OpOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (OpOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (OpOrder == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (OpOrder == 4) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y - Y); }
        else if (OpOrder == 5) { OpOrder = 1; }
        else if (OpOrder == 0) { OpOrder = 5; }
    }


    public void LoadScene(string scenename)
    {
        GM.LevelToLoad = scenename;
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
    }
}
