using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;

    public CharacterController controller;

    public float rotationSpeed;

    private Vector2 rotation;

    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() 
    {
        Movement();
        Rotation();
    }

    void Movement() 
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical"); 

        Vector3 movement = new Vector3(moveX, -1, moveZ);

        movement = transform.TransformDirection(movement);

        if (moveX == 0 && moveZ == 0) movement = Vector3.zero;

        controller.Move(movement * movementSpeed * Time.deltaTime);
    }

    void Rotation()
    {
        rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
        rotation.y += Input.GetAxis("Mouse Y") * rotationSpeed;

        rotation.y = Mathf.Clamp(rotation.y, -10, 10);

        transform.localRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
    }
}
