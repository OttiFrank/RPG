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
    GameObject playerWeapon;
    new Rigidbody rigidbody;
    CameraController cameraController;
    Collider col;

    float damage;
    float range;
    float speed;
    float initialForce;
    float lifetime;
    bool isFired;
    float screenWidth;
    float screenHeight;
    Transform projectileSpawnSpot;
    Player player;
    bool isShot = false;
    Camera currentCamera;
    Ray ray; 

    float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<BoxCollider>();        
        rigidbody = GetComponent<Rigidbody>();
        FindPlayer();
        GetCurrentCamera();
        GetScreenSize();
        SetupWeaponConfig();
    }

    private void GetScreenSize()
    {
        screenWidth = Screen.width / 2;
        screenHeight = Screen.height / 2;
        ray = currentCamera.ScreenPointToRay(new Vector3(screenWidth, screenHeight, 0));
    }   

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, ray.direction, Color.red); 
        if (isShot)
        {
            DestroyProjectile();

            if (initialForce == 0)
                GetComponent<Rigidbody>().velocity = transform.forward * speed;
        }
        GetCurrentCamera();
    }

    private void FindPlayer()
    {
        playerModel = GameObject.FindGameObjectWithTag("Player");
        player = playerModel.GetComponent<Player>();
        weaponInUse = player.GetPlayerWeaponConfig;
    }

    private void GetCurrentCamera()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        currentCamera = cameraController.GetCurrentCamera();
    }

    public void FireProjectile()
    {
        gameObject.transform.parent = null; 
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;

        
        rigidbody.velocity = ray.direction * initialForce;
        GameObject myArrow = gameObject;
        myArrow.transform.forward = Vector3.Slerp(myArrow.transform.forward, rigidbody.velocity.normalized, Time.deltaTime);
        StartCoroutine(RemoveTrigger());       
    }

    IEnumerator RemoveTrigger()
    {
        
        yield return new WaitForSeconds(.05f);
        col.isTrigger = false;
    }

    


    private void SetupWeaponConfig()
    {
        damage = weaponInUse.GetWeaponDamage;
        speed = weaponInUse.GetProjectileSpeed;
        initialForce = weaponInUse.GetInitialForce;
        lifetime = weaponInUse.GetLifetime;

        if (!isShot)
            rigidbody.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ;
    }

    
    

    private void DestroyProjectile()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.parent = collision.gameObject.transform;
        col.enabled = false;
        Debug.Log(collision.gameObject.name);
        rigidbody.AddRelativeForce(Vector3.zero);
        rigidbody.useGravity = false;      
        
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            GameObject enemy = collision.gameObject;
            var enemyComponent = enemy.GetComponentInParent<Enemy>();
            enemyComponent.TakeDamage(damage);
        }
    }

}
