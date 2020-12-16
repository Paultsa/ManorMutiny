using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy
//Kostin koodimuistiinpanot: Hook launch, hook fly, hook hit, hook reel in, parameters for these

public class GrapplingHook : MonoBehaviour
{
    public GameObject explosionObj;
    public LineRenderer grapplingRope;
    public Transform grapplingRopePlayerPoint;
    [Header("Grappling hook settings")]
    [Tooltip("The hook's travel speed")]
    public float speed = 10;
    [Tooltip("How fast the hook pulls the player")]
    public float reelingSpeed = 10;
    [Tooltip("The hook's duration. this sets how long and how far the hook travels with the speed variable")]
    public float duration = 3f;
    [Tooltip("Once the hook hits something it pulls the player for this duration")]
    public float pullDuration = 3f;
    [Tooltip("The hook's cooldown")]
    public float cooldown = 3;
    [Tooltip("Once the player and grappling hook are within this distance from each other the hook disconnects")]
    public float detachDistance = 2f;
    [Tooltip("How many times can the grappling hook be shot consecutively")]
    public int hookCharges = 3;
    [Tooltip("If the hook uses gravity -> flies in an arc")]
    public bool useGravity = false;
    public LayerMask rangeMask;

    [HideInInspector]
    public bool hookFlying = false;
    [HideInInspector]
    public bool hookHit = false;
    [HideInInspector]
    public bool arrived = false;
    //[HideInInspector]
    public bool inHookRange;
    [HideInInspector]
    public float durationTimer = 0f;
    [HideInInspector]
    public static float cooldownTimer = 0f;
    [HideInInspector]
    public static float maxCooldown;
    Vector3 travelDirection;

    public GameObject hookPrefab;
    GameObject grapplingHookContainer;
    public GameObject newGrapplingHook;
    Rigidbody playerRB;
    Vector3[] ropeAnchors;
    float doubleClickInputBuffer = 0.2f;

    //KostinKoodi
    [FMODUnity.EventRef]
    public string GrapplingHookSoundPath = "";
    FMOD.Studio.EventInstance GrapplingHookSound;
    void Start()
    {
        maxCooldown = cooldown * hookCharges;
        cooldownTimer = maxCooldown;
        grapplingHookContainer = new GameObject();
        grapplingHookContainer.name = "GrapplingHookContainer";
        grapplingHookContainer.transform.position = new Vector3(0, 0, 0);
        playerRB = transform.GetComponentInParent<Rigidbody>();
        ropeAnchors = new[] { grapplingRopePlayerPoint.position, grapplingRopePlayerPoint.position };

        UIManager.grapplingHook = true;
        UIManager.grapplingHookCharges = hookCharges;
        UIManager.grapplingHookMaxCooldown = maxCooldown;
        UIManager.grapplingHookCooldown = cooldown;
        

    }

    // Update is called once per frame
    RaycastHit push;
    RaycastHit grapplingRange;
    public float buffer = 0.03f;
    void Update()
    {
        UIManager.grapplingHookTimer = cooldownTimer;
        UIManager.inGrapplingRange = inHookRange;

        //Enemy ai push test
        /*if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, transform.forward, out push, 1000))
            {
                Debug.DrawRay(transform.position, transform.forward * push.distance, Color.blue, 5);
                Explosion.ObstructedExplosionDamage(push.point, 10, 5, 30, 1);
                Instantiate(explosionObj, push.point+transform.position.normalized*2, Quaternion.identity);
                //Debug.Log(push.collider.gameObject);
                //if (push.transform.GetComponent<EnemyAi>())
                //{
                    //push.transform.GetComponent<EnemyAi>().StunEnemy(1, transform.position, 20);
                    //push.transform.GetComponent<IHealth>().TakeDamage(5);
               //}

            }
        }*/

        if (Physics.Raycast(transform.position, transform.forward, out grapplingRange, (speed * duration) + buffer, rangeMask))
        {
            //Debug.Log("IN HOOK RANGE");
            inHookRange = true;
        }
        else
        {
            inHookRange = false;
        }


        if (hookFlying || hookHit)
        {
            durationTimer += Time.deltaTime;
            if (Input.GetButtonDown("Hook") && durationTimer > doubleClickInputBuffer)
            {
                durationTimer = 0;
                Destroy(newGrapplingHook);

                //KostinKoodi
                GrapplingHookSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                
                //grapplingRope.enabled = false;
            }

        }
        if (hookFlying && durationTimer > duration || hookHit && durationTimer > pullDuration)
        {
            durationTimer = 0;
            Destroy(newGrapplingHook);

            //KostinKoodi
            GrapplingHookSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
           
            //grapplingRope.enabled = false;
        }
        if (hookHit)
        {
            playerRB.gameObject.GetComponent<PlayerMovement>().grappling = true;
            playerRB.gameObject.GetComponent<PlayerMovement>().dodging = false;
            playerRB.gameObject.GetComponent<PlayerMovement>().specialAbilityDisables = true;
            playerRB.velocity = (newGrapplingHook.transform.position - transform.position).normalized * reelingSpeed;

            //KostinKoodi  
            GrapplingHookSound.setParameterByName("GraplingHook Hit", 1);

            if ((newGrapplingHook.transform.position - transform.position).magnitude < detachDistance)
            {
                durationTimer = 0;
                Destroy(newGrapplingHook);

                //KostinKoodi
                GrapplingHookSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
               
                //grapplingRope.enabled = false;
            }
        }

        if (newGrapplingHook)
        {
            ropeAnchors[1] = newGrapplingHook.transform.position;
            ropeAnchors[0] = grapplingRopePlayerPoint.position;
            grapplingRope.SetPositions(ropeAnchors);
        }


        if (cooldownTimer < maxCooldown)
        {
            cooldownTimer += Time.deltaTime;
            cooldownTimer = Mathf.Clamp(cooldownTimer, 0, maxCooldown);
        }

        if (!hookFlying && Input.GetButtonDown("Hook") && !newGrapplingHook && cooldownTimer > maxCooldown / hookCharges)
        {
            hookFlying = true;
            travelDirection = transform.forward;
            cooldownTimer -= cooldown;
            newGrapplingHook = Instantiate(hookPrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
            newGrapplingHook.GetComponent<GrapplingHookCollision>().shooter = this.transform;
            newGrapplingHook.transform.parent = grapplingHookContainer.transform;
            if (useGravity)
                newGrapplingHook.GetComponent<Rigidbody>().useGravity = true;
            newGrapplingHook.transform.forward = travelDirection;
            //newGrapplingHook.GetComponent<GrapplingHookCollision>().pullDuration = pullDuration;
            newGrapplingHook.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
            newGrapplingHook.GetComponent<Rigidbody>().AddForce(travelDirection * speed, ForceMode.Impulse);
            grapplingRope.enabled = true;

            //KostinKoodi
            GrapplingHookSound = FMODUnity.RuntimeManager.CreateInstance(GrapplingHookSoundPath);
            GrapplingHookSound.start();
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(GrapplingHookSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        }
    }

    private void OnDisable()
    {
        if (newGrapplingHook)
        {
            Destroy(newGrapplingHook);
        }
    }

}
