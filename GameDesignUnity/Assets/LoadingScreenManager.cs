using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public string LevelToLoad;
    GameManager GM;
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (GM.LevelToLoad != null)
        {
            LevelToLoad = GM.LevelToLoad;
            StartCoroutine(LoadScene());
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        
    }
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(LevelToLoad, LoadSceneMode.Single);
    }

 }
