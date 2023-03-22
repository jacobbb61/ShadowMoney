using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("Controller")]
    public float sensX;
    public float sensY;



    public bool aim;    

    Vector2 movementInput = Vector2.zero;

    public Transform player;

    float xRotation = 0f;

    public bool IsCointroller;

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

            xRotation -= movementInput.y * sensY * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.transform.Rotate(Vector3.up * movementInput.x * sensX * Time.deltaTime);
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
