using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugscript : MonoBehaviour
{
    GameObject player;
    GameObject UI;

    public GameObject PlayerManager, PlayerCombat, UIManager;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.FindGameObjectWithTag("UI");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerManager>().enabled) { PlayerManager.SetActive(true); } else { PlayerManager.SetActive(false); }
        if (player.GetComponent<PlayerCombat>().enabled) { PlayerCombat.SetActive(true); } else { PlayerCombat.SetActive(false); }
        if (UI.GetComponent<UI_Manager>().enabled) { UIManager.SetActive(true); } else { UIManager.SetActive(false); }
    }
}
