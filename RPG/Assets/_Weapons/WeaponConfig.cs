using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    public enum WeaponType
    {
        Ranged,
        Melee,
        Unarmed
    }
    [CreateAssetMenu(menuName = ("RPG/Create new weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public bool AIWeapon = false;
        public Transform weaponGrip;
        public WeaponType type = WeaponType.Ranged;
        [SerializeField] GameObject weaponModel = null;
        [SerializeField] float weaponDamage = 75f;
        [SerializeField] float weaponRange = 9f;
        [SerializeField] float timeBetweenAttacks = 4f;
        [SerializeField] float staminaDrain = 50f;
        [SerializeField] AnimationClip attackAnim = null;
        [SerializeField] AnimationClip idleAnim = null;
        [SerializeField] AnimationClip deathAnim = null;
        [SerializeField] AnimationClip runAnimation = null;

        //Projectile
        [SerializeField] float initialForce = 15f;
        [SerializeField] float speed = 25f;
        [SerializeField] float lifetime = 5f;
        [SerializeField] Transform projectileSpawnSpot = null;
        [SerializeField] GameObject projectilePrefab = null;

        // Getters
        public GameObject GetWeaponModel
        {
            get
            {
                return weaponModel;
            }
        }
        public float GetWeaponDamage
        {
            get
            {
                return weaponDamage;
            }
        }
        public float GetWeaponRange
        {
            get
            {
                return weaponRange;
            }
        }
        public float GetTimeBetweenAttacks
        {
            get
            {
                return timeBetweenAttacks;
            }
        }
        public float GetStaminaDrain
        {
            get
            {
                return staminaDrain; 
            }
        }

        // Projectile 
        public float GetInitialForce
        {
            get
            {
                return initialForce;
            }
        }
        public float GetProjectileSpeed
        {
            get
            {
                return speed;
            }
        }
        public float GetLifetime
        {
            get
            {
                return lifetime;
            }
        }
        public Transform GetProjectileSpawnSpot
        {
            get
            {
                return projectileSpawnSpot;
            }
        }
        public GameObject GetProjectilePrefab
        {
            get
            {
                return projectilePrefab;
            }
        }

        // Animations
        public AnimationClip GetRunningAnimation
        {
            get
            {
                runAnimation.events = new AnimationEvent[0];
                return runAnimation;
            }
        }
        public AnimationClip GetDeathAnimation
        {
            get
            {
                deathAnim.events = new AnimationEvent[0];
                return deathAnim;
            }
        }
        public AnimationClip GetIdleAnimation
        {
            get
            {
                idleAnim.events = new AnimationEvent[0];
                return idleAnim;
            }
        }
        public AnimationClip GetAttackAnimation
        {
            get
            {
                attackAnim.events = new AnimationEvent[0];
                return attackAnim;
            }
        }

        public void setEnemyWeapon(bool value)
        {
            AIWeapon = value;
        }
    }
}
