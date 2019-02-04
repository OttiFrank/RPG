using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.ThirdPerson;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] bool godMode = false;
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] WeaponConfig weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] float maxStamina = 100.0f;
        [SerializeField] float staminaRecoveryRate = 10f;
        [SerializeField] float staminaCooldown = 5f;

        WeaponType type;
        ThirdPersonUserControl thirdPersonUserControl;
        GameObject weaponPrefab;
        GameObject projectileModel;
        GameObject dominantHand;
        GameObject arrowHand;
        float currentHealth;
        float currentStamina;
        float staminaStart = 0; 
        float timeBetweenAttacks;
        float attackTimer;
        float staminaDrain;
        bool isAlive;
        bool staminaCD = false; 
        Animator animator;
        Weapon weapon;

        // Start is called before the first frame update
        void Start()
        {
            thirdPersonUserControl = GetComponent<ThirdPersonUserControl>();
            SetupWeapon();
            SetMaxCharacterResources();
            PutWeaponInHands();
            SetupRuntimeAnimator();

            attackTimer = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (isAlive)
            {
                HandleUserInput();
                //TODO: Fix stamina recovery
                
            }
        }
       
        private void SetupWeapon()
        {
            weaponPrefab = weaponInUse.GetWeaponModel;
            type = weaponInUse.type;
            weapon = weaponPrefab.GetComponent<Weapon>();
            timeBetweenAttacks = weaponInUse.GetTimeBetweenAttacks;
            staminaDrain = weaponInUse.GetStaminaDrain; 
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
                if(type == WeaponType.Ranged)
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

        private void SetMaxCharacterResources()
        {
            isAlive = true;
            currentHealth = maxHealth;
            currentStamina = maxStamina; 
        }

        public float healthAsPercentage
        {
            get
            {
                float healthAsPercentage = currentHealth / maxHealth;
                return healthAsPercentage;
            }
        }
        public float staminaAsPercentage
        {
            get
            {
                float staminaAsPercentage = currentStamina / maxStamina;
                return staminaAsPercentage;
            }
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["Basic Attack"] = weaponInUse.GetAttackAnimation;
            animatorOverrideController["Idle"] = weaponInUse.GetIdleAnimation;
            animatorOverrideController["Death"] = weaponInUse.GetDeathAnimation;
            animatorOverrideController["Run"] = weaponInUse.GetRunningAnimation;
        }

        private void HandleUserInput()
        {            
            if (Input.GetButtonDown("Fire1"))
            {
                AttackTarget();                
            }
        }

        private void AttackTarget()
        {
            if(isAlive)
            {
                if(currentStamina >= staminaDrain)
                {
                    if (Time.time - attackTimer > timeBetweenAttacks)
                    {
                        currentStamina = currentStamina - staminaDrain;
                        animator.SetTrigger("Attack");
                        attackTimer = Time.time;
                    }
                }
                
            }            
        }

        public WeaponConfig GetPlayerWeapon
        {
            get
            {
                return weaponInUse;
            }
        }

        public void TakeDamage(float damage)
        {
            if(isAlive && !godMode)
            {
                if(currentHealth > 0)
                    currentHealth = currentHealth - damage;

                Debug.Log("Player current health:" + currentHealth); 
                if (currentHealth <= 0)
                {
                    Die();
                    currentHealth = 0; 
                }
            }
        }

        private void Die()
        {
            Debug.Log("Player died"); 
            animator.enabled = false;
            thirdPersonUserControl.enabled = false;
            isAlive = false;
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, weaponInUse.GetWeaponRange);
        }
    }
}

