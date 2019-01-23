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
        [SerializeField] float timeBetweenAttacks;
        
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
