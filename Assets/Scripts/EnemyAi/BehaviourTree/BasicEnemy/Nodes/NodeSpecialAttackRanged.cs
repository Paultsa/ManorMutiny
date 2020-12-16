using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSpecialAttackRanged : IBehaviourTreeNode
{
    private EnemyAi enemy;
    float timer;
    float turnSpeed = 6f;
    float rangedAttackBuffer = 2f;
    float projectileSpeed = 2f;
    bool attacking;
    bool projectileLaunched;

    bool damageDealt;

    float frontSwing;
    float attackTime;
    float backSwing;

    public NodeSpecialAttackRanged(EnemyAi _enemy)
    {
        enemy = _enemy;

        projectileSpeed = enemy.specialSpeed;
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
            projectileLaunched = false;
        }
        /*if (!attacking && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)      //Maybe not needed. depends how stiff we want the AI to be. remove if needs to be stiffer
        {
            enemy.chase.EnterState();
        }*/

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

                //Debug.Log("FRONT SWING");
            }
            else if (timer >= frontSwing && timer < frontSwing + attackTime)        //Beginning of the attack after front swing. Performs attack
            {
                /*enemy.specialFrontSwing = false;
                if (!enemy.specialSwing)
                    enemy.specialSwing = true;*/
                enemy.specialAttacking = true;

                //if (enemy.IndicatorTemp)
                    //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.red;
                
                if (!projectileLaunched)
                {
                    Vector3 temp = new Vector3(enemy.transform.position.x, enemy.transform.position.y + enemy.transform.lossyScale.y, enemy.transform.position.z);
                    Vector3 tempPos = new Vector3(enemy.transform.position.x, enemy.transform.position.y + (enemy.transform.lossyScale.y * 0.5f), enemy.transform.position.z) + enemy.transform.forward * 0.5f;
                    GameObject rangedProjectile = enemy.ShootProjectile(enemy.target.GetChild(0).position - tempPos, projectileSpeed);
                    rangedProjectile.GetComponent<EnemyProjectile>().knockback = enemy.specialKnockback;
                    projectileLaunched = true;
                }

                //Debug.Log("ATTACK");

            }
            else if (timer >= frontSwing + attackTime && timer < frontSwing + attackTime + backSwing)       //Back swing of the animation
            {
                enemy.specialRecovering = true;
                enemy.specialAttackStart = false;
                /*enemy.specialSwing = false;
                if (!enemy.specialBackSwing)
                    enemy.specialBackSwing = true;*/

                //if (enemy.IndicatorTemp)
                    //enemy.IndicatorTemp.GetComponent<Renderer>().material.color = Color.blue;
                

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
