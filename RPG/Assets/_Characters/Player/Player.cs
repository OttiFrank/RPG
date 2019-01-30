using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] bool godMode = false;
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] WeaponConfig weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        WeaponType type;
        GameObject weaponPrefab;
        GameObject projectileModel;
        GameObject dominantHand;
        GameObject arrowHand;
        float currentHealth;
        float timeBetweenAttacks;
        float attackTimer;
        bool isAlive;
        bool canAttack = true;
        Animator animator;
        Weapon weapon;


        // Start is called before the first frame update
        void Start()
        {
            SetupWeapon();
            SetMaxCharacterHealth();
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
            }
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
                if(type == WeaponType.Ranged)
                {
                    projectileModel = weaponInUse.GetProjectilePrefab;
                    arrowHand = RequestArrowHand();
                    var projectile = Instantiate(projectileModel, arrowHand.transform);
                    projectile.transform.localPosition = weaponInUse.weaponGrip.localPosition;
                    projectile.transform.localRotation = weaponInUse.weaponGrip.localRotation;
                }
                
            }        
        }

        private GameObject RequestArrowHand()
        {
            var arrowHands = GetComponentsInChildren<ArrowHand>();
            int numberOfArrowHands = arrowHands.Length;
            Assert.IsFalse(numberOfArrowHands <= 0, "Could not find any arrow hands, please add one");
            Assert.IsFalse(numberOfArrowHands > 1, "Found multiple arrow hands, please remove at least one");
            return arrowHands[0].gameObject;
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "Could not find any dominant hands, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Found multiple dominant hands, please remove one");
            return dominantHands[0].gameObject; 
        }

        

        private void SetMaxCharacterHealth()
        {
            isAlive = true;
            currentHealth = maxHealth;
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
                if (Time.time - attackTimer > timeBetweenAttacks)
                {
                    animator.SetTrigger("Attack");
                    attackTimer = Time.time;
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
            throw new NotImplementedException();
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, weaponInUse.GetWeaponRange);
        }
    }
}

