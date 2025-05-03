using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject observeObject;
    [SerializeField]
    private Vector3 cameraDisplacement = new Vector3(-5, 5, -10);

    [SerializeField]
    private float movementSpeed, mouseSensitivity;
    //private Rigidbody cameraBody;
    private Camera cam;
    private Vector3 movement, movex, movez, newPosition, reset = Vector3.zero;
    private float mouseX, mouseY, rotateX, rotateY, rotateZ;
    private void MoveInPersonalSpace()
    {
        //controls movement based on facing of camera
        movement = reset;
        movex = reset;
        movez = reset;

        cam.transform.rotation = Quaternion.Euler(rotateX, rotateY, 0f);

        if (Input.GetKey(KeyCode.W)) movex = cam.transform.forward;
        else if (Input.GetKey(KeyCode.S)) movex = cam.transform.forward * -1f;

        if (Input.GetKey(KeyCode.A)) movez = cam.transform.right * -1f;
        else if (Input.GetKey(KeyCode.D)) movez = cam.transform.right;

        movement = movex + movez;
    }
    private void MoveInGlobalSpace()
    {
        //Controls movement in world coordinates
        if (Input.GetKey(KeyCode.W)) movement.z = 1;
        else if (Input.GetKey(KeyCode.S)) movement.z = -1;
        else movement.z = 0;

        if (Input.GetKey(KeyCode.A)) movement.x = -1;
        else if (Input.GetKey(KeyCode.D)) movement.x = 1;
        else movement.x = 0;
    }
    private void RotateCamera()
    {
        //makes camera follow the mouse
        rotateX += mouseY * -1f;
        //rotateX = Mathf.Clamp(rotateX, 20f, 90f);
        rotateY += mouseX;

        cam.transform.rotation = Quaternion.Euler(rotateX, rotateY, rotateZ);
    }
    private void GetMouse()
    {
        //Gets mouse inputs
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
    }
    private void MouseControl()
    {
        //Sorts mouse inputs; mouse movement, left mousebutton, right mousebutton

        GetMouse();

        if (Input.GetMouseButton(1)) RotateCamera(); // rightclick
    }
    private void Mover()
    {
        //cameraBody.velocity = reset; //Prevents Bounce

        movement = movement.normalized * movementSpeed * Time.deltaTime;
        Vector3 pos = transform.position;
        newPosition.x = movement.x + pos.x;
        newPosition.z = movement.z + pos.z;
        newPosition.y = pos.y;
        transform.position = newPosition;
        //gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
    }
    private void Awake()
    {
        //cameraBody = GetComponent<Rigidbody>();
        cam = GetComponent<Camera>();
        Vector3 camRot = cam.transform.rotation.eulerAngles;
        rotateX = camRot.x;
        rotateY = camRot.y;
    }

    public void SetRotation(Vector3 rotation)
    {
        rotateX = rotation.x;
        rotateY = rotation.y;
    }

    private void KeyDownCheck()
    {
        if (!Input.anyKey) return;

        if (Input.GetKeyDown(KeyCode.R))
            ResetCameraPosition();
    }

    private void ResetCameraPosition()
    {
        if (observeObject == null)
        {
            Debug.Log($"Missing observeObject");
            return;
        }

        transform.position = observeObject.transform.position + cameraDisplacement;
        transform.LookAt(observeObject.transform);

        Quaternion rot = transform.rotation;

        rotateX = rot.eulerAngles.x;
        rotateY = rot.eulerAngles.y;
        rotateZ = rot.eulerAngles.z;
    }

    private void Update()
    {
        MouseControl();
        KeyDownCheck();
    }
    private void FixedUpdate()
    {
        // if (Input.GetKey(KeyCode.LeftShift)) MoveInGlobalSpace(); //World position or object position
        // else MoveInPersonalSpace();
        MoveInPersonalSpace();
        Mover();
    }
}

/*
 * * * Invisible Cursor
 * Cursor rotates view direction
 * w - forwards
 * s - backwards
 * a - left
 * d - right
 * * * holding left mousebutton - zoom in
 * holding left shift - move in world coordinates
 * speed of movement should be constant regardless of direction
 * direction of observation should not affect speed of movement
 */