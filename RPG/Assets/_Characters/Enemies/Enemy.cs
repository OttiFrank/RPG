using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealth = 100f;

    bool isAlive;
    float currentHealth;

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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
