using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Kostin koodimuistiinpanot: launch, hover, land
public class RocketPack : MonoBehaviour
{

    [Header("Rocket pack settings")]
    [Tooltip("How much force is added to kick the player off the ground")]
    public float initialThrust = 20;
    [Tooltip("How much lift does the pack give the player")]
    public float thrust = 20;
    [Tooltip("How long after initial burst will the hovering start")]
    public float thrustDuration = 1;
    [Tooltip("How long will the player hover")]
    public float hoverDuration = 2;
    [Tooltip("How fast will the player barrel towards the aimed point")]
    public float crashSpeed = 40;
    [Tooltip("How fast the player accelerator from hover when initiationg crash")]
    public float crashAcceleration = 15;
    [Tooltip("Small delay added to the start of the crash to add a feeling of more weight")]
    public float crashDelay = 0.3f;
    [Tooltip("How close to the crash point will the player trigger the crash explosion")]
    public float explosionProximity = 1;
    [Tooltip("The size of the explosion caused by the crash")]
    public float explosionRange = 5;
    [Tooltip("How much damage does the explosion do")]
    public int explosionDamage = 10;
    [Tooltip("How much knockback does the explosion have")]
    public int explosionKnockback = 10;
    [Tooltip("How long does the explosion stuns the enemy for")]
    public int explosionStunDuration = 10;
    [Tooltip("How long will it take for the rocket pack to be usable again after every use")]
    public float cooldown = 5;
    [Tooltip("Highest angle the player can perform the crash. This has to be below 90")]
    public float maxCrashAngle = 60;
    [Tooltip("If the player will face the point he is crashing toward")]
    public bool turnWhenCrashing;

    public LayerMask mask;

    public GameObject aoeIndicator;

    [HideInInspector]
    public PlayerMovement playerMovement;
    Rigidbody playerRB;
    float cooldownTimer;

    bool active;
    bool rocket;
    bool decelerate;
    bool hover;
    [HideInInspector]
    public bool crashing;
    bool accelerated;
    float thrustTimer;
    float hoverTimer;
    float crashDelayTimer;
    float upDownAngle;
    float crashAcceleratingSpeed;

    Vector3 crashPoint;
    Vector3 crashDirection;
    Vector3 crashLineOfSight;

    RaycastHit lineOfSight;

    //KostinKoodi
    FMOD.Studio.EventInstance jumpPackSound;

    
    // Start is called before the first frame update
    void Start()
    {

        SetGlobalScale(aoeIndicator.transform, new Vector3 (explosionRange * 2, explosionRange * 2, explosionRange * 2));
        cooldownTimer = cooldown;
        maxCrashAngle = 90 - maxCrashAngle;
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
        playerRB = transform.parent.GetComponent<Rigidbody>();

        UIManager.rocketPack = true;
        UIManager.rocketjumpCooldown = cooldown;
    }

    void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    // Update is called once per frame
    void Update()
    {

        UIManager.rocketjumpTimer = cooldownTimer;

        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
            cooldownTimer = Mathf.Clamp(cooldownTimer, 0, cooldown);
            return;
        }

