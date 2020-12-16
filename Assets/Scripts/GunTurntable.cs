using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Pauli
public class GunTurntable : MonoBehaviour
{
    public weapon weaponType;
    public int[] weapon_ammo_amount;
    public GunController gunCon;
    
    //KostinKoodi
    public WeaponSpawner weaponSpawnerScript;

    void Start()
    {
        weapon_ammo_amount = new int[(int)weapon.total];
        weapon_ammo_amount[(int)weapon.empty] = -1;
        weapon_ammo_amount[(int)weapon.revolver]        = 25;
        weapon_ammo_amount[(int)weapon.chaingun]        = 75;
        weapon_ammo_amount[(int)weapon.rocketLauncher]  = 8;
        weapon_ammo_amount[(int)weapon.grenadeLauncher] = 15;
        weapon_ammo_amount[(int)weapon.shotgun]         = 20;
        weapon_ammo_amount[(int)weapon.lightningGun]    = 8;
        weapon_ammo_amount[(int)weapon.rocketFist]      = 10;
        weapon_ammo_amount[(int)weapon.meleeFist] = int.MaxValue;
        weapon_ammo_amount[(int)weapon.meleeBlade] = int.MaxValue;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerOne") || other.CompareTag("PlayerTwo"))
        {
            gunCon = other.transform.GetChild(0).GetChild(0).GetComponent<GunController>();
            weaponSpawnerScript = transform.parent.GetComponent<WeaponSpawner>();

            if (gunCon.weapons[gunCon.current_weapon] == weapon.empty && gunCon.cooldown <= 0 && gunCon.shot == false)
            {
                //Player picks up a weapon of type weaponType
                gunCon.ammos[gunCon.current_weapon] = weapon_ammo_amount[(int)weaponType];
                gunCon.weapons[gunCon.current_weapon] = weaponType;
                other.transform.parent.GetComponent<GunAnimationPaths>().currentWeapon = weaponType;
                PickupSound(weaponType);
                

                //KostinKoodi
                weaponSpawnerScript.SpawnerSound.setParameterByName("Spawned", 0);
               
                Destroy(this.gameObject); 
               
            }
            else
            {
                for (var i = 0; i < gunCon.inventory_size; i++)
                {
                    if ((gunCon.weapons[i] == weapon.empty || gunCon.weapons[i] == weapon.meleeFist) || gunCon.ammos[i] <= 0)
                    {
                        if (gunCon.current_weapon == gunCon.inventory_size)
                        {
                            gunCon.current_weapon = i;
                            gunCon.ammos[gunCon.current_weapon] = weapon_ammo_amount[(int)weaponType];
                            gunCon.weapons[gunCon.current_weapon] = weaponType;
                            other.transform.parent.GetComponent<GunAnimationPaths>().currentWeapon = weaponType;
                        }
                        else
                        {
                            gunCon.ammos[i] = weapon_ammo_amount[(int)weaponType];
                            gunCon.weapons[i] = weaponType;
                        }
                        //other.transform.parent.GetComponent<GunAnimationPaths>().currentWeapon = weaponType;
                        PickupSound(weaponType);
                       
                        //KostinKoodi
                        weaponSpawnerScript.SpawnerSound.setParameterByName("Spawned", 0);

                        Destroy(this.gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void PickupSound(weapon weapon_type)
    {
        string sound_path = "event:/SX/Weapons/Sx_revolver_pickup";

        switch (weapon_type)
        {
            case weapon.revolver: sound_path = "event:/SX/Weapons/Revolver/Sx_revolver_pickup"; break;
            case weapon.shotgun: sound_path = "event:/SX/Weapons/Shotgun/Sx_shotgun_pickup"; break;
            case weapon.chaingun: sound_path = "event:/SX/Weapons/Chaingun/Sx_chaingun_pickup"; break;
            case weapon.grenadeLauncher: sound_path = "event:/SX/Weapons/Hand Mortar/Sx_handMortar_pickup"; break;
            case weapon.lightningGun: sound_path = "event:/SX/Weapons/Railgun/Sx_railgun_pickup"; break;
            case weapon.rocketLauncher: sound_path = "event:/SX/Weapons/Rocket Launcher/Sx_rocketlauncher_pickup"; break;
            case weapon.rocketFist: sound_path =        "event:/SX/Weapons/Sx_revolver_pickup"; break;
            case weapon.meleeFist: sound_path =         "event:/SX/Weapons/Sx_revolver_pickup"; break;
            case weapon.meleeBlade: sound_path =        "event:/SX/Weapons/Sx_revolver_pickup"; break;
        }

        FMODUnity.RuntimeManager.PlayOneShot(sound_path, this.transform.position);
    }
}
