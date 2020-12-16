using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;


//Pauli
public enum weapon
{
    empty,
    revolver,
    chaingun,
    rocketLauncher,
    grenadeLauncher,
    shotgun,
    lightningGun,
    rocketFist,
    meleeFist,
    meleeBlade,
    total
};

public class GunAnimationPaths : MonoBehaviour
{

    [Serializable]
    public class Gun
    {
        public weapon name;
        //public float idleTimeBetweenAnim = 0.1f;
        public float shootTimeBetweenAnim = 0.1f;
        public List<UnityEngine.Object> idleTexture;
        public List<UnityEngine.Object> idleNormal;
        public List<UnityEngine.Object> idleEmission;
        public List<UnityEngine.Object> idleMetallic;
        public List<UnityEngine.Object> idleOcclusion;

        public List<UnityEngine.Object> shootTexture;
        public List<UnityEngine.Object> shootNormal;
        public List<UnityEngine.Object> shootEmission;
        public List<UnityEngine.Object> shootMetallic;
        public List<UnityEngine.Object> shootOcclusion;
    }
    public GameObject gun;
    public weapon currentWeapon;
    public weapon lastWeapon;
    public Gun[] guns;
    public Dictionary<weapon, Gun> gunsDict = new Dictionary<weapon, Gun>();
    SpriteAnimator spriteAnim;

    public AnimatorController animController;
    float gunSwapCooldown;
    float gunSwapTimer;
    float secondaryCooldown;
    float secondaryTimer;
    bool switchedOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        lastWeapon = currentWeapon;
        spriteAnim = gun.GetComponent<SpriteAnimator>();
        animController = GetComponentInChildren<AnimatorController>();

        foreach (Gun g in guns)
        {
            Debug.Log(g.name);
            gunsDict.Add(g.name, g);
        }

    }

    // Update is called once per frame
    void Update()
    {



        if (currentWeapon != lastWeapon)
        {
            if(lastWeapon == weapon.empty)
            {
                gunSwapCooldown = animController.SwitchNewWeapon();
            }
            else
            {
                gunSwapCooldown = animController.SwitchWeapon();
            }
            
            secondaryCooldown = gunSwapCooldown;
            secondaryTimer = 0;
            gunSwapTimer = 0;
            lastWeapon = currentWeapon;
        }
        if(gunSwapCooldown != 0)
        {
            if (gunSwapTimer < gunSwapCooldown/2)
            {
                gunSwapTimer += Time.deltaTime;
            }
            else
            {
                spriteAnim.pauseCounter = true;
                //spriteAnim.idleTimeBetweenAnimFrames = gunsDict[currentWeapon].idleTimeBetweenAnim;
                spriteAnim.shootTimeBetweenAnimFrames = gunsDict[currentWeapon].shootTimeBetweenAnim;
                spriteAnim.idleTextures = gunsDict[currentWeapon].idleTexture;
                spriteAnim.idleNormals = gunsDict[currentWeapon].idleNormal;
                spriteAnim.idleEmissions = gunsDict[currentWeapon].idleEmission;
                spriteAnim.idleMetallics = gunsDict[currentWeapon].idleMetallic;
                spriteAnim.idleOcclusions = gunsDict[currentWeapon].idleOcclusion;
                spriteAnim.shootTextures = gunsDict[currentWeapon].shootTexture;
                spriteAnim.shootNormals = gunsDict[currentWeapon].shootNormal;
                spriteAnim.shootEmissions = gunsDict[currentWeapon].shootEmission;
                spriteAnim.shootMetallics = gunsDict[currentWeapon].shootMetallic;
                spriteAnim.shootOcclusions = gunsDict[currentWeapon].shootOcclusion;
                spriteAnim.LoadTextures();
                gunSwapCooldown = 0;
                gunSwapTimer = 0;
            }
        }
        if (secondaryCooldown != 0)
        {
            if(secondaryTimer < secondaryCooldown)
            {
                secondaryTimer += Time.deltaTime;
            }
            else
            {
                animController.switchingWeapon = false;
                secondaryTimer = 0;
                secondaryCooldown = 0;
            }
        }
    }
}
