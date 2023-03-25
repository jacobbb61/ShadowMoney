using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    GameManager GM;
    public GameObject Door;
    public bool Unlocked;
    public bool IsDoorA;
    public int Level;

    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        CheckMemory();
        if (Unlocked) { StartCoroutine(Open()); GetComponent<BoxCollider>().enabled = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
       StartCoroutine(Open());
    }

    void CheckMemory()
    {
        if (Level == 1)
        {
            if (IsDoorA) { if (GM.L1_DoorA == true) { Unlocked = true; } }
            else { if (GM.L1_DoorB == true) { Unlocked = true; } }
        }
        else if (Level == 2)
        {
            if (IsDoorA) { if (GM.L2_DoorA == true) { Unlocked = true; } }
            else { if (GM.L2_DoorB == true) { Unlocked = true; } }
        }
        else if (Level == 3)
        {
            if (IsDoorA) { if (GM.L3_DoorA == true) { Unlocked = true; } }
            else { if (GM.L3_DoorB == true) { Unlocked = true; } }
        }
    }

    IEnumerator Open()
    {
        if (Level == 1)
        {
            if (IsDoorA) { GM.L1_DoorA = true; }
            else { GM.L1_DoorB = true; }
        }
        else if (Level == 2)
        {
            if (IsDoorA) { GM.L2_DoorA = true; }
            else { GM.L2_DoorB = true; }
        }
        else if (Level == 3)
        {
            if (IsDoorA) { GM.L3_DoorA = true; }
            else { GM.L3_DoorB = true; }
        }
        GetComponent<Animator>().SetBool("Open", true);
        yield return new WaitForSeconds(0.5f);
        Door.GetComponent<Animator>().SetBool("Open", true);
    }
}
