using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealth;

    bool isAlive;
    float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        SetMaxHealth();    
    }

    private void SetMaxHealth()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy hit");
    }
}
