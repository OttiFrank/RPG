using RPG.Characters;
using System;
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
        float weaponDamage;
        float chargeTime; 
        // Start is called before the first frame update
        void Start()
        {
            FindPlayer();
            FindPlayerWeapon();           

        }

        private void FindPlayerWeapon()
        {
            weaponInUse = player.GetPlayerWeapon;
        }

        private void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<Player>();
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

