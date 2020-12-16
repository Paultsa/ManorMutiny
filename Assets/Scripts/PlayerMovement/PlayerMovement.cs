using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy
//Kostin koodimuistiinpanot: Movement needs check for graplinghook, Big/Small???
public class PlayerMovement : MonoBehaviour
{
    //Initilization
    [HideInInspector]
    public Rigidbody playerRB;
    [HideInInspector]
    public Camera cam;

    GameObject dashParticleObj;
    //Movement values
    [Header("Movement settings")]
    [Tooltip("Max movement speed of the player")]
    public float maxSpeed = 1;
    [Tooltip("How fast the player can move universally. for example by being pushed by an explosion what is the maximum velocity the player can move")]
    public float maxUniversalSpeed = 100;
    [Tooltip("How fast player accelerates")]
    public float acceleration = 1;
    [Tooltip("How fast player decelerates")]
    public float deceleration = 1;
    [Tooltip("How much control the player retains when in the air. 0 = none 1 = full")]
    public float airControlMultiplier = 0.5f;
    [Tooltip("How much deceleration is affected by being in the air. 0 = no deceleration 1 = normal deceleration")]
    public float airDecelerationMultiplier = 0.1f;
    [Tooltip("Layers which player movement uses")]
    public LayerMask PlayerMovemenMask;
    //KostinKoodi
    public float walkingSoundSpeed;




    [Header("Jump settings")]
    [Tooltip("How much initial force the players jump gets")]
    public float jumpForce = 30f;
    [Tooltip("How much force gets applied toward the direction the player is moving. This effect creates bunnyhop")]
    public float hopForce = 10f;
    [Tooltip("How much holding jump button gives extra height")]
    public float jumpHoldFloat = 0.3f;
    [Tooltip("How floaty the player will be at the peak of the jump. Higher = floatier")]
    public float floatFactor = 0.8f;
    [Tooltip("How much faster will the player fall compared to normal gravity")]
    public float fallMultiplier = 1.8f;
    [Tooltip("How close to the peak of the jump for the float to kick in")]
    public float peakStartVelocity = 0.5f;
    [Tooltip("When the floating stops and the player starts to fall faster")]
    public float peakEndVelocity = 0.2f;
    [Tooltip("Time window after the player leaves the ground and can perform a jump")]
    public float coyoteTime = 0.3f;
    [Tooltip("Small window set between jumps so player cant spam jump and get a double acceleration")]
    public float jumpCooldown = 0.3f;
    [Tooltip("Small window when player is in the air and jump input is saved so the player instantly jumps when he hits the ground. This is to make bunnyhopping easier")]
    public float jumpInputBuffer = 0.1f;

    [Header("Ledge grab settings")]
    [Tooltip("How fast the player climbs up a ledge")]
    public float climbSpeed = 2f;
    [Tooltip("Does the player turn toward the ledge they are climbing")]
    public bool turnWhenClimbing = false;

    [Header("Dodge/Air dodge settings")]
    [Tooltip("How long is the dodge")]
    public float dodgeDuration = 0.5f;
    [Tooltip("How fast is the dodge")]
    public float dodgeSpeed = 30f;
    [Tooltip("How fast is the dodge")]
    public float dodgeExitSpeed = 20f;
    [Tooltip("Duration between dashes")]
    public float dodgeCooldown = 1f;
    [Tooltip("Can the player dodge in the air")]
    public bool airDodge = false;

    [HideInInspector]
    public bool grounded;
    //KostinKoodi
    [HideInInspector]
    public bool walkingOnBlood;

    [HideInInspector]
    public bool movingWithInput;
    [HideInInspector]
    public bool frontGrabColliding;
    [HideInInspector]
    public bool topGrabColliding;
    [HideInInspector]
    public bool grappling;
    [HideInInspector]
    public bool dodging = false;
    [HideInInspector]
    public bool specialAbilityDisables;

    bool grabbing = false;
    float diagonalClamp = 1;
    float diagonalDeceleration = 1;
    float airControl;
    float airDeceleration;
    Vector3 localVelocity;
    Collider grabbableLedge;
    [HideInInspector]
    public bool ledgeGrabBufferFrame = false;

