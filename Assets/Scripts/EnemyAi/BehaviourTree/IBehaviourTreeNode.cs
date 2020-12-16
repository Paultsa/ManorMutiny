using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IBehaviourTreeNode
{
    void EnterState();
    void OnCollisionEnter(Collision collision);
    void UpdateState();

}
