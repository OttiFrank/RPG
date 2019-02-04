using RPG.Characters;
using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.ThirdPerson;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealth = 100f;
        [SerializeField] WeaponConfig weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] float chaseRadius = 12f;
        [SerializeField] float attackRadius = 2f;
        
        WeaponType type;
        Player player = null;
        WeaponConfig playerWeapon = null;
        AICharacterControl aiCharacterControl = null;
        Weapon weapon = null;
        Animator animator = null;
        NavMeshAgent navMesh = null;
        GameObject weaponPrefab = null;
        GameObject dominantHand = null;

        float currentHealth;
        float timeBetweenAttacks;
        float attackTimer;
        float timeBetweenHits;
        float hitTimer;
        bool isAlive = true;
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
            aiCharacterControl = GetComponent<AICharacterControl>();
            SetupWeapon();
            SetMaxHealth();
            FindPlayer();
            FindCurrentEquipedPlayerWeapon();
            SetupAnimatorOverriderController();
            
            PutWeaponInHands();

            attackTimer = 0f;
        }

        private void SetupWeapon()
        {
            weaponPrefab = weaponInUse.GetWeaponModel;
            type = weaponInUse.type;
            weapon = weaponPrefab.GetComponent<Weapon>();
            timeBetweenAttacks = weaponInUse.GetTimeBetweenAttacks;
        }

        private void PutWeaponInHands()
        {
            weaponPrefab = weaponInUse.GetWeaponModel;
            if (weaponInUse != null && weaponPrefab != null)
            {
                dominantHand = RequestDominantHand();
                var weapon = Instantiate(weaponPrefab, dominantHand.transform);
                weapon.transform.localPosition = weaponInUse.weaponGrip.localPosition;
                weapon.transform.localRotation = weaponInUse.weaponGrip.localRotation;

                // TODO: Change to bow only
                if (type == WeaponType.Ranged)
                {
                    GameObject arrowSpawnPoint = weaponInUse.GetProjectilePrefab.transform.Find("ArrowSpawnPoint").gameObject;
                    Instantiate(weaponInUse.GetProjectilePrefab, arrowSpawnPoint.transform.position, Quaternion.identity);
                }

            }
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "Could not find any dominant hands, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Found multiple dominant hands, please remove one");
            return dominantHands[0].gameObject;
        }

        private void SetupAnimatorOverriderController()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["Basic Attack"] = weaponInUse.GetAttackAnimation;
            animatorOverrideController["Idle"] = weaponInUse.GetIdleAnimation;
            animatorOverrideController["Death"] = weaponInUse.GetDeathAnimation;
            animatorOverrideController["Run"] = weaponInUse.GetRunningAnimation;
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
            CheckDistanceToPlayer();
        }

        private void CheckDistanceToPlayer()
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= chaseRadius)
            {
                aiCharacterControl.SetTarget(player.transform);

                if (dist <= attackRadius)
                    AttackPlayer();
            }
            else
                aiCharacterControl.SetTarget(null); 
        }

        private void AttackPlayer()
        {
            if(isAlive)
            {
                if (Time.time - attackTimer > timeBetweenAttacks)
                {
                    animator.SetTrigger("Attack");
                    attackTimer = Time.time;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth = currentHealth - damage;

            if (currentHealth <= 0)
            {
                Die();                
            }
                
        }

        private void Die()
        {
            navMesh.enabled = false;
            animator.enabled = false;
            GameObject socket = transform.Find("UI Socket").gameObject;
            socket.SetActive(false);
            aiCharacterControl.enabled = false;
            isAlive = false;
        }

        private void OnDrawGizmos()
        {
            Color color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Color Attackcolor = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}

