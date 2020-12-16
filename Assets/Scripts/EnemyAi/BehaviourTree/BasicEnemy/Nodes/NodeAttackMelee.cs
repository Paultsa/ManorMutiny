using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class NodeAttackMelee : IBehaviourTreeNode

{
    private EnemyAi enemy;
    float timer;
    float turnSpeed = 6f;
    float rangedAttackBuffer = 2f;
    float knockback = 5f;
    bool attacking;
    public bool damageDealt;

    float frontSwing;
    float attackTime;
    float backSwing;
    public NodeAttackMelee(EnemyAi _enemy)
    {
        enemy = _enemy;

        frontSwing = enemy.meleeFrontSwing;
        attackTime = enemy.meleeAttackTime;
        backSwing = enemy.meleeBackSwing;

    }

    public void EnterState()
    {
        enemy.currentState = this;
        enemy.navMeshAgent.isStopped = true;
        attacking = false;
        timer = 0;
        //Debug.Log("DONG");
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void UpdateState()
    {

        if (!attacking && enemy.FaceTargetCheck(rangedAttackBuffer) && enemy.meleeAttackAvailable)
        {
            /*enemy.meleeFrontSwing = false;
            enemy.meleeSwing = false;
            enemy.meleeBackSwing = false;*/
            enemy.meleeAttackStart = false;
            enemy.meleeAttackEnd = false;

            attacking = true;
            damageDealt = false;
        }
        if (!attacking && enemy.navMeshAgent.remainingDistance >= enemy.navMeshAgent.stoppingDistance)      //Maybe not needed. depends how stiff we want the AI to be. remove if needs to be stiffer
        {
            enemy.chase.EnterState();
        }

        if (attacking)
        {

            timer += Time.deltaTime;

            if (timer <= frontSwing)        //Front swing of the attack animation
            {
                /*if (!enemy.meleeFrontSwing)
                    enemy.meleeFrontSwing = true;*/
                if (!enemy.meleeAttackStart)
                    enemy.meleeAttackStart = true;

                if (enemy.IndicatorTemp)
                    enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.yellow;
                //Debug.Log("FRONT SWING");
            }
            else if (timer >= frontSwing && timer < frontSwing + attackTime)        //Beginning of the attack after front swing. Performs attack
            {
                /*enemy.meleeFrontSwing = false;
                if (!enemy.meleeSwing)
                    enemy.meleeSwing = true;*/

                if (enemy.IndicatorTemp)
                    enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.red;
                //Debug.Log("PERFORM ATTACK");
                if (!damageDealt)
                {
                    MeleeCollision(enemy.transform.position + enemy.transform.forward, 1);
                }
            }
            else if (timer >= frontSwing + attackTime && timer < frontSwing + attackTime + backSwing)       //Back swing of the animation
            {
                /*enemy.meleeSwing = false;
                if (!enemy.meleeBackSwing)
                    enemy.meleeBackSwing = true;*/
                enemy.meleeAttacking = true;
                enemy.MeleeRecovering = true;
                enemy.meleeAttackStart = false;

                if (enemy.IndicatorTemp)
                    enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.blue;
                //Debug.Log("BACK SWING");
            }
            else if (timer >= frontSwing + attackTime + backSwing)      //End of attack animation.
            {
               //enemy.meleeBackSwing = false;
                enemy.meleeAttackEnd = true;
                enemy.meleeAttacking = false;
                enemy.MeleeRecovering = false;

                if (enemy.IndicatorTemp)
                    enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.gray;
                //Debug.Log("END ATTACK");
                enemy.StartMeleeCooldown();
                enemy.chase.EnterState();
            }
        }
        else
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(enemy.target.position.x, enemy.transform.position.y, enemy.target.position.z) - enemy.transform.position);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, rotation, Time.deltaTime * turnSpeed);
            if (!enemy.CheckLineOfSight(rangedAttackBuffer))
            {
                enemy.chase.EnterState();
            }
        }
    }

    void MeleeCollision(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if ((hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo")) && hitCollider.GetComponent<IHealth>() != null)
            {
                hitCollider.GetComponent<IHealth>().TakeDamage(enemy.meleeAttackDamage, enemy.transform.position);
                hitCollider.GetComponent<Rigidbody>().AddForce((hitCollider.transform.position - enemy.transform.position).normalized * enemy.meleeKnockback, ForceMode.Impulse);
                enemy.meleeDamageDealt = true;
                damageDealt = true;
            }
        }
    }
}
