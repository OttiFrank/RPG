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
        [SerializeField] WeaponConfig weaponInDominantHand = null;
        [SerializeField] WeaponConfig weaponInOffHand = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] float maxStamina = 100.0f;
        [SerializeField] float staminaRecoveryRate = 10f;
        [SerializeField] float staminaCooldown = 2f;

        WeaponType mainType, offHandType;
        ThirdPersonUserControl thirdPersonUserControl;
        PlayerLog playerLog;
        GameObject mainHandWeaponPrefab;
        GameObject offHandWeaponPrefab;
        GameObject projectileModel;
        GameObject dominantHand, offHand;
        GameObject arrowHand;
        GameObject playerWeapon;
        GameObject projectile;
        GameObject bowRoot;
        GameObject stringSpawn;
        Camera currentCamera;
        CameraController cameraController;
        float currentHealth;
        float currentStamina;
        float mainWeaponTimeBetweenAttacks, offHandWeaponTimeBetweenAttacks;
        float attackTimer;
        float mainWeaponStaminaDrain, offHandWeaponStaminaDrain;
        float lastHitTimer = 0;
        bool isAlive;
        bool staminaCD = false;
        bool rangedWeapon = false;
        bool isLoaded = true;
        bool isUnarmed, isTwoHand; 
        AnimationEvent evt;
        Animator animator;
        Weapon mainWeapon, offHandWeapon;

        // Start is called before the first frame update
        void Start()
        {
            cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            if(cameraController != null) 
                currentCamera = cameraController.GetCurrentCamera();
            thirdPersonUserControl = GetComponent<ThirdPersonUserControl>();
            playerLog = GetComponent<PlayerLog>();

            SetupWeapon();
            if (!testMode)
                SetMaxCharacterResources();
            else
                SetTestResources();
            PutWeaponInHands();

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

            if (!isLoaded)
                PutArrowInBow();

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
            if (weaponInDominantHand == null && weaponInOffHand == null)
            {
                isUnarmed = true;
                return;
            }
            GetMainHandWeapon();
            if(!isTwoHand)
                GetOffHandWeapon();
        }

        private void GetOffHandWeapon()
        {
            offHandWeaponPrefab = weaponInDominantHand.GetWeaponModel;
            offHandType = weaponInDominantHand.type;
            offHandWeapon = offHandWeaponPrefab.GetComponent<Weapon>();
            offHandWeaponTimeBetweenAttacks = weaponInDominantHand.GetTimeBetweenAttacks;
            offHandWeaponStaminaDrain = weaponInDominantHand.GetStaminaDrain;
        }

        private void GetMainHandWeapon()
        {
            mainHandWeaponPrefab = weaponInDominantHand.GetWeaponModel;
            mainType = weaponInDominantHand.type;
            mainWeapon = mainHandWeaponPrefab.GetComponent<Weapon>();
            mainWeaponTimeBetweenAttacks = weaponInDominantHand.GetTimeBetweenAttacks;
            mainWeaponStaminaDrain = weaponInDominantHand.GetStaminaDrain;

            // Checks if the weapon used is two-handed or single handed
            WeaponHandle handle = weaponInDominantHand.handle;
            if (handle == WeaponHandle.TwoHands)
                isTwoHand = true;
            else
                isTwoHand = false;
        }

        private void PutWeaponInHands()
        {
            if (isUnarmed)
                return;
            PutWeaponInDominantHand();
            if(!isTwoHand)
                PutWeaponInOffHand();
            
        }

        private void PutWeaponInOffHand()
        {
            offHandWeaponPrefab = weaponInOffHand.GetWeaponModel; 
            if(weaponInOffHand != null && offHandWeaponPrefab != null)
            {
                offHand = RequestOffHand();
                var weapon = Instantiate(offHandWeaponPrefab, offHand.transform);
                weapon.transform.localPosition = weaponInOffHand.weaponGrip.localPosition;
                weapon.transform.localRotation = weaponInOffHand.weaponGrip.localRotation; 
            }
        }

        

        private void PutWeaponInDominantHand()
        {
            mainHandWeaponPrefab = weaponInDominantHand.GetWeaponModel;
            if (weaponInDominantHand != null && mainHandWeaponPrefab != null)
            {
                dominantHand = RequestDominantHand();
                var weapon = Instantiate(mainHandWeaponPrefab, dominantHand.transform);
                weapon.transform.localPosition = weaponInDominantHand.weaponGrip.localPosition;
                weapon.transform.localRotation = weaponInDominantHand.weaponGrip.localRotation;

                playerWeapon = GameObject.FindGameObjectWithTag("Weapon");

                // TODO: Change to bow only
                if (mainType == WeaponType.Ranged)
                {
                    rangedWeapon = true;
                    PutArrowInBow();

                }

            }
        }

        private void PutArrowInBow()
        {
            bowRoot = playerWeapon.transform.Find("Bow_Root").gameObject;
            bowRoot = bowRoot.transform.Find("Bow_Jnt").gameObject;
            if (bowRoot != null)
            {
                stringSpawn = bowRoot.transform.Find("String_jnt").gameObject;
                projectile = Instantiate(weaponInDominantHand.GetProjectilePrefab, stringSpawn.transform);
                projectile.transform.localPosition = stringSpawn.transform.localPosition + new Vector3(-0.25f, 0, 0);
                projectile.transform.localRotation = Quaternion.Euler(0f, -90f, 90f);
            }
            isLoaded = true;
        }

        private GameObject RequestOffHand()
        {
            var offHands = GetComponentsInChildren<OffHand>();
            int numberOfOffHands = offHands.Length;
            Assert.IsFalse(numberOfOffHands <= 0, "Couldn't find any off-hands, please add one");
            Assert.IsFalse(numberOfOffHands > 0, "Found multiple off-hands, please remove at least one");
            return offHands[0].gameObject;
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

        private void HandleUserInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (currentStamina >= mainWeaponStaminaDrain)
                    AttackTarget();
                else
                    playerLog.AddEvent("Can't do that, too low stamina");
            }
        }

        private void AttackTarget()
        {
            if (isAlive)
            {
                if (currentStamina >= mainWeaponStaminaDrain)
                {
                    if (Time.time - attackTimer > mainWeaponTimeBetweenAttacks)
                    {
                        if(!testMode)
                            currentStamina = currentStamina - mainWeaponStaminaDrain;
                        animator.SetTrigger("Attack");

                        if (rangedWeapon)
                        {
                            float x = Screen.width / 2;
                            float y = Screen.height / 2;

                            var ray = currentCamera.ScreenPointToRay(new Vector3(x, y, 0));


                            gameObject.transform.LookAt(ray.direction); 
                            playerWeapon.GetComponent<Animator>().SetTrigger("Shoot");
                            StartCoroutine(Shoot());
                            isLoaded = false;
                        }

                        attackTimer = Time.time;
                    }
                }

            }
        }

        IEnumerator Shoot()
        {
            yield return new WaitForSeconds(0.55f);
            projectile.GetComponent<Projectile>().FireProjectile(); 
        }

        public WeaponConfig GetPlayerWeaponConfig
        {
            get
            {
                return weaponInDominantHand;
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

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Enemy")
            {
                var enemyComponent = collision.gameObject.GetComponent<Enemy>();
                enemyComponent.TakeDamage(weaponInDominantHand.GetWeaponDamage); 
            }
        }
    }
}