    bool smallDoc;
    bool bigAssistant;
    // Start is called before the first frame update

    public Color32 GetPixelColor(RaycastHit hit)
    {
        Color32 pixelColor = new Color32(0,0,0,0);
        Renderer rend = hit.transform.GetComponent<Renderer>();
        if(rend != null)
        {
            Texture2D groundTex = (Texture2D)hit.transform.GetComponent<Renderer>().material.GetTexture("_BaseMap");

            if(groundTex != null && groundTex.isReadable)
            {
                pixelColor = (Color32)groundTex.GetPixel((int)(hit.textureCoord.x * groundTex.width), (int)(hit.textureCoord.y * groundTex.height));
                //Debug.Log(pixelColor);
                return pixelColor;
            }
        }
        //Debug.Log("Couldnt get texture pixel");
        return pixelColor;
    }

    void Start()
    {
        dashParticleObj = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        GameObject expTarget = new GameObject("ExpTarget");
        expTarget.transform.parent = gameObject.transform;
        expTarget.transform.SetAsLastSibling();
        expTarget.transform.localPosition = new Vector3(0, 0, 0);
        expTarget.layer = LayerMask.NameToLayer("ExpTarget");
        expTarget.AddComponent<SphereCollider>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        playerRB = transform.GetComponent<Rigidbody>();
        airControl = airControlMultiplier;
        airDeceleration = airDecelerationMultiplier;

        smallDoc = false;
        bigAssistant = false;

        if (gameObject.CompareTag("PlayerOne"))
            smallDoc = true;
        else if (gameObject.CompareTag("PlayerTwo"))
            bigAssistant = true;

        //KostinKoodi
        InvokeRepeating("MovementSound", 0, walkingSoundSpeed);
    }

    void GroundColorCheck()
    {
        //CHECK THE COLOW OF THE GROUND THE PLAYER IS WALKING ON HERE. MAYBE EVEN THE AUDIO I DONT KNOW IM NOT YOUR BOSS
    }

