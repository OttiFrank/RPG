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
        AnimationClip attackAnimation; 
        
        float weaponDamage;
        float chargeTime;
        bool isRanged = false;
        bool aiCharacter = false; 
        // Start is called before the first frame update
        void Start()
        {            
            FindPlayer();
            FindPlayerWeapon();
            GetWeaponAnimations();
            if (GetComponentInParent<Player>() == null)
                aiCharacter = true;
            else
                aiCharacter = false;
        }

        private void GetWeaponAnimations()
        {
            attackAnimation = weaponInUse.GetAttackAnimation; 
        }

        

        

        private void FindPlayerWeapon()
        {
            weaponInUse = player.GetPlayerWeapon;
            weaponDamage = weaponInUse.GetWeaponDamage;

            if (weaponInUse.type == WeaponType.Ranged)
                isRanged = true;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && !aiCharacter)
            {
                GameObject enemy = other.gameObject;
                var enemyComponent = enemy.GetComponentInParent<Enemy>();
                enemyComponent.TakeDamage(weaponDamage);
            }
            if(other.gameObject.tag == "Player" && aiCharacter)
            {
                GameObject player = other.gameObject;
                var playerComponent = player.GetComponentInParent<Player>();
                playerComponent.TakeDamage(weaponDamage);
            }
        }
    }
}

