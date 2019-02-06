using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Environment
{
    public class HealingPack : MonoBehaviour
    {
        [SerializeField] float healingValue = 50f; 
        // Start is called before the first frame update
        void Start()
        {
            healingValue = -healingValue; 
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            GameObject target = other.gameObject;
            if (target.tag == "Player")
            {
                var player = target.GetComponentInParent<Player>();
                player.TakeDamage(healingValue);
                Destroy(gameObject);
            }
        }
    }
}

