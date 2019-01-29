using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    public class Weapon : MonoBehaviour
    {
        Player player;
        WeaponConfig weaponInUse;
        WeaponType type;
        Projectile projectile;
        // Start is called before the first frame update
        void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<Player>();
            weaponInUse = player.GetPlayerWeapon;

        }

        // Update is called once per frame
        void Update()
        {
            
            
        }

        public void Shoot()
        {
                      
        }
        

        
    }
}

