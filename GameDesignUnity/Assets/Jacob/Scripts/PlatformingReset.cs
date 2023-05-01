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
            if(GM.PlatformingResetPos == null) { GM.PlatformingResetPos = ResetPos; }
            Teleport();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    public void Teleport()
    {
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Player.transform.position = GM.PlatformingResetPos.transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        if (Player.GetComponent<PlayerManager>().Health >10) { Player.GetComponent<PlayerManager>().Health -= 10; }
        Debug.Log("A");
    }

}
