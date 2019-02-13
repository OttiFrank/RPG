using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] bool testMode = false;
        [SerializeField] bool godMode = false;
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] WeaponConfig weaponInDominantHand = null;
        [SerializeField] WeaponConfig weaponInOffHand = null;
        [SerializeField] float maxStamina = 100.0f;
        [SerializeField] float staminaRecoveryRate = 10f;
        [SerializeField] float staminaCooldown = 2f;


        WeaponType mainWeaponType, offHandType;
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
        float mainWeaponStaminaDrain;
        float offHandWeaponStaminaDrain;
        float lastHitTimer = 0;
        bool isAlive;
        bool staminaCD = false;
        bool rangedWeapon = false;
        bool isLoaded = true;
        bool isTwoHand;
        bool isWeaponWielded;
        AnimationEvent evt;
        Weapon mainWeapon, offHandWeapon;

        // Start is called before the first frame update
        void Start()
        {
            cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            if (cameraController != null)
                currentCamera = cameraController.GetCurrentCamera();
            playerLog = GetComponent<PlayerLog>();

            GetMainHandWeapon();
            // If type is 2H the Off hand will not be instantiated
            if (!isTwoHand)
                GetOffHandWeapon();
            PutWeaponInHands();
            if (!testMode)
                SetMaxCharacterResources();
            else
                SetTestResources();


            mainHandWeaponPrefab = weaponInDominantHand.GetWeaponModel;

            isWeaponWielded = true;
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

            if (rangedWeapon)
                if (!isLoaded)
                    PutArrowInBow();
        }

        private void HandleUserInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log(isWeaponWielded);
                if(isWeaponWielded)
                {
                    if (currentStamina >= offHandWeaponStaminaDrain)
                    {
                        var offHand = GameObject.Find("LeftHand");
                        GameObject offHandWeapon = offHand.transform.GetChild(0).gameObject;
                        if (isAlive)
                            AttackTarget(offHandWeapon, false);

                    }
                    else
                        playerLog.AddEvent("Can't do that, too low stamina");
                }
                
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if(isWeaponWielded)
                {
                    if (currentStamina >= mainWeaponStaminaDrain)
                    {
                        var mainHand = GameObject.Find("RightHand");
                        GameObject mainHandWeapon = mainHand.transform.GetChild(0).gameObject;
                        if (isAlive)
                            AttackTarget(mainHandWeapon, true);
                    }
                    else
                        playerLog.AddEvent("Can't do that, too low stamina");
                }                
            }

            if (Input.GetButtonDown("Sheath Weapons"))
            {
                SheathWeapons();
            }

        }

        private void SheathWeapons()
        {
            

            Debug.Log(isWeaponWielded); 
            GameObject weaponLeftParent = GameObject.Find("LeftHand").gameObject;
            GameObject weaponRightParent = GameObject.Find("RightHand").gameObject;

            GameObject weaponLeftHand = weaponLeftParent.transform.GetChild(0).gameObject;
            GameObject weaponRightHand = weaponRightParent.transform.GetChild(0).gameObject;

            Animator mainHandWeaponAnimator = weaponRightHand.GetComponent<Animator>();
            Animator offHandWeaponAnimator = weaponLeftHand.GetComponent<Animator>();
            Debug.Log(offHandWeaponAnimator.runtimeAnimatorController); 
            if(isWeaponWielded)
            {
                mainHandWeaponAnimator.SetBool("isWeaponWielded", isWeaponWielded);
                mainHandWeaponAnimator.SetBool("isMainWeapon", true);
                offHandWeaponAnimator.SetBool("isOffWeapon", true); 
                offHandWeaponAnimator.SetBool("isWeaponWielded", isWeaponWielded);
            } else
            {
                mainHandWeaponAnimator.SetBool("isWeaponWielded", isWeaponWielded);
                offHandWeaponAnimator.SetBool("isWeaponWielded", isWeaponWielded); 

                //TODO: Make sure that stamina doesn't drain until weapon wield animation is fully done 
            }
            isWeaponWielded = !isWeaponWielded;

        }

        private void AttackTarget(GameObject weapon, bool isMainHand)
        {
            Animator weaponAnimator = weapon.GetComponent<Animator>();
            if (isMainHand)
            {
                currentStamina = currentStamina - mainWeaponStaminaDrain;
                weaponAnimator.SetTrigger("MainHandAttack");

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

            }
            if (!isMainHand)
            {
                currentStamina = currentStamina - offHandWeaponStaminaDrain;
                weaponAnimator.SetTrigger("OffHandAttack");
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



        private void GetOffHandWeapon()
        {
            offHandWeaponPrefab = weaponInOffHand.GetWeaponModel;
            offHandType = weaponInOffHand.type;
            offHandWeapon = offHandWeaponPrefab.GetComponent<Weapon>();
            offHandWeaponStaminaDrain = weaponInOffHand.GetStaminaDrain;
        }

        private void GetMainHandWeapon()
        {
            mainHandWeaponPrefab = weaponInDominantHand.GetWeaponModel;
            mainWeaponType = weaponInDominantHand.type;
            mainWeapon = mainHandWeaponPrefab.GetComponent<Weapon>();
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
            PutWeaponInDominantHand();
            if (!isTwoHand)
                PutWeaponInOffHand();

        }

        private void PutWeaponInOffHand()
        {

            if (weaponInOffHand != null && offHandWeaponPrefab != null)
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
                if (mainWeaponType == WeaponType.Ranged)
                {
                    rangedWeapon = true;
                    //PutArrowInBow();

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
            Assert.IsFalse(numberOfOffHands > 1, "Found multiple off-hands, please remove at least one");
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
            isAlive = false;
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.tag == "Enemy")
            {
                var enemyComponent = collision.gameObject.GetComponent<Enemy>();
                enemyComponent.TakeDamage(weaponInDominantHand.GetWeaponDamage);
            }
        }
    }
}

