using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
//Dartti
/*
public enum ammunition
{
    pistol,
    shotgun,
    mortar,
    railgun,
    minigun,
    rocketlauncher,
    fist,
    fist_melee,
    knife_melee
}*/
//Kostin koodimuistiinpanot: mihin switch weapon, drop,

public class GunController : MonoBehaviour
{
    // Multiplayer stuff :
    private bool ownerIsLocal = true; // flag is this owned by local player or not
    private NetworkPlayer networkScript;

    //Bullet & Shoot
    public GameObject pfBullet;
    public Camera main_camera;
    public bool rapid_fire = false;
    private float timer = 0.5f;

    //public GameObject muzzleFlash;
    public SpriteAnimator spriteAnim;
    public GunAnimationPaths gunPaths;
    public AnimatorController animController;
    public DecalHandler decal_handler;

    //public float ammo = 10;
    public float spamShotCooldown;
    float cooldownTimer = 0;
    public bool shot = false;

    //Gun
    public int current_weapon;
    public weapon[] weapons;
    public int[] ammos;
    public int inventory_size = 2;
    public float[] weapon_cooldown;
    public float cooldown = 0;
    public Light flash;
    private int flashTime = 0;

    //KostinKoodi


    void Start()
    {
        animController = transform.parent.parent.GetComponent<AnimatorController>();
        // Figure out are we in multiplayer?
        networkScript = GetParentPlayerObj().transform.parent.GetComponent<NetworkPlayer>();
        // If we are -> check is the owning player object local or not?
        if (networkScript != null)
            ownerIsLocal = networkScript.isLocalPlayer;

        weapon_cooldown = new float[(int)weapon.total];
        weapon_cooldown[(int)weapon.empty]              = -1;
        weapon_cooldown[(int)weapon.revolver]           = 0;
        weapon_cooldown[(int)weapon.chaingun]           = 0;
        weapon_cooldown[(int)weapon.rocketLauncher]     = 1f;
        weapon_cooldown[(int)weapon.grenadeLauncher]    = 0.6f;
        weapon_cooldown[(int)weapon.shotgun]            = 0.8f;
        weapon_cooldown[(int)weapon.lightningGun]       = 1.5f;
        weapon_cooldown[(int)weapon.rocketFist]         = 7f;
        weapon_cooldown[(int)weapon.meleeFist]          = 1f;
        weapon_cooldown[(int)weapon.meleeBlade]         = int.MaxValue;

        current_weapon = inventory_size;
        weapons = new weapon[inventory_size+1];
        ammos = new int[inventory_size+1];

        for (var i = 0; i < inventory_size; i++)
        {
            weapons[i] = weapon.empty;
            ammos[i] = -1;
        }

        //Melee
        weapons[inventory_size]  = weapon.meleeFist;
        ammos[inventory_size]    = int.MaxValue;

        spriteAnim = GetComponent<SpriteAnimator>();

        //weapons[1] = weapon.lightningGun;
        //ammos[1] = 5;

       
    }

    // Returns the Player game object which owns this GunController
    GameObject GetParentPlayerObj()
    {
        // Gun  / Camera / Player
        return transform.parent.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (flashTime > 0) { flash.enabled = true; } else { flash.enabled = false; }
        flashTime--;
        // Updatetaan GunControlleria vaan lokaalille pelaajalle..
        bool allowUpdate = networkScript != null;

        if (allowUpdate)
            allowUpdate = networkScript.isLocalPlayer;
        else
            allowUpdate = true;
        
        if (GameManager.gameManager && allowUpdate && !GameManager.gameManager.gamePaused && GameManager.gameManager.playersFound)
            UpdateGunController();
    }

