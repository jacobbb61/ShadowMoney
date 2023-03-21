using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom_Manager : MonoBehaviour
{
    [Header("Combat room stats")]
    public bool InCombat;
    public float TimeBeforeSpawn;
    public int TotalEnemiesToSpawn;

    [Header("Nuts")]
    public GameObject Nuts;
    public int CurrentNuts;
    public int NutsToSpawn;
    public int MaxNuts;
    public int NutsElement;
    
    [Header("Rizzard")]
    public GameObject Rizzard;
    public int CurrentRizzards;
    public int RizzardsToSpawn;
    public int MaxRizzards;
    public int RizzardElement;

    [Header("Footer")]
    public GameObject Footer;
    public int CurrentFooters;
    public int FootersToSpawn;
    public int MaxFooters;
    public int FooterElement;

    [Header("Tank")]
    public GameObject Tank;
    public int CurrentTanks;
    public int TanksToSpawn;
    public int MaxTanks;
    public int TankElement;

    [Header("Spawn locations")]
    public GameObject Pos1;
    public GameObject Pos2;
    public GameObject Pos3;
    public GameObject Pos4;

    [Header("Entrance and Exit Doors")]
    public GameObject[] Doors;



    private float SpawnTimer;
    private int SpawnLocationNumber;
    private GameObject SpawnLocation;

    GameManager GM;

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        TotalEnemiesToSpawn = NutsToSpawn + RizzardsToSpawn + FootersToSpawn + TanksToSpawn;
    }


    void FixedUpdate()
    {
        if (InCombat)
        {
            
            foreach (var item in Doors) { item.SetActive(true); }
            
            SpawnTimer += Time.deltaTime;
            if (SpawnTimer >= TimeBeforeSpawn && CurrentNuts < MaxNuts && NutsToSpawn>0) { SpawnEnemy(Nuts, NutsElement); NutsToSpawn--; }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentRizzards < MaxRizzards && RizzardsToSpawn > 0) { SpawnEnemy(Rizzard, RizzardElement); RizzardsToSpawn--; }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentFooters < MaxFooters && FootersToSpawn > 0) { SpawnEnemy(Footer, FooterElement); FootersToSpawn--; }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentTanks < MaxTanks && TanksToSpawn > 0) { SpawnEnemy(Tank, TankElement); TanksToSpawn--; }

            /* if(NutsToSpawn == 0 && RizzardsToSpawn == 0 && FootersToSpawn == 0 && TanksToSpawn == 0)
             {
                 if (CurrentNuts == 0 && CurrentRizzards == 0 && CurrentFooters == 0 && CurrentTanks == 0)
                 {     
                     InCombat = false;
                     GM.OutCombat();
                 }
             }*/
            GetCurrentEnemies();
            if (TotalEnemiesToSpawn == 0 && CurrentNuts == 0 && CurrentRizzards == 0 && CurrentFooters == 0 && CurrentTanks == 0)
            {
                InCombat = false;
                GM.OutCombat();
            }

        }
        else
        {
            foreach (var item in Doors)
            {
                item.SetActive(false);
            }
            
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player")) 
        { 
            InCombat = true; 
            GM.InCombat();
            GetComponent<Collider>().enabled = false;
        }   
       
    } 

    public void GetCurrentEnemies()
    {
        CurrentNuts = GameObject.FindGameObjectsWithTag("Nuts").Length;
        CurrentRizzards = GameObject.FindGameObjectsWithTag("Rizzard").Length;
        CurrentFooters = GameObject.FindGameObjectsWithTag("Footer").Length;
        CurrentTanks = GameObject.FindGameObjectsWithTag("Tank").Length;
    }
    
    public void SpawnEnemy(GameObject Enemy, int Element)
    {
        SpawnTimer = 0;
        SpawnLocationNumber = Random.Range(1, 5);
        if (SpawnLocationNumber == 1) { SpawnLocation = Pos1; }
        else if (SpawnLocationNumber == 2) { SpawnLocation = Pos2; }
        else if (SpawnLocationNumber == 3) { SpawnLocation = Pos3; }
        else if (SpawnLocationNumber == 4) { SpawnLocation = Pos4; }
        GameObject NewEnemy =  Instantiate(Enemy, SpawnLocation.transform.position, SpawnLocation.transform.rotation);
        if (NewEnemy.CompareTag("Nuts"))
        {
            if (Element == 1) { NewEnemy.GetComponent<Nuts_Manager>().Fire = true;  }
            if (Element == 2) { NewEnemy.GetComponent<Nuts_Manager>().Ice = true; }
            if (Element == 3) { NewEnemy.GetComponent<Nuts_Manager>().Void = true; }
            if (Element == 4) { NewEnemy.GetComponent<Nuts_Manager>().Air = true; }
        }
        if (NewEnemy.CompareTag("Rizzard"))
        {
            if (Element == 1) { NewEnemy.GetComponent<Rizzard_Manager>().Fire = true; }
            if (Element == 2) { NewEnemy.GetComponent<Rizzard_Manager>().Ice = true; }
            if (Element == 3) { NewEnemy.GetComponent<Rizzard_Manager>().Void = true; }
            if (Element == 4) { NewEnemy.GetComponent<Rizzard_Manager>().Air = true; }
        }
        if (NewEnemy.CompareTag("Footer"))
        {
         //   if (Element == 1) { NewEnemy.GetComponent<Nuts_Manager>().Fire = true; }
         //   if (Element == 2) { NewEnemy.GetComponent<Nuts_Manager>().Ice = true; }
         //   if (Element == 3) { NewEnemy.GetComponent<Nuts_Manager>().Void = true; }
         //  if (Element == 4) { NewEnemy.GetComponent<Nuts_Manager>().Air = true; }
        }
        if (NewEnemy.CompareTag("Tank"))
        {
         //   if (Element == 1) { NewEnemy.GetComponent<Nuts_Manager>().Fire = true; }
         //   if (Element == 2) { NewEnemy.GetComponent<Nuts_Manager>().Ice = true; }
         //   if (Element == 3) { NewEnemy.GetComponent<Nuts_Manager>().Void = true; }
         //   if (Element == 4) { NewEnemy.GetComponent<Nuts_Manager>().Air = true; }
        }
        TotalEnemiesToSpawn--;
    }
}
