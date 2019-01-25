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
    Transform projectileSpawnSpot;
    Player player;

    float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        SetupWeaponConfig();

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
        projectileSpawnSpot = weaponInUse.GetWeaponModel.GetComponentInChildren<ProjectileSpawnSpot>().gameObject.transform;
        Debug.Log(projectileSpawnSpot);
    }

    
    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifetime)
            Destroy(gameObject);

        if (initialForce == 0)
            GetComponent<Rigidbody>().velocity = transform.forward * speed;
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
