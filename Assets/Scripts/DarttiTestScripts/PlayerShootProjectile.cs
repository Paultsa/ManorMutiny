using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//Dartti
/*
public class PlayerShootProjectile : MonoBehaviour
{
    public GameObject pfBullet;
    public Camera main_camera;
    public Transform gun;
    public bool rapid_fire = false;
    private float timer = 0.5f;

    private void Start()
    {

    }
    // Start is called before the first frame update
    void Update()
    {
        //DEBUG
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("TestSceneShooting");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Pressed left click.");
            //Shoot(ammunition.pistol);
            //Shoot(ammunition.shotgun);
            //Shoot(ammunition.fist);
            Shoot(ammunition.fist_melee);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Pressed right click.");
            //Shoot(ammunition.mortar);
            //Shoot(ammunition.rocketlauncher);
            Shoot(ammunition.railgun);
        }
        
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Pressed middle click.");
            //Shoot(ammunition.shotgun);
            Shoot(ammunition.knife_melee);
        }
        

        
       //if (Input.GetMouseButton(2))
       //{
       //    Debug.Log("Pressed middle click.");
       //    rapid_fire = true;
       //
       //    timer -= Time.deltaTime;
       //    if (timer <= 0)
       //    {
       //        Shoot(ammunition.minigun);
       //        timer = 0.1f;
       //    }
       //}
       //
       //if (!Input.GetMouseButton(2))
       //{
       //    rapid_fire = false;
       //    timer = 0.5f;
       //}
        
    }

 private void Shoot(weapon ammotype)
    {
        float debug_time = 0.5f;

        //Shoot
        GameObject newBullet = Instantiate(pfBullet, gun.transform.position + main_camera.transform.forward, Quaternion.identity);
        newBullet.transform.up = main_camera.transform.forward;
        newBullet.GetComponent<BulletMovement>().player = this.gameObject;
        var bullet_movement = newBullet.GetComponent<BulletMovement>();
        var rigidbody = newBullet.GetComponent<Rigidbody>();
        var collider = newBullet.GetComponent<CapsuleCollider>();
        bullet_movement.weapon_type = ammotype;

        switch (ammotype)
        {
            case weapon.revolver:
                bullet_movement.damage = 20f;
                bullet_movement.range = 30f;
                bullet_movement.knock_back = 3f;
                bullet_movement.lifeDuration = debug_time;
                break;
            case weapon.shotgun:
                bullet_movement.damage = 4f;
                bullet_movement.range = 20f;
                bullet_movement.spread = 0.4f;
                bullet_movement.shells = 20;
                bullet_movement.lose_damage_over_distence = true;
                bullet_movement.knock_back = 1f;
                bullet_movement.lifeDuration = debug_time;
                bullet_movement.thrust_force = 0.5f;
                break;
            case weapon.chaingun:
                bullet_movement.damage = 15f;
                bullet_movement.range = 25f;
                bullet_movement.spread = 0.2f;
                bullet_movement.knock_back = 2f;
                bullet_movement.lifeDuration = debug_time;
                bullet_movement.thrust_force = 0.02f;
                break;
            case weapon.lightningGun:
                bullet_movement.damage = 25f;
                bullet_movement.range = 25f;
                bullet_movement.knock_back = 12f;
                bullet_movement.lifeDuration = 0.1f;
                bullet_movement.thrust_force = 5f;
                break;
            case weapon.grenadeLauncher:
                bullet_movement.speed = 0f;
                bullet_movement.lifeDuration = 10f;
                bullet_movement.thrust_force = 2f;
                rigidbody.useGravity = true;
                collider.isTrigger = false;
                break;
            case weapon.rocketLauncher:
                bullet_movement.damage = 5f;
                bullet_movement.speed = 10f;
                bullet_movement.lifeDuration = 5f;
                break;
            case weapon.rocketFist:
                bullet_movement.damage = 5f;
                bullet_movement.speed = 10f;
                bullet_movement.lifeDuration = 15f;
                bullet_movement.knock_back = 15f;
                break;
            case weapon.meleeFist:
                bullet_movement.damage = 20f;
                bullet_movement.speed = 0;
                bullet_movement.lifeDuration = 0.1f;
                bullet_movement.knock_back = 10f;
                break;
            case weapon.meleeBlade:
                bullet_movement.damage = 30f;
                bullet_movement.speed = 0;
                bullet_movement.lifeDuration = 0.1f;
                bullet_movement.knock_back = 5f;
                break;
        }

    }


}
*/
