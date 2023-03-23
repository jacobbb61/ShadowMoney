using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Manager : MonoBehaviour
{
    Bullet_Manager BM;
    Effects_Manager EM;

    [Header("Projectile Effect")]
    public GameObject AllPart;
    public GameObject FirePart;
    public GameObject IcePart;
    public GameObject VoidPart;
    public GameObject AirPart;

    [Header("Projectile Explosions")]
    public GameObject AirPushExplosion;
    public GameObject VoidPullExplosion;


    // [Header("Projectile Type One")]
    private float Speed_T1=25f;
    private float Range_T1=2;

    // [Header("Projectile Type Two")]
    private float Speed_T2=40;
    private float Range_T2=8;

    // [Header("Projectile Type Three")]
    private float Speed_T3=18;
    private float Range_T3=0.5f;

    // [Header("Projectile Type Four")]
    private float Speed_T4=5;
    private float Range_T4=15;

    void Start()
    {
        BM = GetComponent<Bullet_Manager>();
        EM = GetComponent<Effects_Manager>();
    }

    void Update()
    {
        ApplyElement();

            if (BM.BulletType1)
            {
                Destroy(this.gameObject, Range_T1);
                transform.position += Speed_T1 * Time.deltaTime * transform.TransformDirection(Vector3.forward);
            }
            else if (BM.BulletType2)
            {
                Destroy(this.gameObject, Range_T2);
                transform.position += Speed_T2 * Time.deltaTime * transform.TransformDirection(Vector3.forward);
            }
            else if (BM.BulletType3)
            {
                Destroy(this.gameObject, Range_T3);
                transform.position += Speed_T3 * Time.deltaTime * transform.TransformDirection(Vector3.forward);
             //   transform.localScale = new Vector3(transform.localScale.x + (Time.deltaTime * 4), transform.localScale.y, transform.localScale.z);
            }
            else if (BM.BulletType4)
            {
                Destroy(this.gameObject, Range_T4);
                transform.position += Speed_T4 * Time.deltaTime * transform.TransformDirection(Vector3.forward); 
                transform.localScale += new Vector3(0.1f * Time.deltaTime * 10, 0.1f * Time.deltaTime * 10, 0.1f * Time.deltaTime * 10);
            }
            else { return; }
        
    }

    public void ApplyElement()
    {
        if (EM.FireEffect) { FirePart.SetActive(true); } else { FirePart.SetActive(false); }
        if (EM.IceEffect) { IcePart.SetActive(true); } else { IcePart.SetActive(false); }
        if (EM.VoidEffect) { VoidPart.SetActive(true); } else { VoidPart.SetActive(false); }
        if (EM.AirEffect) { AirPart.SetActive(true); } else { AirPart.SetActive(false); }
    }

    public void CreateAirPush()
    {
      GameObject Explosion = Instantiate(AirPushExplosion);
                Explosion.transform.position = transform.position;
                Explosion.GetComponent<AirPushExplosion>().Active();
    }
    public void CreateVoidPush()
    {
        GameObject Explosion = Instantiate(VoidPullExplosion);
        Explosion.transform.position = transform.position;
        Explosion.GetComponent<VoidPullExplosion>().Active();
    }
    private void OnTriggerEnter(Collider other)
    {
        

        if (transform.CompareTag("EnemyBullet"))
        {
            
            if (other.CompareTag("Ground") || other.CompareTag("Wall") || other.CompareTag("Player"))
            {
                if (BM.BulletType1) { Destroy(this.gameObject); }
                if (BM.BulletType2) { Destroy(this.gameObject); }
                if (BM.BulletType3) { Destroy(this.gameObject); }
                if (BM.BulletType4) { }
            
            } 
        }
        if (transform.CompareTag("PlayerBullet"))
        {
            
            if (other.CompareTag("Ground") || other.CompareTag("Wall") || other.CompareTag("Nuts") || other.CompareTag("Rizzard") || other.CompareTag("Footer") || other.CompareTag("Tank"))
            {
                AllPart.transform.parent = null;
                Destroy(AllPart,0.75f);
                if (EM.AirEffect) { CreateAirPush(); }
                if (EM.VoidEffect) { CreateVoidPush(); }
                if (BM.BulletType1) { Destroy(this.gameObject); }
                if (BM.BulletType2) { Destroy(this.gameObject); }
                if (BM.BulletType3) { }
                if (BM.BulletType4) { Destroy(this.gameObject,1); }
            }
        }
    }
}
