using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct TempTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public TempTransform(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
    
    public static bool operator==(TempTransform left, TempTransform right)
    {
        return left.position == right.position && left.rotation == right.rotation;
    }

    public static bool operator !=(TempTransform left, TempTransform right)
    {
        return left.position != right.position || left.rotation != right.rotation;
    }
}


public class NetworkPlayer : MonoBehaviour
{
    public bool isReady = false;

    // These are for forcing player to wait a little after starting multiplayer game,
    // so we can sync game start countdown more accurately
    double waitTimer = 0.0;
    const double maxWaitTimer = 3.0;

    bool firstFrame = true;
    
    // For identifying this player on the server side
    public byte ID { get; set; }
    // Is this our local player or not?
    public bool isLocalPlayer = false;

    private float timePoint_prevTransformUpdate = 0.0f;
    private float time_sinceLastTransformUpdate = 0.0f;

    // Current transform of the player on the server
    public TempTransform currentTransform;
    public TempTransform currentLocalTransform;

    // We need to store the previous transformation of the player (for non local player) to "extrapolate" movement..
    public TempTransform PreviousTransform { get; set; }

    public GameObject playerObj;
    public GameObject cameraObj;
    public SpriteDirController spriteDirController_duke;
    public SpriteDirController spriteDirController_assistant;
    public GunController gunController;

    public GameObject realLookDirObj;

    public void Start()
    {
    }

    void SetSpriteDirControllerProperties(Camera camToFollow)
    {
        spriteDirController_duke.camToFollow = camToFollow;
        spriteDirController_assistant.camToFollow = camToFollow;
    }

    public void StartLocal()
    {
        waitTimer = maxWaitTimer;
        isLocalPlayer = true;
        isReady = true;
        GameManager.gameManager.localPlayer = playerObj;

        SetSpriteDirControllerProperties(cameraObj.GetComponent<Camera>());

        // Set the other players non local
        foreach (NetworkPlayer np in GameObject.FindObjectsOfType<NetworkPlayer>())
        {
            if (np != this)
                np.StartNonLocal(this);
        }
        AttachLocalAnimComponentWebHELL();

        
        cameraObj.tag = "MainCamera";
    }

    public void StartNonLocal(NetworkPlayer localPlayer)
    {
        isLocalPlayer = false;
        cameraObj.tag = "SecondaryPlayerCamera";
        SetSpriteDirControllerProperties(localPlayer.cameraObj.GetComponent<Camera>());
        cameraObj.GetComponent<Camera>().enabled = false;
        cameraObj.GetComponent<FMODUnity.StudioListener>().enabled = false;
        playerObj.GetComponent<PlayerMovement>().enabled = false;
        playerObj.GetComponent<MouseLook>().enabled = false;


        playerObj.transform.GetChild(0).GetChild(0).GetComponent<SpriteAnimator>().enabled = false;
        playerObj.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

    }


    public void AttachLocalAnimComponentWebHELL()
    {
        GameObject animControllerObj = GameObject.Find("AnimController");
        if (animControllerObj != null)
        {
            AnimatorController animController = animControllerObj.GetComponent<AnimatorController>();
            if (animController != null)
            {
                Animator gunAnimator = gunController.gameObject.GetComponent<Animator>();
                Animator gunMaterialAnimator = gunController.transform.Find("GunMaterial").gameObject.GetComponent<Animator>();

                PlayerMovement playerMovement = playerObj.GetComponent<PlayerMovement>();
                GunAnimationPaths gunAnimPaths = GetComponent<GunAnimationPaths>();
                if (gunAnimator == null)
                {
                    Debug.Log("Tried to init local player's AnimController, but the Gun Animator was null!");
                    return;
                }
                if (playerMovement == null)
                {
                    Debug.Log("Tried to init local player's AnimController, but the PlayerMovement script was null!");
                    return;
                }
                animController.SwayAnimator = gunMaterialAnimator;
                animController.JumpAnimator = gunAnimator;
                animController.gunController = gunController;
                animController.playerMovement = playerMovement;

                gunAnimPaths.animController = animController;
                gunController.animController = animController;

                playerMovement.playerRB = playerObj.GetComponent<Rigidbody>();
            }
        }
    }

    public static byte CastWeaponNameToByte(weapon w)
    {
        switch (w)
        {
            case weapon.empty:              return 0;
            case weapon.revolver:           return 1;
            case weapon.chaingun:           return 2;
            case weapon.rocketLauncher:     return 3;
            case weapon.grenadeLauncher:    return 4;
            case weapon.shotgun:            return 5;
            case weapon.lightningGun:       return 6;
            case weapon.rocketFist:         return 7;
            case weapon.meleeFist:          return 8;
            case weapon.meleeBlade:         return 9;
            case weapon.total:              return 10;
            default:
                return 0;
        }
    }

    public static weapon CastByteToWeaponName(byte w)
    {
        switch (w)
        {
            case 0: return weapon.empty;
            case 1: return weapon.revolver;
            case 2: return weapon.chaingun;
            case 3: return weapon.rocketLauncher;
            case 4: return weapon.grenadeLauncher;
            case 5: return weapon.shotgun;
            case 6: return weapon.lightningGun;
            case 7: return weapon.rocketFist;
            case 8: return weapon.meleeFist;
            case 9: return weapon.meleeBlade;
            case 10: return weapon.total;
            default:
                return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Client client = Client.Instance;

        if (isLocalPlayer)
        {
            Package transformPackage = new Package(
                client.RoomId,
                client.Id, 
                NetworkingCommon.MESSAGE_TYPE__StorePlayerTransform, 
                playerObj.transform.localPosition,
                cameraObj.transform.rotation,
                playerObj.transform.lossyScale
            );

            if (MultiplayerSceneProperties.Instance.updatedOnce && waitTimer <= 0.0)
                client.SendData(transformPackage.Buffer.Bytes);
            else
                waitTimer -= Time.deltaTime;
        }
        else
        {
            // "extrapolate movement"
            if (time_sinceLastTransformUpdate <= 0.0f)
                time_sinceLastTransformUpdate = 0.0001f;
            
            Vector3 velocity = (currentTransform.position - PreviousTransform.position) / time_sinceLastTransformUpdate;
            currentLocalTransform.position += velocity * Time.deltaTime;

            Quaternion finalRot = currentTransform.rotation;
            cameraObj.transform.rotation = finalRot;

            playerObj.transform.localPosition = currentLocalTransform.position;
            //playerObj.transform.rotation = finalRot;

            // Update sprite dir controller's direction
        }
    }

    public void SetTransform(Vector3 pos, Quaternion rot)
    {
        isReady = true;
        
        PreviousTransform = currentTransform;
        currentTransform = new TempTransform(pos, rot);
        
        currentLocalTransform = currentTransform;

        time_sinceLastTransformUpdate = Time.time - timePoint_prevTransformUpdate;
        timePoint_prevTransformUpdate = Time.time;
        // No scales at the moment..
    }
}
