using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Jimmy
//Kostin koodimuistiinpanot: Lunge, aggro, ranged

public class EnemyAi : MonoBehaviour
{
    public enum SpecialAttackType
    {
        Lunge,
        Ranged,
        DoubleShot
    };

    public enum EnemyType       //This can be used in nodes for sounds with "if (enemy.enemyType == EnemyType.(Change this))"
    {
        Fencer,
        FlyingBlob,
        SharkDog,
        Mancubus,
        CyberDemon
    };

    public EnemyType enemyType = EnemyType.Fencer;

    [Header("Special attack settings")]
    [Header("Enemy stats")]
    [Tooltip("Special attack type")]
    public SpecialAttackType specialAttackType = SpecialAttackType.Lunge;
    [Tooltip("If this enemy can perform a special attack")]
    public bool hasSpecialAttack;
    [Tooltip("Maximum attack range for any attack")]
    public float maxSpecialAttackRange;
    [Tooltip("Maximum attack range for special attack")]
    public float minSpecialAttackRange;
    [Tooltip("Once the player is within the max attack range, how probable is it that the enemy makes a single ranged attack. 0-100%")]
    public int specialAttackProbability;
    [Tooltip("After performing a ranged attack, how probable is it that the enemy holds position and makes another ranged attack. 0-100%")]
    public int specialAttackContinueProbability;
    [Tooltip("Time between special attacks")]
    public float specialAttackCooldown;
    [Tooltip("Damage dealt by special attack")]
    public int specialAttackDamage;
    [Tooltip("How hard the player is knocked back by this")]
    public float specialKnockback = 5;
    [Tooltip("Windup time for special attack")]
    public float specialFrontSwing = 0.4f;
    [Tooltip("Duration of the attack itself for special attack")]
    public float specialAttackTime = 0.6f;
    [Tooltip("Recovery time for special attack")]
    public float specialBackSwing = 0.6f;
    [Tooltip("Movement speed for the special attack (projectile speed for ranged)")]
    public float specialSpeed = 2f;

    [Header("Melee attack settings")]
    [Tooltip("Time between melee attacks")]
    public float meleeAttackCooldown;
    [Tooltip("Damage dealt by melee attack")]
    public int meleeAttackDamage;
    [Tooltip("How hard the player is knocked back by this")]
    public float meleeKnockback = 5;
    [Tooltip("If this enemy can perform a melee attack")]
    public bool hasMeleeAttack = true;
    [Tooltip("Windup time for melee attack")]
    public float meleeFrontSwing = 0.2f;
    [Tooltip("Duration of the attack itself for melee attack")]
    public float meleeAttackTime = 0.2f;
    [Tooltip("Recovery time for melee attack")]
    public float meleeBackSwing = 0.6f;

    [HideInInspector]
    public bool specialAttackAvailable;
    [HideInInspector]
    public bool meleeAttackAvailable;

    LayerMask enemySightMask;


    [HideInInspector]
    public Transform[] targets;

    public Transform target;

    public GameObject IndicatorTemp;

    public GameObject rangedProjectile;

    public GameObject projectileExpObj;

    public IBehaviourTreeNode currentState;

    public IBehaviourTreeNode chase;

    public IBehaviourTreeNode attack;

    public IBehaviourTreeNode stunned;

    public IBehaviourTreeNode special;

    public IBehaviourTreeNode melee;

    [HideInInspector]
    public NPCNavigation npcNav;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public bool nearGround;
    [HideInInspector]
    public bool lunging;
    [HideInInspector]
    public float specialAttackCooldownTimer;
    [HideInInspector]
    public float meleeAttackCooldownTimer;

    [HideInInspector]
    public bool specialAttackStart;
    [HideInInspector]
    public bool specialAttacking;
    [HideInInspector]
    public bool specialRecovering;
    [HideInInspector]
    public bool specialAttackEnd;
    [HideInInspector]
    public bool specialDamageDealt;
    [HideInInspector]
    public bool meleeAttackStart;
    [HideInInspector]
    public bool meleeAttacking;
    [HideInInspector]
    public bool MeleeRecovering;
    [HideInInspector]
    public bool meleeAttackEnd;
    [HideInInspector]
    public bool meleeDamageDealt;

    int attackType;

    //KostinKoodi   
    FMOD.Studio.EventInstance enemyMovementSound;
    FMOD.Studio.EventInstance enemyAttackSound;
    FMOD.Studio.EventInstance enemySpecialSound;
    FMOD.Studio.EventInstance enemyHitSound;

