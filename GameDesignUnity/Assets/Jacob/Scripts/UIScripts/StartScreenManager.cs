using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class StartScreenManager : MonoBehaviour
{

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.action.triggered) { SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }
    }
}
