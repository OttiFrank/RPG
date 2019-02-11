using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] GameObject player = null;
        [SerializeField] float sensitivity = 4.0f;
        [SerializeField] bool lockCursor = false;
        [Header("Cameras")]
        [SerializeField] Camera aimCamera = null;
        [SerializeField] Camera mainCamera = null;

        Vector3 offset;
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

        public Camera GetCurrentCamera()
        {
            if (mainCamera.enabled)
                return mainCamera;
            else
                return aimCamera;
        }

    }
}

