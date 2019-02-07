using RPG.Characters;
using RPG.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    GameObject playerModel;
    WeaponConfig weaponInUse;

    float damage;
    float range;
    float speed;
    float initialForce;
    float lifetime;
    bool justFired;
    Transform projectileSpawnSpot;
    Player player;
    bool isShot = false;

    float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        SetupWeaponConfig();

    }
    public void FireProjectile()
    {

    }

    private void FindPlayer()
    {
        playerModel = GameObject.FindGameObjectWithTag("Player");
        player = playerModel.GetComponent<Player>();
        weaponInUse = player.GetPlayerWeapon;
    }


    private void SetupWeaponConfig()
    {
        damage = weaponInUse.GetWeaponDamage;
        speed = weaponInUse.GetProjectileSpeed;
        initialForce = weaponInUse.GetInitialForce;
        lifetime = weaponInUse.GetLifetime;
    }

    
    // Update is called once per frame
    void Update()
    {
        if(isShot)
        {
            DestroyProjectile();            

            if (initialForce == 0)
                GetComponent<Rigidbody>().velocity = transform.forward * speed;
        }
        
    }

    private void DestroyProjectile()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision);
    }

    private void Hit(Collision col)
    {
        player.TakeDamage(damage); 
    }

}