    string sound_path;



    // Use this for initialization
    void Awake()
    {
        attackType = (int)specialAttackType;

        chase = new NodeChase(this);
        attack = new NodeAttack(this);
        stunned = new NodeStunned(this);
        melee = new NodeAttackMelee(this);
        if (attackType == (int)SpecialAttackType.Lunge)
        {
            special = new NodeSpecialAttackLunge(this);
        }
        if (attackType == (int)SpecialAttackType.Ranged)
        {
            special = new NodeSpecialAttackRanged(this);
        }
        if (attackType == (int)SpecialAttackType.DoubleShot)
        {
            special = new NodeSpecialAttackRangedDouble(this);
        }

        npcNav = GetComponent<NPCNavigation>();
        if (npcNav == null)
            npcNav = transform.parent.GetComponent<NPCNavigation>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
            navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();

        targets = new Transform[2];

    }

    void Start()
    {
        enemySightMask = LayerMask.GetMask("Player") | LayerMask.GetMask("Default") | LayerMask.GetMask("Geometry");

        GameObject expTarget = new GameObject("ExpTarget");
        expTarget.transform.parent = gameObject.transform;
        expTarget.transform.SetAsLastSibling();
        expTarget.transform.localPosition = new Vector3(0, 0, 0);
        expTarget.layer = LayerMask.NameToLayer("ExpTarget");
        expTarget.AddComponent<SphereCollider>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (enemyType == EnemyType.FlyingBlob)
        {
            expTarget.AddComponent<SphereCollider>().transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        lunging = false;
        currentState = chase;

        if (GameManager.gameManager)
        {
            if (GameManager.gameManager.playerOne)
                targets[0] = GameManager.gameManager.playerOne.transform;
            if (GameManager.gameManager.playerTwo)
                targets[1] = GameManager.gameManager.playerTwo.transform;

            target = targets[0];


            if (GameManager.gameManager.playerCount == 2)
            {
                if ((transform.position - targets[0].position).magnitude > (transform.position - targets[1].position).magnitude)
                {
                    target = targets[1];
                    target = targets[1];
                }
            }
        }
        //KostinKoodi
        switch (enemyType)
        {
            case EnemyType.CyberDemon:
                movementSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                attackSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_attack_swing";
                specialSoundPath = "event:/VO/Monster/Melee01/Vo_meleeMonstrer01_die";
                //hitSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                break;
            
            case EnemyType.Mancubus:
                movementSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                attackSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_attack_swing";
                specialSoundPath = "event:/VO/Monster/Melee01/Vo_meleeMonstrer01_die";
                //hitSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                break;

            case EnemyType.Fencer:
                movementSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                attackSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_attack_swing";
                specialSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_dash";
                //hitSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                break;

            case EnemyType.FlyingBlob:
                movementSoundPath = "event:/SX/Monsters/ranged01/Sx_rangedMonster_movement";
                attackSoundPath = "event:/SX/Monsters/ranged01/Sx_rangedMonster_attack";
                specialSoundPath = "event:/SX/Monsters/ranged01/Sx_rangedMonster_attack";
                //hitSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                break;

            case EnemyType.SharkDog:
                movementSoundPath = "event:/SX/Monsters/Melee02 Shatk/sx_meleeShark_movement";
                attackSoundPath = "event:/SX/Monsters/Melee02 Shatk/sx_monsterShark_attack";
                specialSoundPath = "event:/SX/Monsters/Melee02 Shatk/sx_monsterShark_attack";
                // hitSoundPath = "event:/SX/Monsters/Melee01/Sx_meleeMonster_movement";
                break;
        }
        enemyMovementSound = FMODUnity.RuntimeManager.CreateInstance(movementSoundPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(enemyMovementSound, GetComponent<Transform>(), GetComponent<Rigidbody>());


        enemyAttackSound = FMODUnity.RuntimeManager.CreateInstance(attackSoundPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(enemyAttackSound, GetComponent<Transform>(), GetComponent<Rigidbody>());


        enemySpecialSound = FMODUnity.RuntimeManager.CreateInstance(specialSoundPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(enemySpecialSound, GetComponent<Transform>(), GetComponent<Rigidbody>());

        /* enemyHitSound = FMODUnity.RuntimeManager.CreateInstance(hitSoundPath);
         FMODUnity.RuntimeManager.AttachInstanceToGameObject(enemyHitSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
                 */
    }

    // Update is called once per frame
    void Update()
    {
        //Grounded check raycast
        //Debug.Log(navMeshAgent.isOnOffMeshLink);

        AttackCooldowns();

        grounded = !navMeshAgent.isOnOffMeshLink;
        nearGround = isGrounded();


        currentState.UpdateState();

        if (navMeshAgent.enabled && target != null)
            npcNav.SetDestination(target);


        if (GetComponent<Rigidbody>() && currentState != stunned && grounded && !lunging && nearGround)
            Destroy(GetComponent<Rigidbody>());


        if (rocketPackBufferFrames == 0)
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Is_targeted", 0);
        }
        if (rocketPackBufferFrames >= 0)
        {
            rocketPackBufferFrames--;
        }

        Sounds();

    }

    RaycastHit GroundCheck;
    bool isGrounded()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out GroundCheck, transform.lossyScale.y + 0.3f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartSpecialCooldown()
    {
        specialAttackCooldownTimer = specialAttackCooldown;
        specialAttackAvailable = false;
        //Debug.Log("PING" + specialAttackAvailable);
    }

    public void StartMeleeCooldown()
    {
        meleeAttackCooldownTimer = meleeAttackCooldown;
        meleeAttackAvailable = false;
        //Debug.Log("PING" + meleeAttackAvailable);
    }

    void AttackCooldowns()
    {
        if (specialAttackCooldownTimer >= 0)
        {
            specialAttackCooldownTimer -= Time.deltaTime;
            //specialAttackAvailable = false;
        }
        else
        {
            specialAttackAvailable = true;
        }

        if (meleeAttackCooldownTimer >= 0)
        {
            meleeAttackCooldownTimer -= Time.deltaTime;
            //specialAttackAvailable = false;
        }
        else
        {
            meleeAttackAvailable = true;
        }
    }


    RaycastHit targetingRay;
    RaycastHit lineOfSightRay;
    public bool FaceTargetCheck(float specialAttackBuffer)
    {
        float forwardVectorElevation = (target.transform.position - transform.position).normalized.y;
        Vector3 forwardVectorWithElevation = new Vector3(transform.forward.x, forwardVectorElevation, transform.forward.z);
        Vector3 temp = (target.transform.position - transform.position);
        float angle = Vector3.Angle(Vector3.down, temp);

        if (angle < 5)
        {
            forwardVectorWithElevation.x *= 0.1f;
            forwardVectorWithElevation.z *= 0.1f;
        }
        else if (angle < 15)
        {
            forwardVectorWithElevation.x *= 0.2f;
            forwardVectorWithElevation.z *= 0.2f;
        }
        else if (angle < 25)
        {
            forwardVectorWithElevation.x *= 0.4f;
            forwardVectorWithElevation.z *= 0.4f;
        }
        else if (angle < 35)
        {
            forwardVectorWithElevation.x *= 0.6f;
            forwardVectorWithElevation.z *= 0.6f;
        }
        else if (angle < 45)
        {
            forwardVectorWithElevation.x *= 0.8f;
            forwardVectorWithElevation.z *= 0.8f;
        }

        if (Physics.Raycast(transform.position, forwardVectorWithElevation, out targetingRay, maxSpecialAttackRange + specialAttackBuffer, enemySightMask))
        {
            //Debug.DrawRay(transform.position, forwardVectorWithElevation * targetingRay.distance, Color.red, 3);
            if (targetingRay.collider.CompareTag("PlayerOne") || targetingRay.collider.CompareTag("PlayerTwo"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.DrawRay(transform.position, forwardVectorWithElevation, Color.yellow, 1);
            return false;
        }
    }

    public bool CheckLineOfSight(float specialAttackBuffer)
    {
        //Line of sight raycast
        if (Physics.Raycast(transform.position, target.position - transform.position, out lineOfSightRay, maxSpecialAttackRange + specialAttackBuffer, enemySightMask))
        {
            //Debug.Log("RAY " + lineOfSightRay.collider.gameObject);
            //Debug.DrawRay(transform.position, (target.position - transform.position), Color.magenta, 3);
            if (lineOfSightRay.collider.CompareTag("PlayerOne") || lineOfSightRay.collider.CompareTag("PlayerTwo"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.DrawRay(transform.position, (target.position - transform.position), Color.magenta, 1);
            return false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        currentState.OnCollisionEnter(other);
    }

    public void StunEnemy(float stunDuration, Vector3 knockbackOrigin, float knockbackAmount)
    {
        stunned.EnterState();
        ((NodeStunned)stunned).Stun(stunDuration);
        GetComponent<Rigidbody>().AddForce((transform.position - knockbackOrigin).normalized * knockbackAmount, ForceMode.Impulse);
    }

    public GameObject ShootProjectile(Vector3 direction, float force)
    {
        Vector3 tempPos = new Vector3(transform.position.x, transform.position.y + (transform.lossyScale.y * 0.5f), transform.position.z) + transform.forward * 0.5f;
        GameObject projectile = Instantiate(rangedProjectile, tempPos, Quaternion.identity);
        projectile.GetComponent<EnemyProjectile>().damage = specialAttackDamage;
        projectile.GetComponent<EnemyProjectile>().shooter = transform;
        projectile.GetComponent<EnemyProjectile>().explosionObj = projectileExpObj;
        projectile.GetComponent<Rigidbody>().AddForce(direction.normalized * force, ForceMode.Impulse);
        projectile.transform.forward = direction;

        return projectile;
    }

    int rocketPackBufferFrames;
    private string movementSoundPath;
    private string attackSoundPath;
    private string specialSoundPath;

    public void inRocketPackAoe()
    {
        if (rocketPackBufferFrames != 0)
        {
            ///Debug.LogError("Hit");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Is_targeted", 1);
        }

        //transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        rocketPackBufferFrames = 2;
    }

    public void ReAggro(Transform newTarget)
    {
        target = newTarget;
        if (GetComponent<FlyingEnemyPosition>())
        {
            GetComponent<FlyingEnemyPosition>().target = newTarget;
        }
    }


    public void ReAggro()
    {
        if ((transform.position - targets[0].position).magnitude > (transform.position - targets[1].position).magnitude)
        {
            target = targets[1];
        }
        else
        {
            target = targets[0];
        }
        if (GetComponent<FlyingEnemyPosition>())
        {
            GetComponent<FlyingEnemyPosition>().target = target;
        }
    }

    /*
     * Efectit: movement, attack, attack+hit, "special", take damage,
    * Voice: idle, death, take damage
    */




    void Sounds()
    {
        //Debug.Log(currentState);

        if (meleeDamageDealt || specialDamageDealt)
        {
            meleeDamageDealt = false;
            specialDamageDealt = false;
        }

        if (currentState == chase && navMeshAgent.enabled && !navMeshAgent.isStopped)       //Enemy is moving
        {
            FMOD.Studio.PLAYBACK_STATE state;
            enemyMovementSound.getPlaybackState(out state);


            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                enemyMovementSound.start();
            }

        }
        else if (navMeshAgent.enabled && navMeshAgent.isStopped && enemyType != EnemyType.FlyingBlob)
        {
            enemyMovementSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }


        if (currentState == special && specialAttackStart)                                  //Enemy special attack starts with a small windup (all attacks have a short windup, attack itself and a recovery)
        {


            FMOD.Studio.PLAYBACK_STATE state;
            enemySpecialSound.getPlaybackState(out state);

            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                enemySpecialSound.start();
            }
        }
        if (currentState == special && specialAttacking)                                    //The main part of the animation where the enemy can deal damage
        {


        }
        if (specialDamageDealt)                                                             //Player gets hit with special (this is true for only one frame)
        {


        }
        if (currentState == special && specialAttackEnd)                                    //Enemy special attack ends (this is probably not needed)
        {

        }
        if (currentState == melee && meleeAttackStart)                                      //Enemy melee attack starts with a small windup 
        {


        }
        if (currentState == melee && meleeAttacking)                                        //The main part of the animation where the enemy can deal damage
        {

            FMOD.Studio.PLAYBACK_STATE state;
            enemyAttackSound.getPlaybackState(out state);


            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                enemyAttackSound.start();
            }

        }
        if (meleeDamageDealt)                                                               //Player gets hit with melee (this is true for only one frame)
        {
            //if (enemyType == EnemyType.Fencer)
            {

            }

        }
        if (currentState == special && meleeAttackEnd)                                      //Enemy melee attack ends (this is probably not needed)
        {

        }
    }

    private void OnDestroy()
    {
        enemyMovementSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