    // Entinen Update() kontentti siirretty tähän funktioon
    void UpdateGunController()
    {
        //Debug.Log(cooldown);
        if (GameManager.gameManager != null)
        {
            int playerIndex = 0;
            if (transform.parent.parent.CompareTag("PlayerTwo"))
            {
                playerIndex = 1;
            }
            GameManager.gameManager.players[playerIndex].chosenWeapon = current_weapon;
            GameManager.gameManager.players[playerIndex].primaryWeapon = weapons[0];
            GameManager.gameManager.players[playerIndex].secondaryWeapon = weapons[1];
            GameManager.gameManager.players[playerIndex].thirdWeapon = weapons[2];
            GameManager.gameManager.players[playerIndex].ammo = ammos[current_weapon];
            GameManager.gameManager.players[playerIndex].ammos = ammos;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            current_weapon = inventory_size;
            spriteAnim.ChangeState(_States.IDLE);
            gunPaths.currentWeapon = weapons[current_weapon];
        }

        //Drop weapon
        if (Input.GetKeyDown(KeyCode.G))
        {
            //KostinKoodi
            EmptySound(weapons[current_weapon]);
            weapons[current_weapon] = weapon.empty;
            ammos[current_weapon] = -1;

            current_weapon = inventory_size;
            //weapons[inventory_size] = weapon.meleeFist;
            //ammos[inventory_size] = int.MaxValue;
            gunPaths.currentWeapon = weapon.meleeFist;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            current_weapon = 0;
            gunPaths.currentWeapon = weapons[current_weapon];

            if (weapons[current_weapon] == weapon.empty)
            {
                current_weapon = inventory_size;
                //weapons[inventory_size] = weapon.meleeFist;
                //ammos[inventory_size] = int.MaxValue;
                gunPaths.currentWeapon = weapon.meleeFist;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            current_weapon = 1;
            gunPaths.currentWeapon = weapons[current_weapon];

            if (weapons[current_weapon] == weapon.empty)
            {
                current_weapon = inventory_size;
                //weapons[inventory_size] = weapon.meleeFist;
                //ammos[inventory_size] = int.MaxValue;
                gunPaths.currentWeapon = weapon.meleeFist;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            current_weapon = (current_weapon == 0) ? 1 : 0;

            if (ammos[current_weapon] <= 0)
            {
                current_weapon = inventory_size;
                //weapons[inventory_size] = weapon.meleeFist;
                //ammos[inventory_size] = int.MaxValue;
            }

                gunPaths.currentWeapon = weapons[current_weapon];
            
        }

        ///////////

        if (Input.GetMouseButtonDown(0)
            && ammos[current_weapon] >= 0
            && weapons[current_weapon] != weapon.empty 
            && cooldown <= 0
            && (shot == false || weapons[current_weapon] == weapon.revolver)
            && animController.switchingWeapon == false)
        {
            spriteAnim.ChangeState(_States.SHOOT);
            shot = true;
            //cooldownTimer = spamShotCooldown;
            //spriteAnim.ChangeState(_States.SHOOT);
            Shoot(gunPaths.currentWeapon);

            ammos[current_weapon]--;
            cooldownTimer = 0;
            cooldown = weapon_cooldown[(int)weapons[current_weapon]];
        }
        else if (ammos[current_weapon] == 0 && cooldown <= 0 && animController.switchingWeapon == false)
        {
            EmptySound(weapons[current_weapon]);
            gunPaths.currentWeapon = weapon.empty;
            weapons[current_weapon] = weapon.empty;
            ammos[current_weapon] = -1;
            shot = false;

            //Swap new weapon if found
            for (var i = 0; i < inventory_size; i++)
            {
                if (weapons[i] != weapon.empty)
                {
                    current_weapon = i;
                    gunPaths.currentWeapon = weapons[current_weapon];
                    break;
                }
                current_weapon = inventory_size;
            }
        }


        if (spriteAnim.animDone && cooldownTimer >= spamShotCooldown)
        {
            gunPaths.currentWeapon = weapons[current_weapon];
            //if (ammos[current_weapon] >= 0)
            //{
            if (Input.GetMouseButton(0) && animController.switchingWeapon == false)
                {
                    if (cooldown <= 0)
                    {
                        if (ammos[current_weapon] > 0)
                        {
                            spriteAnim.ChangeState(_States.SHOOT);
                            Shoot(gunPaths.currentWeapon);
                            ammos[current_weapon]--;
                            cooldownTimer = 0;
                            cooldown = weapon_cooldown[(int)weapons[current_weapon]];
                        }
                        else
                        {
                            EmptySound(weapons[current_weapon]);
                            gunPaths.currentWeapon = weapon.empty;
                            weapons[current_weapon] = weapon.empty;
                            ammos[current_weapon] = -1;
                            shot = false;

                            //Swap new weapon if found
                            for (var i = 0; i < inventory_size; i++)
                            {
                                if (weapons[i] != weapon.empty)
                                {
                                    current_weapon = i;
                                    gunPaths.currentWeapon = weapons[current_weapon];
                                    spriteAnim.ChangeState(_States.IDLE);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        shot = false;
                        spriteAnim.ChangeState(_States.IDLE);
                    }
                }
                else
                {
                    shot = false;
                    spriteAnim.ChangeState(_States.IDLE);
                }
                /*
            }
            else
            {
                shot = false;
                spriteAnim.ChangeState(_States.IDLE);
            }
                */
        }
        else
        {
            cooldownTimer += Time.deltaTime;
            cooldown -= Time.deltaTime;
        }
    }


    // Shoots "to server"
    public void Shoot_server(weapon ammotype)
    {
        byte currentWeapon_byte = NetworkPlayer.CastWeaponNameToByte(ammotype);
        Package attackPackage = new Package(Client.Instance.RoomId, Client.Instance.Id, NetworkingCommon.MESSAGE_TYPE__StorePlayerAttack, 1, new byte[1] { currentWeapon_byte });
        Client.Instance.SendData(attackPackage.Buffer.Bytes);

        Debug.Log("SERVER SHOOT!");
    }

    // Performs Shoot locally
    // (Kopioitu kaikki vaan aiemmasta "Shoot(weapon ammotype)" - funktiosta)
    public void Shoot_local(weapon ammotype)
    {
        float debug_time = 0f;

        //Shoot
        GameObject newBullet = Instantiate(pfBullet, main_camera.transform.position + main_camera.transform.forward / 3, Quaternion.identity);
        newBullet.transform.up = main_camera.transform.forward;
        newBullet.GetComponent<Bullets>().player = this.transform.parent.parent.gameObject; //this.gameObject;
        newBullet.GetComponent<Bullets>().gunPosition = this.transform;
        var bullet_movement = newBullet.GetComponent<Bullets>();
        var rigidbody = newBullet.GetComponent<Rigidbody>();
        var collider = newBullet.GetComponent<CapsuleCollider>();
        bullet_movement.weapon_type = ammotype;
        bullet_movement.decal_handler = decal_handler;
        ShootSound(weapons[current_weapon]);
        flashTime = 5;

        switch (ammotype)
        {
            case weapon.revolver:
                bullet_movement.damage = 20f;
                bullet_movement.range = 30f;
                bullet_movement.knock_back = 3f;
                bullet_movement.lifeDuration = debug_time;

                break;
            case weapon.shotgun:
                bullet_movement.damage = 7f;
                bullet_movement.range = 25f;
                bullet_movement.spread = 0.35f;
                bullet_movement.shells = 20;
                //bullet_movement.lose_damage_over_distence = true;
                bullet_movement.knock_back = 1f;
                bullet_movement.lifeDuration = debug_time;
                bullet_movement.thrust_force = 2f;
                break;
            case weapon.chaingun:
                bullet_movement.damage = 17f;
                bullet_movement.range = 30f;
                bullet_movement.spread = 0.12f;
                bullet_movement.knock_back = 2f;
                bullet_movement.lifeDuration = debug_time;
                bullet_movement.thrust_force = 0.02f;
                break;
            case weapon.lightningGun:
                bullet_movement.damage = 150f;
                bullet_movement.speed = 0f;
                bullet_movement.range = 10f;
                bullet_movement.knock_back = 12f;
                bullet_movement.lifeDuration = 0.4f;
                bullet_movement.thrust_force = 6f;
                bullet_movement.blastRadius = 2f;
                collider.isTrigger = false;
                break;
            case weapon.grenadeLauncher:
                bullet_movement.damage = 50f;
                bullet_movement.speed = 0f;
                bullet_movement.lifeDuration = 4f;
                bullet_movement.thrust_force = 4f;
                bullet_movement.blastRadius = 5f;
                bullet_movement.knock_back = 9f;
                rigidbody.useGravity = true;
                collider.isTrigger = false;
                break;
            case weapon.rocketLauncher:
                bullet_movement.damage = 65f;
                bullet_movement.speed = 10f;
                bullet_movement.lifeDuration = 5f;
                bullet_movement.blastRadius = 4.5f;
                bullet_movement.knock_back = 10f;
                bullet_movement.thrust_force = 4f;
                collider.isTrigger = false;
                break;
            case weapon.rocketFist:
                bullet_movement.damage = 5f;
                bullet_movement.speed = 10f;
                bullet_movement.lifeDuration = 15f;
                bullet_movement.knock_back = 15f;
                break;
            case weapon.meleeFist:
                bullet_movement.damage = 25f;
                bullet_movement.speed = 0;
                bullet_movement.lifeDuration = 0.1f;
                bullet_movement.knock_back = 5f;
                bullet_movement.blastRadius = 1f;
                flashTime = 0;
                break;
            case weapon.meleeBlade:
                bullet_movement.damage = 30f;
                bullet_movement.speed = 0;
                bullet_movement.lifeDuration = 0.1f;
                bullet_movement.knock_back = 5f;
                break;
        }
    }

    public void Shoot(weapon ammotype)
    {
        if (networkScript != null) // meaning, we are in multiplayer currently.. (theres no NetworkPlayer script in singleplayer)
            Shoot_server(ammotype);
        else // singleplayer
            Shoot_local(ammotype);
    }

    private void EmptySound(weapon weapon_type)
    {
         //KostinKoodi
        //WeaponFireSound = FMODUnity.RuntimeManager.CreateInstance(sound_path);

        string sound_path = "event:/SX/Weapons/Sx_revolver_empty";
        
        switch (weapon_type)
        {
            case weapon.revolver: sound_path = "event:/SX/Weapons/Revolver/Sx_revolver_empty"; break;
            case weapon.shotgun: sound_path = "event:/SX/Weapons/Shotgun/Sx_shotgun_empty"; break;
            case weapon.chaingun: sound_path = "event:/SX/Weapons/Chaingun/Sx_chaingun_empty"; break;
            case weapon.grenadeLauncher: sound_path = "event:/SX/Weapons/Hand Mortar/Sx_handMortar_empty"; break;
            case weapon.lightningGun: sound_path = "event:/SX/Weapons/Railgun/Sx_railgun_empty"; break;
            case weapon.rocketLauncher: sound_path = "event:/SX/Weapons/Rocket Launcher/Sx_rocketlauncher_empty"; break;
            case weapon.rocketFist: sound_path =        "event:/SX/Weapons/Sx_revolver_empty"; break;
            case weapon.meleeFist: sound_path =         "event:/SX/Weapons/Sx_revolver_empty"; break;
            case weapon.meleeBlade: sound_path =        "event:/SX/Weapons/Sx_revolver_empty"; break;
        }

        FMODUnity.RuntimeManager.PlayOneShotAttached(sound_path, this.gameObject);
    }


    private void ShootSound(weapon weapon_type)
    {
        string sound_path = "event:/SX/Weapons/Sx_revolver_fire";

        switch (weapon_type)
        {
            case weapon.revolver: sound_path = "event:/SX/Weapons/Revolver/Sx_revolver_fire"; break;
            case weapon.shotgun: sound_path = "event:/SX/Weapons/Shotgun/Sx_shotgun_fire"; break;
            case weapon.chaingun: sound_path = "event:/SX/Weapons/Chaingun/Sx_chaingun_fire"; break;
            case weapon.grenadeLauncher: sound_path = "event:/SX/Weapons/Hand Mortar/Sx_handMortar_fire"; break;
            case weapon.lightningGun: sound_path = "event:/SX/Weapons/Railgun/Sx_railgun_fire"; break;
            case weapon.rocketLauncher: sound_path = "event:/SX/Weapons/Rocket Launcher/Sx_rocketlauncher_fire"; break;
            case weapon.rocketFist: sound_path = "event:/SX/Player/Small/Sx_Player_melee2"; break;
            case weapon.meleeFist: sound_path = "event:/SX/Player/Small/Sx_Player_melee2"; break;
            case weapon.meleeBlade: sound_path = "event:/SX/Player/Small/Sx_Player_melee2"; break;
        }

        FMODUnity.RuntimeManager.PlayOneShotAttached(sound_path, this.gameObject);

    }
}
