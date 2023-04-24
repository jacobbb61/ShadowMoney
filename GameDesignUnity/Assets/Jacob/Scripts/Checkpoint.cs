using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager Gm;
    public GameObject Fire;
    public bool used = false;

    void Start()
    {
        Gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        FireCheck();
        used = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player")) 
        { 
         Gm.LastCheckPoint = gameObject.transform.position;
            if (used == false)
            {
                other.GetComponent<PlayerManager>().Health = 100;
                other.GetComponent<PlayerCombat>().SuperEnergyCharges++;
                other.GetComponent<PlayerCombat>().SuperEnergyCharges++;
                other.GetComponent<PlayerCombat>().SuperEnergyCharges++;
                used = true;
            }
            FireCheck();
        }
    }
    private void FireCheck()
    {
        if (Gm.LastCheckPoint == gameObject.transform.position) { Fire.SetActive(true); }
        else { Fire.SetActive(false); }
    }
}
