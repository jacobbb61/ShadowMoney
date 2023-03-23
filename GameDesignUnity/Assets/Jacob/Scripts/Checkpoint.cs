using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager Gm;

    void Start()
    {
        Gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player")) 
        { 
         Gm.LastCheckPoint = gameObject.transform.position; 
        }
    }
}
