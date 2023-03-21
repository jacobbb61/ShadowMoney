using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footerMovement : MonoBehaviour
{
    //flyingY
    float groundDist;
    LayerMask groundLM;

    bool isGrounded;
    float yHeight, sinYHeight;

    //rotation
    public Transform player;
    float eulerY;
    float flyTimer;

    //positions
    float dist, xMvt, zMvt;
    float mvtVarX, mvtVarY;

    //teleport locations
    private GameObject telePos1, telePos2, telePos3;
    private int TeleportLocationNumber;
    private GameObject TeleportLocation;

    void Start()
    {
        groundDist = 4f;
        groundLM = LayerMask.GetMask("Ground");
        telePos1 = GameObject.Find("Teleport Location (1)");
        telePos2 = GameObject.Find("Teleport Location (2)");
        telePos3 = GameObject.Find("Teleport Location (3)");
    }

    void Update()
    {
        flyingY();
        movement();
        rotation();
        if (flyTimer >= 0.25f)
        {
            positionUpdates();
            flyTimer = 0;
        }
        flyTimer += Time.deltaTime;
    }

    void rotation()
    {
        transform.LookAt(player);
    }

    void positionUpdates()
    {
        eulerY = transform.localRotation.eulerAngles.y;
        dist = Vector3.Distance(player.position, transform.position);
        xMvt = Mathf.Sin(eulerY);
        zMvt = Mathf.Cos(eulerY);

        if (dist > 42)
        {
            teleport();
        }
    }

    void movement()
    {
        if (dist > 14)
        {
            mvtVarX = 1;
            mvtVarY = 1;
        }
        else if (dist <= 14 && dist > 10)
        {
            mvtVarX = (dist - 10) / 2;
            mvtVarY = (dist - 10) / 2;
        }

        this.transform.position = this.transform.position + new Vector3(xMvt * mvtVarX, 0f, zMvt * mvtVarY) * Time.deltaTime;
    }

    void flyingY()
    {
        sinYHeight = Mathf.Sin(Time.time * 8) * 2;

        isGrounded = Physics.CheckSphere(transform.position, groundDist, groundLM);

        if (isGrounded == true)
        {
            if (sinYHeight > 0)
            {
                yHeight = sinYHeight;
            }
            else
            {
                yHeight -= 0.01f;
            }
        }
        else
        {
            yHeight -= 0.01f;
        }

        this.transform.position = this.transform.position + new Vector3(0f, yHeight, 0f) * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        teleport();
    }

    void teleport()
    {
        TeleportLocationNumber = Random.Range(1, 4);

        if (TeleportLocationNumber == 1)
        {
            TeleportLocation = telePos1;
        }
        else if (TeleportLocationNumber == 2)
        {
            TeleportLocation = telePos2;
        }
        else if (TeleportLocationNumber == 3)
        {
            TeleportLocation = telePos3;
        }

        this.transform.position = TeleportLocation.transform.position;
    }
}
