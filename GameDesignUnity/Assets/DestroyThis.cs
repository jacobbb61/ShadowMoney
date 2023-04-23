using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour
{
 
    public float WhenToDestory;
    void Start()
    {
       Destroy(gameObject, WhenToDestory); 
    }

}
