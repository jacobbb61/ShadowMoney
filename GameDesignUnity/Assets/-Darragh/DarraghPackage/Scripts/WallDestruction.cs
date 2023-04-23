using System;
using System.Collections;
using UnityEngine;
namespace UnityEngine.AI

{
    public class WallDestruction : MonoBehaviour
    {
        public Effects_Manager EM;

        //public bool IsBurning, IsFrozen;
        public int Health;
        public int MaxHealth;

        private float _BurningTime;
        private float TickTime = 1;
        public float TimeToStopBurning;
        public MeshRenderer wall;

        public Material Wood;
        public Material BurntWood;
        public GameObject FireParticles;

        public GameObject[] Navmeshes;
        public BoxCollider NavCol1;
        public BoxCollider NavCol2;


        public BoxCollider NavMesH;

        public bool IsSheild;

        private void Start()
        {
            Navmeshes = GameObject.FindGameObjectsWithTag("NavMesh");

            Health = MaxHealth;
            if (!IsSheild) { GetComponent<Renderer>().material = Wood; }
            EM = GetComponent<Effects_Manager>();
        }

        private void Update()
        {
            if (EM.IsBurning) { WallBurnt(); }
            if (EM.IsFrozen) { Extinguished(); }
            if (Health <= 0 && Health<1000)
            {
                StartCoroutine(Death());
            }
        }

        public IEnumerator Death()
        {
            Health = 1000;
            if (!IsSheild)
            {
                NavCol1.enabled = false;
                NavCol2.enabled = false;
                wall.enabled = false;
            }
            else
            {
                NavCol1.enabled = false;
            }

            yield return new WaitForSeconds(.1f);

            if (!IsSheild)
            {
                GetComponentInParent<NavMeshSurface>().BuildNavMesh();
            }

            Destroy(gameObject);
        }

        public void Extinguished()
        {
            FireParticles.SetActive(false);
            EM.IsBurning = false;
            EM.IsFrozen = false;
        }
        public void WallBurnt()
        {
            if (!IsSheild)
            {
                GetComponent<Renderer>().material = BurntWood;
            }
            FireParticles.SetActive(true);
            _BurningTime += Time.deltaTime;
            if (_BurningTime >= TickTime) { TickTime++; Health--; }
            if (_BurningTime >= TimeToStopBurning)
            {
                FireParticles.SetActive(false);
                _BurningTime = 0;
                TickTime = 1;
                EM.IsBurning = false;
            }

        }
        public void EnemyBurned(GameObject toBurn)
        {
            if (toBurn.TryGetComponent(out PlayerManager player))
            {
                if (!player.EM.IsBurning)
                {
                    player.EM.IsBurning = true;
                }
            }
            else if (toBurn.TryGetComponent(out Nuts_Manager nuts))
            {
                if (!nuts.EM.IsBurning)
                {
                    nuts.EM.IsBurning = true;
                }
            }
            /*else if(toBurn.TryGetComponent(out Footer_Manager footer)){
                if(!footer.EM.IsBurning){
                    footer.EM.IsBurning = true;
                }
            }*/
            else if (toBurn.TryGetComponent(out Rizzard_Manager rizzard))
            {
                if (!rizzard.EM.IsBurning)
                {
                    rizzard.EM.IsBurning = true;
                }
            }
            else if (toBurn.TryGetComponent(out Tank_Manager tank))
            {
                if (!tank.EM.IsBurning)
                {
                    tank.EM.IsBurning = true;
                }
            }
            else if (toBurn.TryGetComponent(out Effects_Manager E))
            {
                if (!E.IsBurning)
                {
                    E.IsBurning = true;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (EM.IsBurning && other.TryGetComponent(out Effects_Manager OEM))

            {
                Debug.Log("Enemy touch");
                EnemyBurned(other.gameObject);
            }

            if (other.CompareTag("PlayerBullet"))
            {
                Effects_Manager BEM;
                BEM = other.GetComponent<Effects_Manager>();
                if (BEM.FireEffect) { EM.IsBurning = true; }
                if (BEM.IceEffect) { EM.IsFrozen = true; }
                if (BEM.VoidEffect) { }
                if (BEM.AirEffect) { }
                Destroy(other);
            }
            else { return; }



        }


    }
}