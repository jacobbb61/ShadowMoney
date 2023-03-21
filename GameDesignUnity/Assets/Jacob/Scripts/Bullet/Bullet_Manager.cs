using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour
{
    public bool BulletType1;
    public bool BulletType2;
    public bool BulletType3;
    public bool BulletType4;

    public int DamageBuff;
    public int Damage;
    public int TickTime;

    [Header("DO NOT EDIT")]
    public int Type1Damage=5;
    public int Type2Damage=20;
    public int Type3Damage=5;
    public int Type4Damage=40;

    public int Type1Tick = 5;
    public int Type2Tick = 8;
    public int Type3Tick = 5;
    public int Type4Tick = 10;

    private void Awake()
    {
        if (BulletType1) { Damage = Type1Damage; TickTime = Type1Tick; }
        if (BulletType2) { Damage = Type2Damage; TickTime = Type2Tick; }
        if (BulletType3) { Damage = Type3Damage; TickTime = Type3Tick; }
        if (BulletType4) { Damage = Type4Damage; TickTime = Type4Tick; }
    }
}
