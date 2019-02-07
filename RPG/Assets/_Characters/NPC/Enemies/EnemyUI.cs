using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] GameObject enemyCanvasPrefab = null;

        Camera cameraToLookAt;
        // Start is called before the first frame update
        void Start()
        {
            cameraToLookAt = Camera.main;
            Instantiate(enemyCanvasPrefab, transform.position, Quaternion.identity, transform);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}

