using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUnlock : MonoBehaviour
{
    UI_Manager UM;
    GameManager GM;
    PlayerCombat PC;

    [Header("New Unlock")]
    public bool UnlockedSuperPunch;
    public bool UnlockedFire;
    public bool UnlockedIce;
    public bool UnlockedAir;
    public bool UnlockedVoid;



    private void Start()
    {
        UM = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Manager>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (UnlockedSuperPunch) { UM.UnlockedUIPopUpText.text = "Super Punch Unlocked"; GM.UnlockedSuperPunch = true; }
        else if (UnlockedFire) { UM.UnlockedUIPopUpText.text = "Fire Element Unlocked"; GM.UnlockedFire = true; }
        else if (UnlockedIce) { UM.UnlockedUIPopUpText.text = "Ice Element Unlocked"; GM.UnlockedIce = true; }
        else if (UnlockedAir) { UM.UnlockedUIPopUpText.text = "Air Element Unlocked"; GM.UnlockedAir = true; }
        else if (UnlockedVoid) { UM.UnlockedUIPopUpText.text = "Void Element Unlocked"; GM.UnlockedVoid = true; }

        UM.UnlockedUIPopUp.GetComponent<Animator>().SetTrigger("Active");
        UM.UpdateUnlocked(); 
        PC.UpdateUnlocked();
        StartCoroutine(Delay());
        Destroy(gameObject);
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        UM.UnlockedUIPopUp.GetComponent<Animator>().ResetTrigger("Active");
    }

}
