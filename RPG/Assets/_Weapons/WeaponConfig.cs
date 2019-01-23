using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    public enum WeaponType
    {
        Ranged,
        Melee
    }
    [CreateAssetMenu(menuName = ("RPG/Create new weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform weaponGrip;
        public WeaponType type = WeaponType.Ranged;
        [SerializeField] GameObject weaponModel;
        [SerializeField] float weaponDamage;
        [SerializeField] float weaponRange;
        [SerializeField] AnimationClip attackAnim;
        [SerializeField] AnimationClip idleAnim;
        [SerializeField] AnimationClip deathAnim;
        [SerializeField] float timeBetweenAttacks;
        [SerializeField] AnimationClip runAnimation;

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
    }
}
