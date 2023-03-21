using System;
using System.Collections;
using UnityEngine;

public class PuddleInteraction : MonoBehaviour
{

    public Effects_Manager EM;
    public float puddlefreezeTime;
    public GameObject IceParticles;
    private void Start()
    {
        EM = GetComponent<Effects_Manager>();
    }
    private void Update()
    {
        if(EM.IsFrozen){StartCoroutine(PuddleFrozen());}
        if(EM.IsBurning){PuddleBurnt();}
    }
    IEnumerator PuddleFrozen()
    {
       EM.IsFrozen = true;
        IceParticles.SetActive(true);
        //Apply FrozenVFXEffect or change sprite to frozen water
        yield return new WaitForSeconds(puddlefreezeTime);
        //Remove FrozenVFXEffect or change sprite back to water
        IceParticles.SetActive(false);
        EM.IsFrozen = false;
    }

    public void EnemyFrozen(GameObject toFreeze)
    {
        if (toFreeze.TryGetComponent(out PlayerManager player))
        {
            if (!player.EM.IsFrozen)
            {
                player.EM.IsFrozen = true;
            }
        }
        else if (toFreeze.TryGetComponent(out Nuts_Manager nuts))
        {
            if (!nuts.EM.IsFrozen)
            {
                nuts.EM.IsFrozen = true;
            }
        }
        /*else if(toFreeze.TryGetComponent(out Footer_Manager footer)){
            if(!footer.EM.IsFrozen){
                footer.EM.IsFrozen = true;
            }
        }*/
        else if (toFreeze.TryGetComponent(out Rizzard_Manager rizzard))
        {
            if (!rizzard.EM.IsFrozen)
            {
                rizzard.EM.IsFrozen = true;
            }
        }
        else if (toFreeze.TryGetComponent(out Effects_Manager E))
        {
            if (!E.IsFrozen)
            {
                E.IsFrozen = true;
            }
        }
    }


    public void PuddleBurnt()
    {
        if (EM.IsBurning)
        {
            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if (EM.IsFrozen && other.TryGetComponent(out Effects_Manager OEM))
        {
            EnemyFrozen(other.gameObject);
        }

        if (other.CompareTag("PlayerBullet"))
        {
            Effects_Manager BEM;
            BEM = other.GetComponent<Effects_Manager>();
            if (BEM.FireEffect) {EM.IsBurning = true;}
            if (BEM.IceEffect) {EM.IsFrozen = true;}
            if (BEM.VoidEffect) { }
            if (BEM.AirEffect) { }
        }else { return; }
    }
}
