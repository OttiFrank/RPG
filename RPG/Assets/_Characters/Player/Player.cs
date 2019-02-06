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
        [SerializeField] bool testMode = false;
        [SerializeField] bool godMode = false;
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] WeaponConfig weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] float maxStamina = 100.0f;
        [SerializeField] float staminaRecoveryRate = 10f;
        [SerializeField] float staminaCooldown = 2f;

        WeaponType type;
        ThirdPersonUserControl thirdPersonUserControl;
        PlayerLog playerLog;
        GameObject weaponPrefab;
        GameObject projectileModel;
        GameObject dominantHand;
        GameObject arrowHand;
        GameObject playerWeapon;
        float currentHealth;
        float currentStamina;
        float timeBetweenAttacks;
        float attackTimer;
        float staminaDrain;
        float lastHitTimer = 0;
        bool isAlive;
        bool staminaCD = false;
        bool rangedWeapon = false;
        Animator animator;
        Weapon weapon;

        // Start is called before the first frame update
        void Start()
        {
            thirdPersonUserControl = GetComponent<ThirdPersonUserControl>();
            playerLog = GetComponent<PlayerLog>();

            SetupWeapon();
            if (!testMode)
                SetMaxCharacterResources();
            else
                SetTestResources();
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
                if (!staminaCD)
                    HandleStaminaRecovery();
            }

        }

        private void HandleStaminaRecovery()
        {
            if (Time.time - lastHitTimer > staminaCooldown || currentStamina < maxStamina)
            {
                StartCoroutine(StaminaRecoveryRate(staminaCooldown));
            }
        }

        IEnumerator StaminaRecoveryRate(float cooldown)
        {
            staminaCD = true;
            yield return new WaitForSeconds(cooldown);
            if (currentStamina < maxStamina)
            {
                currentStamina = currentStamina + staminaRecoveryRate;
                Debug.Log("Added stamina");
            }
            staminaCD = false;


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
                if (type == WeaponType.Ranged)
                {
                    rangedWeapon = true;

                    playerWeapon = GameObject.FindGameObjectWithTag("Weapon");
                    GameObject bowRoot;
                    GameObject stringSpawn;

                    bowRoot = playerWeapon.transform.Find("Bow_Root").gameObject;
                    bowRoot = bowRoot.transform.Find("Bow_Jnt").gameObject;
                    if (bowRoot != null)
                    {
                        stringSpawn = bowRoot.transform.Find("String_jnt").gameObject;
                        Debug.Log(stringSpawn);
                        var projectile = Instantiate(weaponInUse.GetProjectilePrefab, stringSpawn.transform);
                        projectile.transform.localPosition = stringSpawn.transform.localPosition + new Vector3(-0.25f, 0,0);
                        projectile.transform.localRotation = Quaternion.Euler(0f, -90f, 90f);
                    }
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

        private void SetTestResources()
        {
            isAlive = true;
            currentHealth = (maxHealth / 2);
            currentStamina = (maxStamina / 2);
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
                if (currentStamina >= staminaDrain)
                    AttackTarget();
                else
                    playerLog.AddEvent("Can't do that, too low stamina");
            }
        }

        private void AttackTarget()
        {
            if (isAlive)
            {
                if (currentStamina >= staminaDrain)
                {
                    if (Time.time - attackTimer > timeBetweenAttacks)
                    {
                        currentStamina = currentStamina - staminaDrain;
                        animator.SetTrigger("Attack");

                        if (rangedWeapon)
                        {
                            playerWeapon.GetComponent<Animator>().SetTrigger("Shoot");


                        }

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
            if (godMode)
                return;
            if (isAlive)
            {
                if (currentHealth > 0 && currentHealth <= maxHealth)
                {
                    currentHealth = currentHealth - damage;
                    lastHitTimer = Time.time;
                }

                if (currentHealth >= maxHealth)
                    currentHealth = maxHealth;

                //Debug.Log("Player current health:" + currentHealth); 
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

