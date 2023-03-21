using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxesDistance : MonoBehaviour
{
    public float BPD; //boxPlayerDistance
    public float b1PD; //box1PlayerDistance
    public float b2PD; //box2PlayerDistance

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        BPD = Vector3.Distance(player.position, transform.position);
    }

    void Update()
    {
        BPD = Vector3.Distance(player.position, transform.position);

        if (BPD <= 50 && BPD >= 5)
        {
            if (CompareTag("Boxes"))
            {
                transform.gameObject.tag = "BoxesAvailable";
            }
        }
        else
        {
            transform.gameObject.tag = "Boxes";
        }
    }
}
