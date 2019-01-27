using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

namespace RPG.Core
{
    public class MoveCamera : MonoBehaviour
    {
        public float turnSpeed = 4.0f;
        public Transform player;
        private Vector3 offset;
        bool isEnabled;
        float baseFOV;
        Transform baseTransform;

        void Start()
        {
            offset = new Vector3(player.position.x, player.position.y + 0f, player.position.z + 0f);
            isEnabled = false;
            baseFOV = Camera.main.fieldOfView;
            baseTransform = gameObject.transform;
        }

        private void LateUpdate()
        {
            HandleUserInput();
        }

        private void HandleUserInput()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isEnabled = true;
            }

            if (Input.GetButtonUp("Fire2"))
            {
                isEnabled = false;
                Camera.main.fieldOfView = baseFOV;
                
            }

            if (isEnabled)
                ZoomIn();
            /*
            if (isEnabled)
                RotateCamera();
                */
        }

        private void ZoomIn()
        {
            Camera.main.fieldOfView = 10;
            Vector3 rotation = transform.eulerAngles;
        }

        private void RotateCamera()
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
            //transform.position = player.position + offset;
            transform.LookAt(player.position);
        }
    }
}