        if (Input.GetButtonDown("Hook") && !active)
        {
            //KostinKoodi
            jumpPackSound = FMODUnity.RuntimeManager.CreateInstance("event:/SX/Player/Big/PlayerBig_jetpack_Jump");
            
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(jumpPackSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
            jumpPackSound.start();

            playerMovement.grappling = true;
            playerMovement.specialAbilityDisables = true;
            playerMovement.dodging = false;
            playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
            playerRB.AddForce(Vector3.up * initialThrust, ForceMode.Impulse);
            rocket = true;
            active = true;
            
        }

        if (rocket)
        {
            //Debug.Log("THRUST");
            playerRB.AddForce(Vector3.up * thrust, ForceMode.Acceleration);
            thrustTimer += Time.deltaTime;
            

            if (thrustTimer >= thrustDuration)
            {
                rocket = false;
                decelerate = true;
                thrustTimer = 0;
                //playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
            }
        }

        if (decelerate)
        {
            //Debug.Log("DECELERATE");
            playerRB.velocity = new Vector3(playerRB.velocity.x, Mathf.Lerp(playerRB.velocity.y, -0.5f, Time.deltaTime * 5), playerRB.velocity.z);
            if (playerRB.velocity.y <= 0)
            {
                //playerMovement.grappling = false;
                playerMovement.specialAbilityDisables = false;
                hover = true;
                decelerate = false;
            }
        }

        if (hover)
        {
            //Debug.Log("HOVER");
            playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
            hoverTimer += Time.deltaTime;
            if (hoverTimer >= hoverDuration)
            {
                hover = false;
                hoverTimer = 0;
                active = false;
                cooldownTimer = 0;
                aoeIndicator.SetActive(false);
                playerMovement.grappling = false;

                //KostinKoodi
                jumpPackSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        if (hover)
        {
            //This creates a directional vector using angles which can be clamped. This makes it so that the raycast can be shot out using angles
            upDownAngle = transform.localRotation.eulerAngles.x;
            upDownAngle = CrashAngleClamp(upDownAngle, maxCrashAngle, 90);
            //Debug.Log(upDownAngle);
            crashLineOfSight = Quaternion.Euler(upDownAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) * Vector3.forward;
            //Debug.DrawRay(transform.position, crashLineOfSight * 10, Color.red);
            if (Physics.Raycast(transform.position, crashLineOfSight, out lineOfSight, 1000, mask) && !crashing)
            {
                aoeIndicator.SetActive(true);
                aoeIndicator.transform.position = lineOfSight.point;
                if (Input.GetButtonDown("Hook"))
                {
                    //Debug.Log("CRASH");
                    transform.parent.GetComponent<Collider>().enabled = false;
                    crashing = true;
                    crashPoint = aoeIndicator.transform.position;
                    crashDirection = crashLineOfSight;
                    hoverTimer = 0;
                    thrustTimer = 0;
                    crashDelayTimer = crashDelay;
                    aoeIndicator.SetActive(false);
                }
            }
            else
            {
                aoeIndicator.SetActive(false);
            }
        }

        if (crashDelayTimer > 0)
            crashDelayTimer -= Time.deltaTime;

        if (crashing && crashDelayTimer <= 0)
        {
            hover = false;
            if (playerRB.velocity.magnitude < crashSpeed - 1 && !accelerated)
            {
                accelerated = false;
                //Debug.Log("ACC" + playerRB.velocity.magnitude);
                crashAcceleratingSpeed = Mathf.Lerp(crashAcceleratingSpeed, crashSpeed + crashAcceleratingSpeed * 2, Time.deltaTime * crashAcceleration);
            }
            else
            {
                accelerated = true;
                //Debug.Log("DONE");
                crashAcceleratingSpeed = crashSpeed;
            }
            playerRB.velocity = crashDirection.normalized * crashAcceleratingSpeed;
            GetComponent<MouseLook>().enabled = false;
            transform.parent.GetComponent<MouseLook>().enabled = false;
            if (turnWhenCrashing)
            {
                transform.forward = crashDirection;
                GetComponent<MouseLook>().originalRotation = transform.localRotation;
                GetComponent<MouseLook>().rotationY = 0;
            }
            if ((transform.position - crashPoint).magnitude <= explosionProximity)
            {
                //Debug.Log("BOOOOOOOOOOOOOOM");
                Explosion.UnobstructedExplosionDamage(transform.parent.position, explosionRange, explosionDamage, explosionKnockback, explosionStunDuration, false);
                transform.parent.GetComponent<Collider>().enabled = true;
                playerRB.velocity /= 1.4f;
                crashing = false;
                active = false;
                GetComponent<MouseLook>().enabled = true;
                transform.parent.GetComponent<MouseLook>().enabled = true;
                cooldownTimer = 0;
                playerMovement.specialAbilityDisables = false;
                playerMovement.grappling = false;

                //KostinKoodi
                jumpPackSound.setParameterByName("JumpPack_Land", 1);
            }
        }
    }
    bool colliderReactivated;
    private void OnCollisionStay(Collision collision)
    {
        
    }

    float CrashAngleClamp(float value, float min, float max)
    {
        if (value > 260) //real value is 270, 10 removed just in case
            return min;
        if (value >= max)
            return max;
        if (value <= min)
            return min;
        return value;
    }
}
