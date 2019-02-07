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
    Rigidbody rigidbody;

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
        rigidbody = GetComponent<Rigidbody>();
        if (!isShot)
            rigidbody.constraints = 
                RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezePositionY | 
                RigidbodyConstraints.FreezePositionZ; 
        FindPlayer();
        SetupWeaponConfig();

    }
    public void FireProjectile()
    {
        gameObject.transform.parent = null; 
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        
        rigidbody.AddRelativeForce(Vector3.forward * 2500);
        StartCoroutine(RemoveTrigger());
        
    }

    IEnumerator RemoveTrigger()
    {
        Collider col = gameObject.GetComponent<BoxCollider>();
        yield return new WaitForSeconds(.05f);
        col.isTrigger = false;
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
        Debug.Log(collision.gameObject.name);
    }

}
