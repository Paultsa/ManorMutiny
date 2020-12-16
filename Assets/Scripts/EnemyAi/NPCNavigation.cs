using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NPCNavigation : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public Transform destination;
    public Vector3 distanceToTargetVector;
    public float distanceToTarget;



    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        /*if (destination != null)
        {
            SetDestination(destination);
        }
        else
        {
            Debug.LogError("NPC" + gameObject.name + " has no destination Transform attached to it");
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDestination(Vector3 targetVector)
    {
        if (targetVector != null)
        {
            distanceToTarget = navMeshAgent.remainingDistance;
            //Debug.Log(navMeshAgent.remainingDistance + " " + navMeshAgent.velocity.magnitude + " " + navMeshAgent.stoppingDistance + " " + navMeshAgent.hasPath);




            navMeshAgent.SetDestination(targetVector);
        }
        else
        {
            Debug.LogError("NPC" + gameObject.name + " has no destination Transform attached to it");
        }
    }

    public void SetDestination(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            distanceToTarget = navMeshAgent.remainingDistance;
            




            navMeshAgent.SetDestination(targetTransform.position);
        }
        else
        {
            Debug.LogError("NPC" + gameObject.name + " has no destination Transform attached to it");
        }
    }
}
