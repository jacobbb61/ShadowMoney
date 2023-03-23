using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    GameObject Player;
    GameObject UI;
    PlayerManager PM;
    PlayerCombat PC;
    UI_Manager UM;
    GameMemory Mem;
    public PlayerCam PCam;
    public GameObject PlatformingResetPos;

    [Header("Unlocked Projectiles")]
    public bool UnlockedProjectileType1;
    public bool UnlockedProjectileType2;
    public bool UnlockedProjectileType3;
    public bool UnlockedProjectileType4;


    [Header("Unlocked Elements")]
    public bool UnlockedFire;
    public bool UnlockedIce;
    public bool UnlockedAir;
    public bool UnlockedVoid;
    public bool UnlockedSuperPunch;

    [Header("CheckPoint and Doors")]
    public Vector3 LastCheckPoint;
    public bool L1_DoorA;
    public bool L1_DoorB;
    public bool L2_DoorA;
    public bool L2_DoorB;
    public bool L3_DoorA;
    public bool L3_DoorB;

    [Header("Options Memory")]
    public bool OptionsMusic;
    public bool OptionsTutorial;
    public int SensX;
    public int SensY;


    [Header("Combat Audio")]
    public GameObject Music;
    public AudioSource Base;
    public AudioSource Base1;
    public AudioSource Base2;
    public GameObject Enter1;
    public GameObject Enter2;
    public GameObject Enter3;
    public GameObject Exit;


    [Header("MainMenu")]
    public bool GameStarted;
    public int LatestLevel=1;
    public string LevelToLoad;


    private GameObject TutorialList;

    void Start()
    {

        Player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.FindGameObjectWithTag("UI");
        PM = Player.GetComponent<PlayerManager>();
        PC = Player.GetComponent<PlayerCombat>();
        UM = UI.GetComponent<UI_Manager>();
        PCam = Player.GetComponentInChildren<PlayerCam>();
        Time.timeScale = 1f;
       
        RespawnPlayer();
        if (SceneManager.GetActiveScene().name == "Level1") { LatestLevel = 1; }
        else if (SceneManager.GetActiveScene().name == "Level2") { LatestLevel = 2; }
        else if (SceneManager.GetActiveScene().name == "Level3") { LatestLevel = 3; }
    }
 
    public void RespawnPlayer()
    {
        Debug.Log("A");
        UpdateTutorials();
        MusicState();
        PCam.UpdateSensX(SensX);
        PCam.UpdateSensY(SensY);
        Player.transform.position = LastCheckPoint; 
        Debug.Log("Spawn");
    }

    public void InCombat()
    {
        Enter1.SetActive(false);
        Enter2.SetActive(false);
        Enter3.SetActive(false);

        Enter1.SetActive(true);
        Debug.Log("1");
        StartCoroutine(SoundFX());
        
       
        Base.volume = 0.5f;
        Base1.volume = 0.5f;
        Base2.volume = 0.5f;      
        Exit.SetActive(false);
    }

    IEnumerator SoundFX()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("2");
        Enter2.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("3");
        Enter3.SetActive(true);
    }
    public void OutCombat()
    {
        Base.volume = 0.4f;
        Base1.volume = 0;
        Base2.volume = 0;
        Enter1.SetActive(false);
        Enter2.SetActive(false);
        Enter3.SetActive(false);
        Exit.SetActive(true);
    }

    public void MusicState()
    {
        if (OptionsMusic) { Music.SetActive(true); }
        else { Music.SetActive(false); }
    }

    public void UpdateTutorials()
    {
        if (TutorialList == null) { TutorialList = GameObject.FindGameObjectWithTag("Tutorial"); }
        TutorialList.SetActive(OptionsTutorial);

    }
}
