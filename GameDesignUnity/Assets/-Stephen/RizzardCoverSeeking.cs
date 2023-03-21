using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RizzardCoverSeeking : MonoBehaviour
{

    Rizzard_Manager RM;

    public GameObject[] boxesAvailable, boxesInUse;
    GameObject closestBox, nextClosestBox, prevClosestBox;
    Transform player;

    float distanceToBox, distanceBetweenBoxes;

    NavMeshAgent navAgent;
    public Vector3 destination;
    Vector3 worldDeltaPosition;
    Vector2 groundDeltaPosition;

    int nextIndex;

    void Start()
    {
        boxesAvailable = GameObject.FindGameObjectsWithTag("BoxesAvailable");
        
        RM = GetComponent<Rizzard_Manager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        destination = NextPatrolPoint(destination);
        navAgent = RM.Agent;
      //  navAgent.updatePosition = false;
    }
    /*
    void Update()
    {
        boxesAvailable = GameObject.FindGameObjectsWithTag("BoxesAvailable");

        if (boxesAvailable.Length >= 2)
        {
            boxSelector();
        }

        navigation();
    }
    */
    public void boxSelector()
    {

        float distanceToClosestBox = 1000;
        float distanceToNextClosestBox = 1000;

        boxesInUse = new GameObject[] { };

        //CURRENT CLOSEST BOX
        for (int i = 0; i < boxesAvailable.Length; i++)
        {
            distanceToBox = Vector3.Distance(this.transform.position, boxesAvailable[i].transform.position);

            if (distanceToBox < distanceToClosestBox)
            {
                closestBox = boxesAvailable[i];
                distanceToClosestBox = distanceToBox;
            }
        }
        //CURRENT CLOSEST BOX

        //CURRENT SECOND CLOSEST BOX
       // if (prevClosestBox == closestBox)
       // {
            for (int i = 0; i < boxesAvailable.Length; i++)
            {
                distanceBetweenBoxes = Vector3.Distance(closestBox.transform.position, boxesAvailable[i].transform.position);

                if (distanceBetweenBoxes < distanceToNextClosestBox && closestBox != boxesAvailable[i]) // equation here
                {
                    nextClosestBox = boxesAvailable[i];
                    distanceToNextClosestBox = distanceBetweenBoxes;
                }
            }
       // }
        //CURRENT SECOND CLOSEST BOX

        //BOXES IN USE
        if (prevClosestBox != closestBox)
        {
            print("update");
            boxesInUse = new GameObject[] { closestBox, nextClosestBox };
            
            destination = NextPatrolPoint(destination);
        }
        //BOXES IN USE

        boxesInUse = new GameObject[] { closestBox, nextClosestBox };
        prevClosestBox = closestBox;
    }

    public void navigation()
    {
        navAgent.SetDestination(destination);
     //   worldDeltaPosition = navAgent.nextPosition - transform.position;
     //   groundDeltaPosition.x = Vector3.Dot(transform.right, worldDeltaPosition);
     //   groundDeltaPosition.y = Vector3.Dot(transform.forward, worldDeltaPosition);

      //  transform.position = navAgent.nextPosition;

        if (Vector3.Distance(transform.position, destination) < 3)
        {         
            destination = NextPatrolPoint(destination);
        }
        if (Vector3.Distance(transform.position, destination) < 2)
        {
            print("shooting");
        }
        if (boxesAvailable.Length < 2)
        {
            destination = RM.Player.transform.position;
        }
        else
        {
            boxSelector();
        }
       
        }

    public Vector3 NextPatrolPoint(Vector3 currentPosition)
    {
        if (boxesAvailable.Length >= 2)
        {
            print("2 or more");
            if (currentPosition != Vector3.zero && Vector3.Distance(transform.position,destination)<3)
            {

                    for (int i = 0; i < boxesInUse.Length; i++)
                    {

                    if (currentPosition == boxesInUse[i].transform.position)
                        {
                            nextIndex = (i + 1) % boxesInUse.Length;

                        }
                    }
                
            }
            else
            {
                nextIndex = 0;
            }

            return boxesInUse[nextIndex].transform.position;
        }
        else
        {
            print("1 or less");
            return player.transform.position;
        }
    }
}
