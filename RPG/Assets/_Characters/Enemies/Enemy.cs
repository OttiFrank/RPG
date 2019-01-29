using RPG.Characters;
using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealth = 100f;

    bool isAlive;
    float currentHealth;
    int hitCounter;
    Player player;
    WeaponConfig playerWeapon;

    public float healthAsPercentage
    {
        get
        {
            float healthAsPercentage = currentHealth / maxHealth;
            return healthAsPercentage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMaxHealth();
        FindPlayer();
        FindCurrentEquipedPlayerWeapon();
        hitCounter = 0;
    }

    private void FindCurrentEquipedPlayerWeapon()
    {
        playerWeapon = player.GetPlayerWeapon;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
    }

    private void SetMaxHealth()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void TakeDamage(float damage)
    {
        currentHealth = currentHealth - damage; 
    }
    private void OnTriggerEnter(Collider other)
    {
        float weaponDamage = playerWeapon.GetWeaponDamage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
        } else
        {
            TakeDamage(weaponDamage);
        }
        
       
        Debug.Log(currentHealth);
        Debug.Log(other.gameObject.name);

    }
}
