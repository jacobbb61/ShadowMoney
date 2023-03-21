using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GameMemory : ScriptableObject
{
    [Header("Per Level")]
    public Transform LastCheckpoint;
}
