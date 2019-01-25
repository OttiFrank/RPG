using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class MoveCamera : MonoBehaviour
    {
        public float turnSpeed = 4.0f;
        public Transform player;

        private Vector3 offset;
        bool isEnabled;

        void Start()
        {
            offset = new Vector3(player.position.x, player.position.y + 0f, player.position.z + 0f);
            isEnabled = false;
        }
        private void LateUpdate()
        {
            HandleUserInput();
            //RotateCamera();
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
            }

            if (isEnabled)
                RotateCamera();
        }

        private void RotateCamera()
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
            transform.position = player.position + offset;
            transform.LookAt(player.position);
        }
    }
}

