using RPG.Characters;
using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealth = 100f;
        bool isAlive;
        float currentHealth;
        Player player;

        WeaponType type;
        WeaponConfig playerWeapon;
        Animator animator;
        NavMeshAgent navMesh;

        float timeBetweenHits;
        float hitTimer;
        public float healthAsPercentage
        {
            get
            {
                float healthAsPercentage = currentHealth / maxHealth;
                return healthAsPercentage;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            navMesh = GetComponent<NavMeshAgent>();
            SetMaxHealth();
            FindPlayer();
            FindCurrentEquipedPlayerWeapon();
        }

    

        private void FindCurrentEquipedPlayerWeapon()
        {
            playerWeapon = player.GetPlayerWeapon;
            timeBetweenHits = playerWeapon.GetTimeBetweenAttacks;
        }

        private void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<Player>();
        }

        private void SetMaxHealth()
        {
            currentHealth = maxHealth;
        }

        // Update is called once per frame
        void Update()
        {

        }



        public void TakeDamage(float damage)
        {
            currentHealth = currentHealth - damage;

            if (currentHealth <= 0)
            {
                navMesh.enabled = false;
                animator.enabled = false;
                GameObject socket = transform.Find("UI Socket").gameObject;
                socket.SetActive(false);
            }
                
        }
    }
}

