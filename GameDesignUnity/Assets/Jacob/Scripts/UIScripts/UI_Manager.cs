using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class UI_Manager : MonoBehaviour
{
    GameObject Player;
    PlayerManager PM;
    PlayerCombat PC;
    PauseMenuManager PauseUI;
    GameManager GM;

    Vector2 StickInput;    
    bool reset;

    [Header("TopBar")]
    public GameObject TopBarMain;
    public Slider PlayerHPSLider;
    public Slider PlayerDashSLider;
    public Slider PlayerSuperBar;

    [Header("Orb")]
    public GameObject OrbMain;
    public GameObject ElementUI;
    public GameObject CurrentElementHightlight;

    [Header("Hit Feedback")]
    public GameObject FireHit;
    public GameObject IceHit;
    public GameObject VoidHit;
    public GameObject AirHit;

    [Header("CrossHair")]
    public GameObject CrossHairOverEnemy;
    public GameObject CrossHairMeleeRange;

    [Header("Pause Menus")]
    public GameObject PauseMenu;
    public GameObject MainButtons;
    public GameObject MainHighlight;
    public bool Paused;
    public int MainOrder;
    public int PauseState;

    [Header("Tutorial")]
    public GameObject MainTutorialUI;
    public TextMeshProUGUI MainTextUI;
    public GameObject MainVideoUI;
    public GameObject MainExitPromptUI;

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

    [Header("End Of Level")]
    public bool EndOfLevelOn;
    public int EndOrder;
    public GameObject EndOfLevelUI;
    public GameObject EndHighlight;

    [Header("You Died")]
    public bool YouDiedOn;
    public int DiedOrder;
    public GameObject YouDiedUI;
    public GameObject YouDiedHighlight;


    [Header("Unlocks")]
    //public GameObject UnlockedSuperPunch;
    public GameObject UnlockedFire;
    public GameObject UnlockedIce;
    public GameObject UnlockedAir;
    public GameObject UnlockedVoid;
    public GameObject UnlockedUIPopUp;
    public TextMeshProUGUI UnlockedUIPopUpText;

    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        PauseUI = GetComponentInChildren<PauseMenuManager>();
        Paused = false;
        if (GM.OptionsMusic) { OpsMusicON.SetActive(true); OpMusicOFF.SetActive(false); } else { OpsMusicON.SetActive(false); OpMusicOFF.SetActive(true); }
        if (GM.OptionsTutorial) { OpTutorialON.SetActive(true); OpTutorialOFF.SetActive(false); } else { OpTutorialON.SetActive(false); OpTutorialOFF.SetActive(true); }
        OpSensX.text = GM.SensX.ToString();
        OpSensY.text = GM.SensY.ToString();
        DiedOrder = 1;
        UpdateUnlocked();
        Time.timeScale = 1f;
    }

    public void UpdateUnlocked()
    {
        if (GM.UnlockedFire) { UnlockedFire.SetActive(false); }
        if (GM.UnlockedIce) { UnlockedIce.SetActive(false); }
        if (GM.UnlockedAir) { UnlockedAir.SetActive(false); }
        if (GM.UnlockedVoid) { UnlockedVoid.SetActive(false); }
      //  if (GM.UnlockedSuperPunch) { UnlockedSuperPunch.SetActive(false); }
    }


    public void Pause(InputAction.CallbackContext context)
    {
        if (context.action.triggered && EndOfLevelOn==false && YouDiedOn==false) { Paused = !Paused; }
            if (Paused)
            {
                Pause();
        }
            else
            {
                UnPause();
        }    
    }
    public void Move(InputAction.CallbackContext context)
    {

        StickInput = context.ReadValue<Vector2>();
        if (Paused)
        {
            if (PauseState == 0)
            {
                if (StickInput.y > 0.5 && reset == true) { MainOrder--; reset = false; }
                if (StickInput.y < -0.5 && reset == true) { MainOrder++; reset = false; }
                if (StickInput.y == 0) { reset = true; }
                MoveMainHighlight(0, 100, MainHighlight);
            }
            if (PauseState == 1)
            {
                if (StickInput.y > 0.5 && reset == true) { OpOrder--; reset = false; }
                if (StickInput.y < -0.5 && reset == true) { OpOrder++; reset = false; }
                if (StickInput.y == 0) { reset = true; }
                MoveOptionsHighlight(0, 150, OpHighlight);
                MoveOptions();
            }
        } 
        else  if (YouDiedOn)
        {
            if (StickInput.y > 0.5 && reset == true) { Debug.Log("2"); DiedOrder++; reset = false; }
            if (StickInput.y < -0.5 && reset == true) {Debug.Log("1"); DiedOrder--; reset = false;  }
            if (StickInput.y == 0) { reset = true; }
            MoveDiedHighlight(0, 150, YouDiedHighlight);
        }
        else if (EndOfLevelOn)
        {
            if (StickInput.y > 0.5 && reset == true) { EndOrder--; reset = false; }
            if (StickInput.y < -0.5 && reset == true) { EndOrder++; reset = false; }
            if (StickInput.y == 0) { reset = true; }
            MoveEndHighlight(0, 200, EndHighlight);
        }

    }
    public void Abutton(InputAction.CallbackContext context)
    {
        if (Paused)
        {
            if (MainOrder == 1) { UnPause(); }//resume
            else if (MainOrder == 2) { OpenOptions(); }//options
            else if (MainOrder == 3) { ExitGame(); }//exit
        }
            if (EndOfLevelOn) { SelectEndOfLevel(); }
            if (YouDiedOn) { SelectYouDied(); }
    }
    public void Bbutton(InputAction.CallbackContext context)
    {
        if (Paused)
        {
            if (PauseState == 1) { CloseOptions(); }
        }
    }

    public void Up(InputAction.CallbackContext context)
    {
        if (Paused)
        {
            if (context.action.triggered && PauseState == 0) { MainOrder--; MoveMainHighlight(0, 100, MainHighlight); }
            if (context.action.triggered && PauseState == 1) { OpOrder--; MoveOptionsHighlight(0, 150, OpHighlight); }
        }
       if (context.action.triggered &&  YouDiedOn) { DiedOrder--; MoveDiedHighlight(0, 150, YouDiedHighlight); }
       if (context.action.triggered &&  EndOfLevelOn) { EndOrder--; MoveEndHighlight(0, 200, EndHighlight); }
    }
    public void Down(InputAction.CallbackContext context)
    {
        if (Paused)
        {
            if (context.action.triggered && PauseState == 0) { MainOrder++; MoveMainHighlight(0, 100, MainHighlight); }
            if (context.action.triggered && PauseState == 1) { OpOrder++; MoveOptionsHighlight(0, 150, OpHighlight); }
        }
        if (context.action.triggered &&  YouDiedOn) { DiedOrder++; MoveDiedHighlight(0, 150, YouDiedHighlight); }
        if (context.action.triggered &&  EndOfLevelOn) { EndOrder++; MoveEndHighlight(0, 200, EndHighlight); }
    }
    public void Left(InputAction.CallbackContext context)
    {
        
    }
    public void Right(InputAction.CallbackContext context)
    {

    }

    public void ExitGame()
    {
        Time.timeScale = 1;
        LoadScene("MainMenu");
    }

    public void Pause()
    {
        if (GM.Exit.activeInHierarchy) {GM.Base.volume = 0.1f; }
        else { GM.Base.volume = 0.1f; GM.Base1.volume = 0.1f; GM.Base2.volume = 0.1f; }           
        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;
        Player.GetComponentInChildren<PlayerCam>().enabled = false;
        PC.enabled = false;
        Paused = true;
        Time.timeScale = 0.01f;
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void UnPause()
    {
        Debug.Log("Unpaused");
        if (GM.Exit.activeInHierarchy) { GM.Base.volume = 0.2f; }
        else { GM.Base.volume = 0.3f; GM.Base1.volume = 0.3f; GM.Base2.volume = 0.3f; }
        Player.GetComponent<CharacterController>().enabled = true;
        Player.GetComponent<PlayerInput>().enabled = true;
        Player.GetComponentInChildren<PlayerCam>().enabled = true;
        PC.enabled = false;
        Paused = false;
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }




    public void OpenOptions()
    {
        PauseState = 1;
        MainOptions.SetActive(true);
        MainButtons.SetActive(false);
    }
    public void CloseOptions()
    {
        PauseState = 0;
        MainOptions.SetActive(false);
        MainButtons.SetActive(true);
     
    }
    public void MoveOptions()
    {
            if (OpOrder == 1)
            {
            if (StickInput.x < -0.3 && reset == true) { OptionsTutotial(); reset = false; }
            if (StickInput.x > 0.3 && reset == true) { OptionsTutotial(); reset = false; }
            if (StickInput.x == 0) { reset = true; }
            GM.UpdateTutorials();
        }
            if (OpOrder == 2)
            {
            if (StickInput.x < -0.3 && reset == true) { OptionsMusic(); reset = false; }
            if (StickInput.x > 0.3 && reset == true) { OptionsMusic(); reset = false; }
            if (StickInput.x == 0) { reset = true; }
            GM.MusicState();
            }
            if (OpOrder == 3)
            {
            if (StickInput.x < -0.3 && reset == true && GM.SensX > -20) { GM.SensX--;  reset = false; }
            if (StickInput.x > 0.3 && reset == true && GM.SensX < 20) { GM.SensX++;  reset = false; }
            if (StickInput.x == 0) { reset = true; }
            OpSensX.text = GM.SensX.ToString();
            GM.PCam.UpdateSensX(GM.SensX);
            }
            if (OpOrder == 4)
            {
            if (StickInput.x < -0.3 && reset == true && GM.SensY > -20) { GM.SensY--; reset = false; }
            if (StickInput.x > 0.3 && reset == true && GM.SensY < 20) { GM.SensY++; reset = false; }
            if (StickInput.x == 0) { reset = true; }
            OpSensY.text = GM.SensY.ToString();
            GM.PCam.UpdateSensY(GM.SensY);
        }
    }

    public void OptionsTutotial()
    {
        GM.OptionsTutorial = !GM.OptionsTutorial; 
        OpTutorialON.SetActive(!OpTutorialON.activeInHierarchy);        
        OpTutorialOFF.SetActive(!OpTutorialOFF.activeInHierarchy);
        GM.UpdateTutorials();
    }

    public void OptionsMusic()
    {
        GM.OptionsMusic = !GM.OptionsMusic;
        OpsMusicON.SetActive(!OpsMusicON.activeInHierarchy);
        OpMusicOFF.SetActive(!OpMusicOFF.activeInHierarchy);
        GM.MusicState();
    }
    public void OptionsPlusSensX()
    {
        if (GM.SensX < 99) { GM.SensX++; }
        OpSensX.text = GM.SensX.ToString();
        GM.PCam.UpdateSensX(GM.SensX);
    }
    public void OptionsMinusSensX()
    {
        if (GM.SensX > -99) { GM.SensX--; }
        OpSensX.text = GM.SensX.ToString();
        GM.PCam.UpdateSensX(GM.SensX);
    }
        public void OptionsPlusSensY()
    {
        if (GM.SensY < 99) { GM.SensY++; }
        OpSensY.text = GM.SensY.ToString();
        GM.PCam.UpdateSensY(GM.SensY);
    }
    public void OptionsMinusSensY()
    {
        if (GM.SensY > -99) { GM.SensY--; }
        OpSensY.text = GM.SensY.ToString();
        GM.PCam.UpdateSensY(GM.SensY);
    }
    public void OpenEndOfLevel()
    {
        Cursor.lockState = CursorLockMode.Confined;
        EndOfLevelOn = true;
        EndOrder = 1;
        EndOfLevelUI.SetActive(true);
        OrbMain.SetActive(false);
        TopBarMain.SetActive(false);
    }
    public void SelectEndOfLevel()
    {
        if (EndOrder == 1) //next level
        {
            SelectNextLevel();
        }
        else //main menu
        {
            LoadScene("MainMenu");
        }
    }
    public void SelectNextLevel()
    { 

if (SceneManager.GetActiveScene().name == "Level1") { LoadScene("Level2"); }
            else if (SceneManager.GetActiveScene().name == "Level2") { LoadScene("Level3"); }
            else if (SceneManager.GetActiveScene().name == "Level3") { LoadScene("MainMenu"); }
    }


        public void OpenYouDied()
    {
        Cursor.lockState = CursorLockMode.Confined;
        YouDiedOn = true;       
        YouDiedUI.SetActive(true);
        GM.Base.volume = 0.1f; GM.Base1.volume = 0.1f; GM.Base2.volume = 0.1f; //audio muffle 
        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;
        Player.GetComponentInChildren<PlayerCam>().enabled = false;
        PC.enabled = false;
        Time.timeScale = 0.1f;
    }
    public void SelectYouDied()
    {
        if (DiedOrder == 1) //respawn
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else //main menu
        {
            Time.timeScale = 1;
            LoadScene("MainMenu");
        }
    }
    public void SelectRespawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void MoveMainHighlight(float X, float Y, GameObject Highlight)
    {
        if (MainOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (MainOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (MainOrder == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (MainOrder == 4) { MainOrder = 1; }
        else if (MainOrder == 0) { MainOrder = 3; }
    }
    public void MoveOptionsHighlight(float X, float Y, GameObject Highlight)
    {
        if (OpOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y); }
        else if (OpOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (OpOrder == 3) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (OpOrder == 4) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y -Y); }
        else if (OpOrder == 5) { OpOrder = 1; }
        else if (OpOrder == 0) { OpOrder = 5; }
    }
    public void MoveEndHighlight(float X, float Y, GameObject Highlight)
    {
        if (EndOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); }
        else if (EndOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); }
        else if (EndOrder == 3) { EndOrder = 1; }
        else if (EndOrder == 0) { EndOrder = 2; }
    }
    public void MoveDiedHighlight(float X, float Y, GameObject Highlight)
    {
        if (DiedOrder == 1) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, 0); Debug.Log("11"); }
        else if (DiedOrder == 2) { Highlight.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, -Y); Debug.Log("22"); }
        else if (DiedOrder == 3) { DiedOrder = 1; }
        else if (DiedOrder == 0) { DiedOrder = 2; }
    }


    public void LoadScene(string scenename)
    {
        GM.LevelToLoad = scenename;
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
    }
}