    // Update is called once per frame
    RaycastHit GroundCheck;
    void Update()
    {
        if(dodging)
        {
            dashParticleObj.SetActive(true);
        }
        else
        {
            dashParticleObj.SetActive(false);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out GroundCheck, transform.lossyScale.y + 0.3f, PlayerMovemenMask))
        {
            grounded = true;
            GroundColorCheck();
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * GroundCheck.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (transform.lossyScale.y + 0.3f), Color.red);
            grounded = false;
        }

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            movingWithInput = true;
        else
            movingWithInput = false;


        if (!grabbing)
        {
            if (!dodging)
            {
                Jump();
                Movement();
            }
            if (!specialAbilityDisables)
                Dodge();
        }
        if (!specialAbilityDisables)
            LedgeGrab();


        if (!ledgeGrabBufferFrame)
        {
            frontGrabColliding = false;
            grabbableLedge = null;
        }
    }


    void Movement()
    {

        if (grounded)
        {
            airControl = 1;
            airDeceleration = 1;
        }
        else
        {
            airControl = airControlMultiplier;
            airDeceleration = airDecelerationMultiplier;
        }

        localVelocity = transform.InverseTransformDirection(playerRB.velocity);
        //Back and forth movement
        if (Input.GetAxisRaw("Vertical") == 1 && localVelocity.z < maxSpeed * diagonalClamp)
        {
            localVelocity.z += Input.GetAxisRaw("Vertical") * acceleration * Time.deltaTime * airControl;
            localVelocity.z = Mathf.Clamp(localVelocity.z, -maxUniversalSpeed, maxSpeed * diagonalClamp);    
        }
        else if (Input.GetAxisRaw("Vertical") == -1 && localVelocity.z > -maxSpeed * diagonalClamp)
        {
            localVelocity.z += Input.GetAxisRaw("Vertical") * acceleration * Time.deltaTime * airControl;
            localVelocity.z = Mathf.Clamp(localVelocity.z, -maxSpeed * diagonalClamp, maxUniversalSpeed);
        }
        else if (localVelocity.z < -0.5 || localVelocity.z > 0.5)   //Deceleration for back and forth movement
        {
            if (localVelocity.z < -0.5)
                localVelocity.z += 1 * deceleration * Time.deltaTime * airDeceleration;
            else if (localVelocity.z > 0.5)
                localVelocity.z += -1 * deceleration * Time.deltaTime * airDeceleration;
            localVelocity.z = Mathf.Clamp(localVelocity.z, -maxUniversalSpeed, maxUniversalSpeed);
        }
        else if (localVelocity.z > -0.5 && localVelocity.z < 0.5)   //Stops the player once the speed is low enough
        {
            localVelocity.z = 0;
        }

        //Side to side movement
        if (Input.GetAxisRaw("Horizontal") == 1 && localVelocity.x < maxSpeed * diagonalClamp)
        {
            localVelocity.x += Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime * airControl;
            localVelocity.x = Mathf.Clamp(localVelocity.x, -maxUniversalSpeed, maxSpeed * diagonalClamp);
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 && localVelocity.x > -maxSpeed * diagonalClamp)
        {
            localVelocity.x += Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime * airControl;
            localVelocity.x = Mathf.Clamp(localVelocity.x, -maxSpeed * diagonalClamp, maxUniversalSpeed);
        }
        else if (localVelocity.x < -0.5 || localVelocity.x > 0.5)   //Deceleration for side to side movement
        {
            if (localVelocity.x < -0.5)
                localVelocity.x += 1 * deceleration * Time.deltaTime * airDeceleration;
            else if (localVelocity.x > 0.5)
                localVelocity.x += -1 * deceleration * Time.deltaTime * airDeceleration;
            localVelocity.x = Mathf.Clamp(localVelocity.x, -maxUniversalSpeed, maxUniversalSpeed);
        }
        else if (localVelocity.x > -0.5 && localVelocity.x < 0.5)   //Stops the player once the speed is low enough
        {
            localVelocity.x = 0;
        }

        //Checks if the player is moving diagonally and normalizes the speed
        if (Input.GetButton("Horizontal") && Input.GetButton("Vertical"))
        {
            diagonalClamp -= Time.deltaTime * 1;
            diagonalClamp = Mathf.Clamp(diagonalClamp, 0.707f, 1);
        }
        else
        {
            diagonalClamp += Time.deltaTime * diagonalDeceleration;
            diagonalClamp = Mathf.Clamp(diagonalClamp, 0.707f, 1);
        }
        playerRB.velocity = transform.TransformDirection(localVelocity);


        //Debug.Log(Vector3.Dot(playerRB.velocity, transform.right) + " : " + Vector3.Dot(playerRB.velocity, transform.forward));


    }
    float coyoteTimeTimer = 0f;
    bool jumpReleased = false;
    float jumpCooldownTimer = 0;
    float jumpInputBufferTimer = 0f;
    float diagonalJump = 1;
    Vector3 jumpDir;
    void Jump()
    {

        //Holding jump down gives the player some reverse gravity making the jump float a bit higher. All this is before the cooldown because it needs to work immediately after jumping, so the cooldown cant affect it
        if (Input.GetButton("Jump") && !jumpReleased)
        {
            //playerRB.velocity -= Vector3.up * Physics.gravity.y * jumpHoldFloat * Time.deltaTime;
            playerRB.AddForce(Vector3.up * Physics.gravity.magnitude * jumpHoldFloat * Time.deltaTime, ForceMode.Acceleration);
        }
        //Jump released
        if (Input.GetButtonUp("Jump"))
        {
            jumpReleased = true;
        }
        //Peak of the jump. makes the player float a bit at the top
        if (playerRB.velocity.y > -peakEndVelocity && playerRB.velocity.y < peakStartVelocity && !grounded)
        {
            playerRB.velocity -= Vector3.up * Physics.gravity.y * (floatFactor) * Time.deltaTime;
            jumpReleased = true;
        }
        else if (playerRB.velocity.y < -peakEndVelocity && !grounded)   //Makes the player fall faster after jumping
        {
            //playerRB.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            playerRB.AddForce(Physics.gravity * (fallMultiplier - 1) * Time.deltaTime, ForceMode.Acceleration);
        }

        //Jump input buffer. If the player presses jump before hitting the ground, this save that input for a small time and perform the jump once the player hits the round. This is to make bunnyhopping easier
        if (Input.GetButtonDown("Jump") && !grounded && !grabbing)
        {
            jumpInputBufferTimer = jumpInputBuffer;
        }
        if (jumpInputBufferTimer > 0)
        {
            jumpInputBufferTimer -= Time.deltaTime;
            if (grounded)
            {
                InitialJump();
                jumpInputBufferTimer = 0;

                //KostinKoodi
                JumpSound();
            }
        }


        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
            coyoteTimeTimer = 1;
            playerRB.useGravity = true;
            return;
        }

        //Coyote time
        if (grounded)
        {
            coyoteTimeTimer = 0f;
            playerRB.useGravity = false;
            playerRB.AddForce(-GroundCheck.normal * Physics.gravity.magnitude * 2f, ForceMode.Acceleration);
        }
        else
        {
            coyoteTimeTimer += Time.deltaTime;
            playerRB.useGravity = true;
        }

        //Initial jump. Down here because this needs to be limited by the cooldown
        if (coyoteTimeTimer < coyoteTime && Input.GetButtonDown("Jump"))
        {
            InitialJump();
        }

    }

    void InitialJump()
    {
        playerRB.useGravity = true;
        jumpCooldownTimer = jumpCooldown;
        Vector3 tempVector = playerRB.velocity;
        tempVector.y = 0f;
        playerRB.velocity = tempVector;
        jumpReleased = false;

        jumpDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (jumpDir.x != 0 && jumpDir.z != 0)
            diagonalJump = 0.707f;
        else
            diagonalJump = 1;
        jumpDir = transform.TransformDirection(jumpDir);
        //Debug.Log(jumpDir);

        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerRB.AddForce(jumpDir * hopForce * diagonalJump, ForceMode.Impulse);

        //KostinKoodi
        JumpSound();
    }
    float grabTimer = 0f;
    float grabTime = 0.5f;
    float ledgeSearchTime = 0.2f;
    Vector3 ledgeClimbDir;
    RaycastHit hit;
    int enemyLayer = ~(1 << 14);
    void LedgeGrab()
    {
        if (!grounded && Input.GetButton("Jump") && !grabbing)
        {
            //Checks inputs and if there is a ledge in front of the player with 2 colliders. 1 above the ledge and 1 on top. the one above needs to be clear and the one of the front needs to collide for it to be seen as a ledge
            if (frontGrabColliding && !topGrabColliding && grabbableLedge)
            {
                //Debug.Log("HITS" + grabbableLedge);

                //Calculates the position from which to shoot a ray toward the object. uses the objects y position and the players x and z so the ray will more likely hit a good surface.
                //This ray is used to calculate a normal from the surface hit so the player will be moven towards it in an upward trajectory making the player climb it.
                Vector3 grabRayPos = new Vector3(transform.position.x, grabbableLedge.transform.position.y, transform.position.z);      //If causes issues, change ray to go from player to object instead of taking the Y position from the colliding object
                if (Physics.Raycast(grabRayPos, grabbableLedge.transform.position - grabRayPos, out hit, 1, PlayerMovemenMask))
                {
                    Debug.DrawRay(grabRayPos, grabbableLedge.transform.position - grabRayPos, Color.white, 5);
                    ledgeClimbDir = hit.normal * -1;
                    grabbing = true;
                    GetComponent<MouseLook>().enabled = false;
                    if (turnWhenClimbing)   //If the player is wanted to turn toward the ledge being scaled
                    {
                        transform.forward = new Vector3(ledgeClimbDir.x, 0, ledgeClimbDir.z);
                        GetComponent<MouseLook>().originalRotation = transform.localRotation;
                        GetComponent<MouseLook>().rotationX = 0;
                    }
                }
                //else
                //{
                //    Debug.DrawRay(grabRayPos, grabbableLedge.transform.position - grabRayPos, Color.white, 5);
                //}
            }
        }
        //Once the player is seen as climbing, the player will be moved in an upward trajectory based on the surface normal for a set duration until the time ends or until the player is concidered grounded again
        if (grabbing)
        {
            jumpInputBufferTimer = 0;       //So player wont immediately jump after climbing
            playerRB.velocity = ((transform.up + (ledgeClimbDir * 1.2f)) * climbSpeed);
            grabTimer += Time.deltaTime;
            if (grounded || grabTimer > grabTime)
            {
                grabTimer = 0;
                grabbing = false;
                playerRB.velocity = new Vector3(0, 0, 0);
                GetComponent<MouseLook>().enabled = true;
            }
            if (grabTimer < ledgeSearchTime && grabbableLedge == null)
            {
                grabTimer = 0;
                grabbing = false;
                playerRB.velocity = new Vector3(0, 0, 0);
                GetComponent<MouseLook>().enabled = true;
            }
        }
    }

    public void climbCollide(Collider collider)
    {
        //Debug.Log(collider);
        grabbableLedge = collider;
    }

    //int i = 0;
    float dodgeTimer = 0;
    float cooldownTimer = 0;
    Vector3 dodgeDir;
    float diagonalDodge;
    void Dodge()
    {
        //If the player is concidered dodging the player will be moved toward the wanted direction at a set speed. after which the player's velocity will be set as another velocity preferably slower after the dodge ends
        if (dodging)
        {
            dodgeTimer += Time.deltaTime;
            playerRB.velocity = (dodgeDir * dodgeSpeed) * diagonalDodge;
            if (dodgeTimer > dodgeDuration)
            {
                playerRB.velocity = (dodgeDir * dodgeExitSpeed) * diagonalDodge;
                dodging = false;
                dodgeTimer = 0;
            }
            //playerRB.AddForce(transform.TransformDirection(dodgeDir) * 100, ForceMode.Impulse);
        }
        //Cooldown timer is down here because it is only designed to deny input. The stuff above is needed every frame to move the player
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }
        //Checks the input for dodges and if the player has access to air dodge
        if (!dodging && Input.GetButtonDown("Dodge"))
        {
            if (!airDodge && grounded || airDodge)
            {
                //Dodge direction is based on the direction of input. Dodge direction defaults to backward if no direction input is given
                dodgeDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                if (dodgeDir.x == 0 && dodgeDir.z == 0)
                    dodgeDir.z = -1;
                if (dodgeDir.x != 0 && dodgeDir.z != 0)
                    diagonalDodge = 0.707f;
                else
                    diagonalDodge = 1;
                dodgeDir = transform.TransformDirection(dodgeDir);
                dodging = true;
                cooldownTimer = dodgeCooldown;
                //Debug.Log("DODGE" + i);
                //i++;
                DodgeSound();

            }
        }
    }
   
    
    //KostinKoodi
    void JumpSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Player/Small/Player_small_jump", gameObject);
    }

    void DodgeSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Player/Small/Player_small_dash", gameObject);
    }
    void ClimbSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Player/Small/Player_small_climb", gameObject);
    }
    void MovementSound()
    {
        //KostinKoodi
        RaycastHit movementRayHit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out movementRayHit, transform.lossyScale.y + 0.5f, LayerMask.GetMask("Geometry")))
        {
            Color32 bloodColor = new Color32(138,3,3,50);
            //Debug.Log("Asd" + GetPixelColor(movementRayHit));
            if (GetPixelColor(movementRayHit).ColorEq(bloodColor))
            {
                walkingOnBlood = true;
            }
            else
            {
                walkingOnBlood = false;
            }
        }

        GrapplingHook grapplingHook = transform.Find("Main Camera").GetComponent<GrapplingHook>();

        if (movingWithInput == true && dodging == false && grounded == true && !grapplingHook.newGrapplingHook)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Player/Small/Player_small_movement", gameObject);

            if (walkingOnBlood)
            {
                
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Blood/Sx_blood_footsteps", gameObject);
            } 
        }
    }
   
    
}


//float localForwardVelocity = Vector3.Dot(playerRB.velocity, transform.forward);
//Debug.Log(localForwardVelocity);
