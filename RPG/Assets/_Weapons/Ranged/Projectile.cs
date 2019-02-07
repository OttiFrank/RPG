using RPG.Characters;
using RPG.Core;
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
    CameraController cameraController;
    Collider col;

    float damage;
    float range;
    float speed;
    float initialForce;
    float lifetime;
    bool justFired;
    Transform projectileSpawnSpot;
    Player player;
    bool isShot = false;
    Camera currentCamera;

    float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<BoxCollider>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>(); 
        rigidbody = GetComponent<Rigidbody>();
        if (!isShot)
            rigidbody.constraints = 
                RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezePositionY | 
                RigidbodyConstraints.FreezePositionZ; 
        FindPlayer();
        GetCurrentCamera();
        SetupWeaponConfig();

    }

    private void GetCurrentCamera()
    {
        currentCamera = cameraController.GetCurrentCamera(); 
    }

    public void FireProjectile()
    {
        gameObject.transform.parent = null; 
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;

        float x = Screen.width / 2;
        float y = Screen.height / 2;

        var ray = currentCamera.ScreenPointToRay(new Vector3(x, y, 0));
        rigidbody.velocity = ray.direction * 80;
        StartCoroutine(RemoveTrigger());
        
    }

    IEnumerator RemoveTrigger()
    {
        
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
        GetCurrentCamera();
        
    }

    private void DestroyProjectile()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        col.enabled = false;
        Debug.Log(collision.gameObject.name);
        rigidbody.AddRelativeForce(Vector3.zero);
        rigidbody.useGravity = false;
        
        transform.parent = collision.gameObject.transform;
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if(collision.gameObject.tag == "Enemy")
        {
            var enemyComponent = collision.gameObject.GetComponent<Enemy>();
            enemyComponent.TakeDamage(weaponInUse.GetWeaponDamage); 
        }
    }


}
