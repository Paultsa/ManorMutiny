using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class NodeChase : IBehaviourTreeNode
{

    private EnemyAi enemy;
    public NodeChase(EnemyAi _enemy)
    {
        enemy = _enemy;
    }

    public void EnterState()
    {
        enemy.currentState = this;
        enemy.navMeshAgent.enabled = true;
        enemy.navMeshAgent.isStopped = false;
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    RaycastHit lineOfSightRay;
    bool hasLineOfSight = true;
    public void UpdateState()
    {
        //TODO Add a raycast to check for line of sight before the enemy can stop to make a ranged attack. if no line of sight the enemy must keep moving closer
        hasLineOfSight = enemy.CheckLineOfSight(0);

        if (enemy.hasSpecialAttack && hasLineOfSight && enemy.grounded && enemy.specialAttackAvailable && (enemy.target.position - enemy.transform.position).magnitude >= enemy.minSpecialAttackRange) // && enemy.navMeshAgent.remainingDistance >= enemy.minSpecialAttackRange && enemy.navMeshAgent.remainingDistance <= enemy.maxSpecialAttackRange)
        {
            //Debug.Log("SPECIAL ATTACK");
            enemy.attack.EnterState();
            return;
        }

        if (enemy.hasMeleeAttack && enemy.navMeshAgent.hasPath && enemy.grounded && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && enemy.navMeshAgent.velocity.magnitude <= 0)
        {
            //Debug.Log("DESTINATION REACHED");
            enemy.attack.EnterState();
            return;
        }

    }
}

