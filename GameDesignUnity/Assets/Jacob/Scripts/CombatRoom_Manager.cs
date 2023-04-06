using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom_Manager : MonoBehaviour
{
    [Header("Combat room stats")]
    public bool InCombat;
    public float TimeBeforeSpawn;
    public int TotalEnemiesToSpawn;
    public GameObject EnemySpawnAnim;
    public Collider EnemySpawnBounds;

    [Header("Nuts")]
    public GameObject Nuts;
    public bool IsNutsBase;
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

    [Header("Entrance and Exit Doors")]
    public GameObject[] Doors;



    private float SpawnTimer;

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
         
            foreach (var item in Doors) { item.GetComponent<Animator>().SetBool("Closed", true); }
            
            SpawnTimer += Time.deltaTime;


            if (SpawnTimer >= TimeBeforeSpawn && CurrentNuts < MaxNuts && NutsToSpawn>0) { AttemptSpawn(Nuts, NutsElement, NutsToSpawn); }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentRizzards < MaxRizzards && RizzardsToSpawn > 0) { AttemptSpawn(Rizzard, RizzardElement, RizzardsToSpawn);  }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentFooters < MaxFooters && FootersToSpawn > 0) { AttemptSpawn(Footer, FooterElement, FootersToSpawn);  }
            if (SpawnTimer >= TimeBeforeSpawn && CurrentTanks < MaxTanks && TanksToSpawn > 0) { AttemptSpawn(Tank, TankElement, TanksToSpawn); }

           
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
                item.GetComponent<Animator>().SetBool("Closed", false);
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
    public void AttemptSpawn(GameObject Enemy, int Element, int EnemyToSpawn)
    {
        Vector3 Location;
        Location =  RandomPointInBounds(EnemySpawnBounds.bounds);
        RaycastHit hit;
        if (Physics.Raycast(Location, -Vector3.up, out hit))
        {
            if (hit.transform.CompareTag("Ground")) 
            { 
                Debug.Log("Spawn");
                StartCoroutine(SpawnEnemy(Enemy, Element,hit.transform));;
            }
            Debug.DrawLine(Location, hit.point, Color.cyan);
        }
        else
        {
            AttemptSpawn(Enemy, Element, EnemyToSpawn);
            Debug.Log("try again");
        }
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    IEnumerator SpawnEnemy(GameObject Enemy, int Element, Transform Location)
    {
        SpawnTimer = 0;
        GameObject NewSpawnAnim = Instantiate(EnemySpawnAnim, Location.position, Enemy.transform.rotation);
        Destroy(NewSpawnAnim, 1f);
        yield return new WaitForSeconds(0.75f);
        GameObject NewEnemy =  Instantiate(Enemy, Location.position, Enemy.transform.rotation);
        if (NewEnemy.CompareTag("Nuts"))
        {
            IsNutsBase = !IsNutsBase;
            if (Element == 1) { NewEnemy.GetComponent<Nuts_Manager>().Fire = true;  }
            if (Element == 2) { NewEnemy.GetComponent<Nuts_Manager>().Ice = true; }
            if (Element == 3) { NewEnemy.GetComponent<Nuts_Manager>().Void = true; }
            if (Element == 4) { NewEnemy.GetComponent<Nuts_Manager>().Air = true; }
            NewEnemy.GetComponent<Nuts_Manager>().IsProjectileNuts = IsNutsBase;
            NutsToSpawn--;
        }
        if (NewEnemy.CompareTag("Rizzard"))
        {
            if (Element == 1) { NewEnemy.GetComponent<Rizzard_Manager>().Fire = true; }
            if (Element == 2) { NewEnemy.GetComponent<Rizzard_Manager>().Ice = true; }
            if (Element == 3) { NewEnemy.GetComponent<Rizzard_Manager>().Void = true; }
            if (Element == 4) { NewEnemy.GetComponent<Rizzard_Manager>().Air = true; }
            RizzardsToSpawn--;
        }
        if (NewEnemy.CompareTag("Footer"))
        {
            //   if (Element == 1) { NewEnemy.GetComponent<Nuts_Manager>().Fire = true; }
            //   if (Element == 2) { NewEnemy.GetComponent<Nuts_Manager>().Ice = true; }
            //   if (Element == 3) { NewEnemy.GetComponent<Nuts_Manager>().Void = true; }
            //  if (Element == 4) { NewEnemy.GetComponent<Nuts_Manager>().Air = true; }
            // TanksToSpawn--;
        }
        if (NewEnemy.CompareTag("Tank"))
        {
            if (Element == 1) { NewEnemy.GetComponent<Tank_Manager>().Fire = true; }
            if (Element == 2) { NewEnemy.GetComponent<Tank_Manager>().Ice = true; }
            if (Element == 3) { NewEnemy.GetComponent<Tank_Manager>().Void = true; }
            if (Element == 4) { NewEnemy.GetComponent<Tank_Manager>().Air = true; }
            TanksToSpawn--;
        }
        TotalEnemiesToSpawn--;
    }
}
