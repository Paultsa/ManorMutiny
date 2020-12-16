using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public float switchSpeed = 10;

    public SpriteAnimator dukeSprite;
    public SpriteAnimator assistantSprite;

    SpriteAnimator usedAnimator;

    public Animator SwayAnimator;
    public Animator JumpAnimator;
    AnimationClip[] clips;
    public GunController gunController;

    public float weaponOutLength;
    public float weaponInLength;
    public bool switchingWeapon = false;

    bool isGrounded = true;

    public float shootingExitTime;
    float timer;

    public PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        gunController = transform.GetChild(0).GetComponentInChildren<GunController>();
        clips = SwayAnimator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "weapon_out")
            {
                weaponOutLength = clip.length;
            }
            if (clip.name == "weapon_in")
            {
                weaponInLength = clip.length;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int playerIndex = 0;
        if (GameManager.gameManager && GameManager.gameManager.playersFound)
        {
            if (GameManager.gameManager && GameManager.gameManager.localPlayer.CompareTag("PlayerTwo"))
            {
                playerIndex = 1;
            }

            if (gunController.shot)
            {
                SwayAnimator.SetBool("shooting", true);
                timer = 0;
            }
            else if (timer < shootingExitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SwayAnimator.SetBool("shooting", false);
                timer = 0;
            }


            SwayAnimator.SetFloat("walkspeed", Mathf.Lerp(0, 1, Mathf.Clamp(new Vector3(playerMovement.playerRB.velocity.x, 0, playerMovement.playerRB.velocity.z).magnitude, 0, 1)));
            SwayAnimator.SetBool("is_grounded", playerMovement.grounded);


            if (playerMovement.playerRB.velocity.y > 0 && !playerMovement.grounded)
            {
                JumpAnimator.ResetTrigger("landing");
                JumpAnimator.SetBool("going_up", true);
                JumpAnimator.SetBool("going_down", false);
            }
            else if (playerMovement.playerRB.velocity.y < 0 && !playerMovement.grounded)
            {
                JumpAnimator.ResetTrigger("landing");
                JumpAnimator.SetBool("going_down", true);
                JumpAnimator.SetBool("going_up", false);
            }

            if (playerMovement.grounded)
            {
                if (!isGrounded)
                {
                    JumpAnimator.ResetTrigger("landing");
                    JumpAnimator.SetBool("going_down", false);
                    JumpAnimator.SetBool("going_up", false);
                    JumpAnimator.SetTrigger("landing");
                    isGrounded = true;
                }
            }
            else
            {
                isGrounded = false;
            }

            if (GameManager.gameManager)
            {
                switch (GameManager.gameManager.players[playerIndex].chosenWeapon)
                {
                    case 0:
                        WeaponSpecificAnim(GameManager.gameManager.players[playerIndex].primaryWeapon);
                        break;
                    case 1:
                        WeaponSpecificAnim(GameManager.gameManager.players[playerIndex].secondaryWeapon);
                        break;
                    case 2:
                        WeaponSpecificAnim(GameManager.gameManager.players[playerIndex].thirdWeapon);
                        break;
                }
            }
        }
    }


    public void WeaponSpecificAnim(weapon currentWeapon)
    {
        switch (currentWeapon)
        {
            case weapon.empty:
                SwayAnimator.SetBool("none_equipped", true);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.revolver:

                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", true);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.chaingun:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", true);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.grenadeLauncher:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", true);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.rocketLauncher:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", true);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.lightningGun:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", true);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.shotgun:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", true);
                SwayAnimator.SetBool("melee_equipped", false);
                break;

            case weapon.meleeFist:
                SwayAnimator.SetBool("none_equipped", false);
                SwayAnimator.SetBool("revolver_equipped", false);
                SwayAnimator.SetBool("chaingun_equipped", false);
                SwayAnimator.SetBool("grenadelauncher_equipped", false);
                SwayAnimator.SetBool("rocketlauncher_equipped", false);
                SwayAnimator.SetBool("lightninggun_equipped", false);
                SwayAnimator.SetBool("shotgun_equipped", false);
                SwayAnimator.SetBool("melee_equipped", true);
                break;
        }
    }

    public float SwitchWeapon()
    {
        switchingWeapon = true;
        SwayAnimator.ResetTrigger("weapon_switch");
        SwayAnimator.SetTrigger("weapon_switch");
        return weaponOutLength + weaponInLength;
    }

    public float SwitchNewWeapon()
    {
        switchingWeapon = true;
        //gunAnimator.ResetTrigger("hand_is_empty");
        //gunAnimator.SetTrigger("hand_is_empty");
        return weaponInLength;
    }

    public void UsingDukeSprite()
    {
        assistantSprite.gameObject.SetActive(false);
        dukeSprite.gameObject.SetActive(true);
        usedAnimator = dukeSprite;
    }
    public void UsingAssistantSprite()
    {
        dukeSprite.gameObject.SetActive(false);
        assistantSprite.gameObject.SetActive(true);
        usedAnimator = assistantSprite;
    }
}
