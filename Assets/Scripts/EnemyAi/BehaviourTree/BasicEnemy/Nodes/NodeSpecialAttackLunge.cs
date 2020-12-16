using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class NodeSpecialAttackLunge : IBehaviourTreeNode
{
    private EnemyAi enemy;
    float timer;
    float turnSpeed = 6f;
    float rangedAttackBuffer = 2f;
    bool attacking;
    bool damageDealt;
    //bool lunging;

    float frontSwing;
    float attackTime;
    float backSwing;

    Vector3 lungeDir;
    float lungeSpeed;// = 1000f;
    public NodeSpecialAttackLunge(EnemyAi _enemy)
    {
        enemy = _enemy;

        lungeSpeed = enemy.specialSpeed;
        frontSwing = enemy.specialFrontSwing;
        attackTime = enemy.specialAttackTime;
        backSwing = enemy.specialBackSwing;
    }

    public void EnterState()
    {
        enemy.currentState = this;
        enemy.navMeshAgent.isStopped = true;
        attacking = false;
        timer = 0;
        //Debug.Log("DING");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (enemy.lunging)
        {
            if ((collision.collider.CompareTag("PlayerOne") || collision.collider.CompareTag("PlayerTwo")) && collision.transform.GetComponent<IHealth>() != null)
            {
                collision.transform.GetComponent<IHealth>().TakeDamage(enemy.specialAttackDamage, enemy.transform.position);
                collision.transform.GetComponent<Rigidbody>().AddForce((collision.transform.position - enemy.transform.position).normalized * enemy.specialKnockback, ForceMode.Impulse);
                enemy.specialDamageDealt = true;
                enemy.lunging = false;
                timer = frontSwing + attackTime;    //Remove if the enemy needs to lunge through the player. This makes the enemy stop on impact
            }
        }
    }

    public void UpdateState()
    {

        if (!attacking && enemy.FaceTargetCheck(rangedAttackBuffer) && enemy.specialAttackAvailable)
        {
            /*enemy.specialFrontSwing = false;
            enemy.specialSwing = false;
            enemy.specialBackSwing = false;*/
            enemy.specialAttackStart = false;
            enemy.specialAttackEnd = false;

            attacking = true;
            lungeDir = enemy.transform.forward;
            damageDealt = false;
        }
        if (!attacking && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)      //Maybe not needed. depends how stiff we want the AI to be. remove if needs to be stiffer
        {
            enemy.chase.EnterState();
        }

        if (attacking)
        {


            timer += Time.deltaTime;

            if (timer <= frontSwing)        //Front swing of the attack animation
            {
                /*if (!enemy.specialFrontSwing)
                    enemy.specialFrontSwing = true;*/
                if (!enemy.specialAttackStart)
                    enemy.specialAttackStart = true;

                //if (enemy.IndicatorTemp)
                    //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (timer >= frontSwing && timer < frontSwing + attackTime)        //Beginning of the attack after front swing. Performs attack
            {
                /*enemy.specialFrontSwing = false;
                if (!enemy.specialSwing)
                    enemy.specialSwing = true;*/

                //if (enemy.IndicatorTemp)
                    //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.red;
                if (!enemy.GetComponent<Rigidbody>())
                    enemy.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                enemy.navMeshAgent.enabled = false;

                enemy.lunging = true;
                enemy.GetComponent<Rigidbody>().velocity = lungeDir * Time.deltaTime * lungeSpeed;
            }
            else if (timer >= frontSwing + attackTime && timer < frontSwing + attackTime + backSwing)       //Back swing of the animation
            {
                /*enemy.specialSwing = false;
                if (!enemy.specialBackSwing)
                    enemy.specialBackSwing = true;*/
                enemy.specialAttacking = true;
                enemy.specialRecovering = true;
                enemy.specialAttackStart = false;

                //if (enemy.IndicatorTemp)
                //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.blue;
                enemy.navMeshAgent.enabled = true;
                enemy.lunging = false;
                //Debug.Log("BACK SWING");
            }
            else if (timer >= frontSwing + attackTime + backSwing)      //End of attack animation.
            {
                //enemy.specialBackSwing = false;
                enemy.specialAttackEnd = true;
                enemy.specialAttacking = false;
                enemy.specialRecovering = false;

                //if (enemy.IndicatorTemp)
                //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.gray;
                //Debug.Log("END ATTACK");
                enemy.StartSpecialCooldown();
                if (RollRandom.RollForProbability(enemy.specialAttackContinueProbability) && enemy.navMeshAgent.remainingDistance > enemy.navMeshAgent.stoppingDistance)
                    enemy.special.EnterState();
                else
                    enemy.chase.EnterState();
            }
        }
        else
        {
            enemy.lunging = false;
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
            //hitCollider.SendMessage("AddDamage");
            if ((hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo")) && hitCollider.GetComponent<IHealth>() != null)
            {
                hitCollider.GetComponent<IHealth>().TakeDamage(enemy.meleeAttackDamage, enemy.transform.position);
                damageDealt = true;
            }
        }
    }
}
