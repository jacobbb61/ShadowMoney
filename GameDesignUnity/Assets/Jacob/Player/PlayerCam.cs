using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public float XAimAssist;
    public float YAimAssist;
    public float sensXAimAssist = 1;
    public float sensYAimAssist = 1;



    public bool aim;    

    Vector2 movementInput = Vector2.zero;

    public Transform player;

    float xRotation = 0f;


    private void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PCam = this;
    }

    public void Look(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }



    void Update()
    {

            xRotation -= movementInput.y * sensY/sensYAimAssist * Time.deltaTime ;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.transform.Rotate(Vector3.up * movementInput.x * sensX/ sensXAimAssist * Time.deltaTime);




        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, 100)) 
        {
            if (hit.transform.CompareTag("Nuts") || hit.transform.CompareTag("Rizzard") || hit.transform.CompareTag("Footer") || hit.transform.CompareTag("Tank"))
            {
                AimAssistOn();
            }
            else if (hit.transform==null)
            {
                AimAssistOff();
            }
            else { AimAssistOff(); }
            
        }



    }

    public void AimAssistOn()
    {
        sensXAimAssist = XAimAssist;
        sensYAimAssist = YAimAssist;
    }
    public void AimAssistOff()
    {
        sensXAimAssist = 1;
        sensYAimAssist = 1;
    }

    public void UpdateSensX (float playerChangeSensX)
    {
        sensX = playerChangeSensX*20; 
    }

    public void UpdateSensY(float playerChangeSensY)
    {
        sensY = playerChangeSensY * 20; 
    }

}
