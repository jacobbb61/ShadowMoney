using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects_Manager : MonoBehaviour
{
    public bool FireEffect;
    public bool IceEffect;
    public bool VoidEffect;
    public bool AirEffect;

    public bool IsBurning;
    public bool IsFrozen;

 
    public void None()
    {
     FireEffect = false;
     IceEffect = false;
     VoidEffect = false;
     AirEffect = false;
     IsBurning = false;
     IsFrozen = false;
}
}
