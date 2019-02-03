using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float sensitivity = 4.0f; 
    [SerializeField] bool lockCursor = false;
    [Header("Cameras")]
    [SerializeField] Camera aimCamera;
    [SerializeField] Camera mainCamera;

    float minHeight = -0.30f;
    float maxHeight = 0.60f;
    Vector3 offset;
    bool isAiming;
    float baseFOV;
    Transform baseCameraTransform;

    private void Awake()
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        baseFOV = Camera.main.fieldOfView;
    }

    private void Update()
    {
        HandleUserInput();
        if (lockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        //TODO: fix so the character is looking at the direction the player is aiming
        /*
        if(isAiming)
        {
            player.transform.LookAt(aimCamera.transform.forward);
        }
        */
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        RotateCameraAroundPlayer();
        transform.position = player.transform.position + offset;
    }

    private void RotateCameraAroundPlayer()
    {
        float rotateHorizontal = -Input.GetAxis("Mouse X");
        float rotateVertical = -Input.GetAxis("Mouse Y");
        
        transform.RotateAround(player.transform.position, -Vector3.up, rotateHorizontal * sensitivity);
        transform.RotateAround(Vector3.zero, transform.right, rotateVertical * sensitivity);
        
            
    }

    private void HandleUserInput()
    {
        if (Input.GetButtonDown("Fire2"))
            ZoomIn();

        if (Input.GetButtonUp("Fire2"))
            ZoomOut();
    }

    private void ZoomOut()
    {
        isAiming = false;
        Debug.Log("Zoom out");
        aimCamera.enabled = false;
        mainCamera.enabled = true;
    }

    private void ZoomIn()
    {
        isAiming = true;
        Debug.Log("Zoom in");
        mainCamera.enabled = false;
        aimCamera.enabled = true;
        

    }

}
