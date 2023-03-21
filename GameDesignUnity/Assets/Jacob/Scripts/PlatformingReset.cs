using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingReset : MonoBehaviour
{
    public GameObject ResetPos;
    GameObject Player;
    GameManager GM;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.GetComponent<CharacterController>().enabled = false;
            Teleport();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    public void Teleport()
    {
        Player.transform.position = GM.PlatformingResetPos.transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        Debug.Log("A");
    }

}
