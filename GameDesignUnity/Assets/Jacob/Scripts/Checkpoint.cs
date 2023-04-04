using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager Gm;
    public GameObject Fire;

    void Start()
    {
        Gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        FireCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player")) 
        { 
         Gm.LastCheckPoint = gameObject.transform.position;
            FireCheck();
        }
    }
    private void FireCheck()
    {
        if (Gm.LastCheckPoint == gameObject.transform.position) { Fire.SetActive(true); }
        else { Fire.SetActive(false); }
    }
}
