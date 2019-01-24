using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class MoveCamera : MonoBehaviour
    {
        FreeLookCam cam;
        // Start is called before the first frame update
        void Start()
        {
            cam = GetComponent<FreeLookCam>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleUserInputs();
        }

        private void HandleUserInputs()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                cam.enabled = true;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                cam.enabled = false;
            }
        }
    }
}

