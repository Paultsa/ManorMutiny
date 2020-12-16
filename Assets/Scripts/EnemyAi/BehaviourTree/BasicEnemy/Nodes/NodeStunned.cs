using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class NodeStunned : IBehaviourTreeNode
{

    private EnemyAi enemy;
    public float stunDuration = 0f;
    public float stunTimer = 0f;
    public NodeStunned(EnemyAi _enemy)
    {
        enemy = _enemy;
    }

    public void EnterState()
    {
        enemy.currentState = this;
        enemy.navMeshAgent.enabled = false;
        Debug.Log("STUNNED");
    }

    public void Stun(float stunDuration)
    {
        stunTimer = stunDuration;
        if (!enemy.GetComponent<Rigidbody>())
            enemy.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void UpdateState()
    {
        if (stunTimer >= 0 || !enemy.nearGround)
        {
            stunTimer -= Time.deltaTime;
            return;
        }
        //enemy.navMeshAgent.enabled = true;
        enemy.chase.EnterState();


    }

}
