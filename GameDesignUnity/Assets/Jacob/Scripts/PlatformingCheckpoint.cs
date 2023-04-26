using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingCheckpoint : MonoBehaviour
{
    GameManager GM;
    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GM.PlatformingResetPos = gameObject;
        }
    }
}
