using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Explosion
{


    static float upwardMultiplier = 0.5f;
    static float damageOverDistanceDivider = 2f;     //The higher this number is the less the damage will fall off over distance
    static LayerMask hitMask = LayerMask.GetMask("ExpTarget", "Default", "Geometry");
    public static void ObstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback)
    {
        RaycastHit obstructionCheck;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        float distanceFromCenter;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo") || hitCollider.CompareTag("Enemy"))
            {
                if (Physics.Raycast(center, hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center, out obstructionCheck, (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude, hitMask))
                {
                    if (hitCollider.transform == obstructionCheck.collider.transform.parent && hitCollider.GetComponent<IHealth>() != null)
                    {
                        Debug.DrawRay(center, hitCollider.transform.position - center, Color.blue, 5);
                        distanceFromCenter = (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude / damageOverDistanceDivider;
                        distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                        hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                        if (hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo"))
                            hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
                    }
                }
            }
        }
    }
    //The most basic explosion. can be copied and modified if an explosion needs to be implemented in another game
    public static void UnobstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        float distanceFromCenter;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<IHealth>() != null)
            {
                distanceFromCenter = (hitCollider.transform.position - center).magnitude / damageOverDistanceDivider;
                distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                if (hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo"))
                    hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
            }
        }
    }






    public static void ObstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback, float stunDuration)
    {
        RaycastHit obstructionCheck;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        float distanceFromCenter;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PlayerOne") || hitCollider.CompareTag("PlayerTwo") || hitCollider.CompareTag("Enemy"))
            {
                if (Physics.Raycast(center, hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center, out obstructionCheck, (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude, hitMask))
                {
                    if (hitCollider.transform == obstructionCheck.collider.transform.parent && hitCollider.GetComponent<IHealth>() != null)
                    {
                        Debug.DrawRay(center, hitCollider.transform.position - center, Color.blue, 5);
                        distanceFromCenter = (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude / damageOverDistanceDivider;
                        distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                        hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                        if (hitCollider.GetComponent<EnemyAi>() && stunDuration > 0)
                            hitCollider.GetComponent<EnemyAi>().StunEnemy(stunDuration, Vector3.zero, 0);
                        if (hitCollider.GetComponent<Rigidbody>() != null)
                            hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    public static void UnobstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback, float stunDuration)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        float distanceFromCenter;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<IHealth>() != null)
            {
                distanceFromCenter = (hitCollider.transform.position - center).magnitude / damageOverDistanceDivider;
                distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                if (hitCollider.GetComponent<EnemyAi>() && stunDuration > 0)
                    hitCollider.GetComponent<EnemyAi>().StunEnemy(stunDuration, Vector3.zero, 0);
                if (hitCollider.GetComponent<Rigidbody>() != null)
                    hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
            }
        }
    }






    public static void ObstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback, float stunDuration, bool affectPlayer)
    {
        if (affectPlayer)
        {
            ObstructedExplosionDamage(center, radius, damage, knockback, stunDuration);
        }
        else
        {
            RaycastHit obstructionCheck;
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            float distanceFromCenter;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Debug.Log("A" + hitCollider.transform.GetChild(hitCollider.transform.childCount - 1));
                    if (Physics.Raycast(center, hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center, out obstructionCheck, (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude, hitMask))
                    {
                        Debug.Log("B" + obstructionCheck.collider.transform.parent + " " + hitCollider);
                        if (hitCollider.transform == obstructionCheck.collider.transform.parent && hitCollider.GetComponent<IHealth>() != null)
                        {
                            Debug.Log("C");
                            Debug.DrawRay(center, hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center, Color.blue, 5);
                            distanceFromCenter = (hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center).magnitude / damageOverDistanceDivider;
                            distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                            hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                            if (hitCollider.GetComponent<EnemyAi>() && stunDuration > 0)
                                hitCollider.GetComponent<EnemyAi>().StunEnemy(stunDuration, Vector3.zero, 0);
                            if (hitCollider.GetComponent<Rigidbody>() != null)
                                hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(center, hitCollider.transform.GetChild(hitCollider.transform.childCount - 1).position - center, Color.blue, 5);
                    }
                }
            }
        }
    }

    public static void UnobstructedExplosionDamage(Vector3 center, float radius, int damage, float knockback, float stunDuration, bool affectPlayer)
    {
        if (affectPlayer)
        {
            UnobstructedExplosionDamage(center, radius, damage, knockback, stunDuration);
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            float distanceFromCenter;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<IHealth>() != null)
                {
                    distanceFromCenter = (hitCollider.transform.position - center).magnitude / damageOverDistanceDivider;
                    distanceFromCenter = Mathf.Clamp(distanceFromCenter, 1, 100);
                    if (!hitCollider.CompareTag("PlayerOne") && !hitCollider.CompareTag("PlayerTwo"))
                        hitCollider.GetComponent<IHealth>().TakeDamage((int)(damage / distanceFromCenter), center);
                    if (hitCollider.GetComponent<EnemyAi>() && stunDuration > 0)
                        hitCollider.GetComponent<EnemyAi>().StunEnemy(stunDuration, Vector3.zero, 0);
                    if (hitCollider.GetComponent<Rigidbody>() != null && !hitCollider.CompareTag("PlayerOne") && !hitCollider.CompareTag("PlayerTwo"))
                        hitCollider.GetComponent<Rigidbody>().AddExplosionForce(knockback, center, radius, upwardMultiplier, ForceMode.Impulse);
                }
            }
        }
    }

}
