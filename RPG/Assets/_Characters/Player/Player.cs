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

        GameObject weaponModel;
        GameObject dominantHand;
        float currentHealth;
        float timeBetweenAttacks;
        float attackTimer;
        bool isAlive;
        bool canAttack = true;
        Animator animator;


        // Start is called before the first frame update
        void Start()
        {
            SetMaxCharacterHealth();
            PutWeaponInHands();
            SetupRuntimeAnimator();

            timeBetweenAttacks = weaponInUse.GetTimeBetweenAttacks;
            attackTimer = 0f;
        }

        private void PutWeaponInHands()
        {
            weaponModel = weaponInUse.GetWeaponModel;
            if (weaponInUse != null && weaponModel != null)
            {                
                dominantHand = RequestDominantHand();
                var weapon = Instantiate(weaponModel, dominantHand.transform);
                weapon.transform.localPosition = dominantHand.transform.localPosition;
                weapon.transform.localRotation = dominantHand.transform.localRotation;
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




        // Update is called once per frame
        void Update()
        {
            

            if (isAlive)
            {
                HandleUserInput();
            }
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

        public float TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }
    }
}

