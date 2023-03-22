using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class VendorManager : MonoBehaviour
{

    PlayerManager PM;
    PlayerCam PCam;
    PlayerCombat PC;
    GameObject Player;




    public bool On;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PM = Player.GetComponent<PlayerManager>();
        PC = Player.GetComponent<PlayerCombat>();
        PCam = Player.GetComponentInChildren<PlayerCam>();
    }






    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            On = true;           
        }
    }

}
