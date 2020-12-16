using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAttack : IBehaviourTreeNode
{

    private EnemyAi enemy;
    float turnSpeed = 6f;
    float rangedAttackBuffer = 2f;
    public NodeAttack(EnemyAi _enemy)
    {
        enemy = _enemy;
    }

    public void EnterState()
    {
        enemy.currentState = this;
        enemy.navMeshAgent.isStopped = true;
        RollRandom.RollForProbability(enemy.specialAttackProbability);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    RaycastHit targetingRay;
    RaycastHit lineOfSightRay;
    bool isFacingTarget;
    bool hasLineOfSight;
    public void UpdateState()
    {
        hasLineOfSight = enemy.CheckLineOfSight(rangedAttackBuffer);

        if (!hasLineOfSight)
        {
            enemy.chase.EnterState();
            return;
        }


        //Special attack
        if (enemy.hasSpecialAttack && enemy.specialAttackAvailable && (enemy.target.position - enemy.transform.position).magnitude >= enemy.minSpecialAttackRange)
        {
            if (RollRandom.RollForProbability(enemy.specialAttackProbability))
            {
                //Debug.Log("SPECIAL");
                enemy.special.EnterState();
                return;
            }
            else
                enemy.StartSpecialCooldown();
        }

        //Melee attack
        else if (enemy.hasMeleeAttack && enemy.navMeshAgent.hasPath && enemy.navMeshAgent.remainingDistance < enemy.navMeshAgent.stoppingDistance)
        {
            //Debug.Log("MELEE");
            enemy.melee.EnterState();
        }

        //Continue chasing
        else//if (enemy.navMeshAgent.hasPath && (enemy.target.position - enemy.transform.position).magnitude >= enemy.navMeshAgent.stoppingDistance)
        {
            //Debug.Log("CHASING");
            enemy.chase.EnterState();
            return;
        }

        /*if (enemy.grounded && enemy.navMeshAgent.hasPath && enemy.navMeshAgent.remainingDistance <= enemy.minSpecialAttackRange)
        {
            Debug.Log("CHASING");
            enemy.chase.EnterState();
            return;
        }*/

    }


}
