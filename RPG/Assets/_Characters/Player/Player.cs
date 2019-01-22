using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {
        [SerializeField] bool godMode = false;
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] WeaponConfig weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        float currentHealth;
        bool isAlive;
        Animator animator;


        // Start is called before the first frame update
        void Start()
        {
            isAlive = true;
            SetMaxCharacterHealth();
            SetupRuntimeAnimator();
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
            currentHealth = maxHealth;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["Basic Attack"] = weaponInUse.GetAttackAnimation;
        }

        private void HandleUserInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}

