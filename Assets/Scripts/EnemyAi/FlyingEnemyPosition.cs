using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyPosition : MonoBehaviour
{
    EnemyAi ai;
    [HideInInspector]
    public Transform parentTransform;

    public LayerMask clearanceMask;
    public LayerMask lineOfSightClearanceMask;
    public Transform target;
    public float distanceAbovePlayer = 4;
    public float heightMovementBuffer = 1.5f;
    public float verticalSpeed = 2;
    float distanceAbovePlayerOriginal;
    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<EnemyAi>().target;
        distanceAbovePlayerOriginal = distanceAbovePlayer;
        ai = GetComponent<EnemyAi>();
        parentTransform = transform.parent;
    }

    // Update is called once per frame
    float followLerp;
    float ceiling;
    float ground;
    float time = 1;
    float timer = 0;
    float distanceFromCeilingOrFloor = 1;
    bool heightClearance;
    bool lineOfSightClearance;
    Vector3 oldPos;
    void Update()
    {
        ceiling = CeilingCheck();
        ground = GroundCheck();
        heightClearance = HeightClearance();
        lineOfSightClearance = LineOfSightClearance();
        if (!heightClearance || !lineOfSightClearance)     //No line of sight, make distance above player 0 so the enemy drops height to player level. And height clearance needs to be clear (doorways)
        {
            distanceAbovePlayer = 1;
            timer = time;
        }
        else if (timer <= 0)
        {
            distanceAbovePlayer = distanceAbovePlayerOriginal;
        }

        if (timer > 0)
            timer -= Time.deltaTime;

        if (transform.position.y < ai.target.transform.position.y + distanceAbovePlayer && ceiling > distanceFromCeilingOrFloor)      //Going up
        {
            if (transform.position.y < ai.target.transform.position.y + heightMovementBuffer && ai.navMeshAgent.enabled && ai.grounded && (!heightClearance || !lineOfSightClearance) && ai.currentState == ai.chase)
            {
                ai.navMeshAgent.isStopped = true;
            }
            else if (ai.navMeshAgent.enabled && ai.currentState == ai.chase)
            {
                ai.navMeshAgent.isStopped = false;
            }

            followLerp = Mathf.Lerp(transform.position.y, ai.target.transform.position.y + distanceAbovePlayer, Time.deltaTime * verticalSpeed);

        }
        else if (ai.navMeshAgent.enabled && ai.currentState == ai.chase)
        {
            ai.navMeshAgent.isStopped = false;
        }

        if (transform.position.y > ai.target.transform.position.y + distanceAbovePlayer && ground > distanceFromCeilingOrFloor)      //Going down
        {
            if (transform.position.y > ai.target.transform.position.y + heightMovementBuffer && ai.navMeshAgent.enabled && ai.grounded && (!heightClearance || !lineOfSightClearance) && ai.currentState == ai.chase)
            {
                ai.navMeshAgent.isStopped = true;
            }
            else if (ai.navMeshAgent.enabled && ai.currentState == ai.chase)
            {
                ai.navMeshAgent.isStopped = false;
            }

            followLerp = Mathf.Lerp(transform.position.y, ai.target.transform.position.y + distanceAbovePlayer, Time.deltaTime * verticalSpeed);

        }
        else if (ai.navMeshAgent.enabled && ai.currentState == ai.chase)
        {
            ai.navMeshAgent.isStopped = false;
        }
        //Debug.Log(ai.navMeshAgent.isStopped + " TEST");
        Vector3 horizontalPosVector = new Vector3(transform.position.x, followLerp, transform.position.z);
        transform.position = horizontalPosVector;



        if (!ai.grounded)
        {
            transform.parent = null;
            Vector3 horizontalPos = new Vector3(parentTransform.position.x, transform.position.y, parentTransform.position.z);
            transform.position = horizontalPos;
        }
        else if (ai.currentState != ai.stunned)
        {
            transform.parent = parentTransform;
        }


        if (ai.currentState == ai.stunned)
        {
            transform.parent = null;
            parentTransform.position = transform.position;
        }
        else if (ai.grounded)
        {
            transform.parent = parentTransform;
        }

        oldPos = transform.position;
    }

    RaycastHit ceilingCheck;
    float ceilingMaxDist = 4;
    float ceilingDistance;
    float CeilingCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out ceilingCheck, ceilingMaxDist, clearanceMask))
        {
            ceilingDistance = ceilingCheck.distance;
            //Debug.DrawRay(transform.position, Vector3.up * ceilingDistance, Color.red, 1);
            return ceilingDistance;
            //Debug.Log(ceilingDistance);
        }
        else
        {
            ceilingDistance = ceilingMaxDist;
            //Debug.DrawRay(transform.position, Vector3.up * ceilingDistance, Color.red, 1);
            return ceilingDistance;
        }
    }

    RaycastHit groundCheck;
    float groundMaxDist = 4;
    float groundDistance;
    float GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundCheck, groundMaxDist, clearanceMask))
        {
            groundDistance = groundCheck.distance;
            //Debug.DrawRay(transform.position, Vector3.down * groundDistance, Color.red, 1);
            return groundDistance;
            //Debug.Log(ceilingDistance);
        }
        else
        {
            groundDistance = groundMaxDist;
            //Debug.DrawRay(transform.position, Vector3.down * groundDistance, Color.red, 1);
            return groundDistance;
        }
    }

    RaycastHit heightClearanceRay;
    float clearanceMarginal = 1f;
    float clearanceRange = 5;
    float clearanceAccuracy = 5;
    bool HeightClearance()
    {
        bool clear = true;
        for (int i = 0; i < clearanceAccuracy; i++)
        {
            float temp = ((float)(i + 1) / clearanceAccuracy);
            if (Physics.Raycast(transform.position + (Vector3.up * (clearanceMarginal * temp)), transform.forward, out heightClearanceRay, clearanceRange, clearanceMask))
            {
               //Debug.DrawRay(transform.position + (Vector3.up * (clearanceMarginal * temp)), transform.forward * heightClearanceRay.distance, Color.green, 1);
                clear = false;
                break;
                //Debug.Log(ceilingDistance);
            }
            else
            {
                //Debug.DrawRay(transform.position + (Vector3.up * (clearanceMarginal * temp)), transform.forward * clearanceRange, Color.green, 1);
            }

        }
        return clear;
    }

    RaycastHit lineOfSightRay;
    bool LineOfSightClearance()
    {
        //Line of sight raycast
        if (Physics.Raycast(transform.position, target.position + (Vector3.up * target.lossyScale.y) - transform.position, out lineOfSightRay, 10, lineOfSightClearanceMask))
        {
            //Debug.DrawRay(transform.position, target.position + (Vector3.up * target.lossyScale.y) - transform.position, Color.blue, 3);
            if (lineOfSightRay.collider.CompareTag("PlayerOne") || lineOfSightRay.collider.CompareTag("PlayerTwo"))
            {
                //Debug.Log("DINGALING");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.DrawRay(transform.position, target.position + (Vector3.up * target.lossyScale.y) - transform.position, Color.blue, 1);
            return true;
        }
    }
}
