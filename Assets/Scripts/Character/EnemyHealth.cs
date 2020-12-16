using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class EnemyHealth : MonoBehaviour, IHealth
{
    [Header("Health settings")]
    [Tooltip("The amount of health the character has")]
    public int MaxHealth = 100;
    public GameObject HealthPickup;
    public int healthDropChance = 5;
    int[] playersDamage;
    public int reAggroDamageThreshold = 5;
    EnemyAi ai;
    GameObject decalHandler;
    bool doOnce;

    //[HideInInspector]
    public int currentHealth;
    void Start()
    {
        doOnce = false;
        ai = GetComponent<EnemyAi>();
        decalHandler = GameObject.Find("DecalHandler");
        currentHealth = MaxHealth;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_MaxHealth", MaxHealth);
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Health", currentHealth);
        playersDamage = new int[2];
    }
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool HealDamage(int heal)
    {
        if (currentHealth < MaxHealth)
        {
            currentHealth += heal;
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Health", currentHealth);
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            return true;
        }
        else
            return false;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth > 0 && currentHealth > damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Health", currentHealth);
            //Debug.Log(damage + " DAMAGE TAKEN. " + currentHealth + " HEALTH LEFT");

            switch (ai.enemyType)
            {
                case EnemyAi.EnemyType.CyberDemon:

                    break;
                case EnemyAi.EnemyType.Fencer:

                    break;
                case EnemyAi.EnemyType.FlyingBlob:

                    break;
                case EnemyAi.EnemyType.SharkDog:

                    break;
            }
        }
        else
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector3 damageDealer)
    {
        if (currentHealth > 0 && currentHealth > damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            CheckDamageDealer(damageDealer, damage);
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Health", currentHealth);
            //Debug.Log(damage + " DAMAGE TAKEN. " + currentHealth + " HEALTH LEFT");

            switch (ai.enemyType)
            {
                case EnemyAi.EnemyType.CyberDemon:

                    break;
                case EnemyAi.EnemyType.Fencer:

                    break;
                case EnemyAi.EnemyType.FlyingBlob:

                    break;
                case EnemyAi.EnemyType.SharkDog:

                    break;
            }
        }
        else
        {
            Die();
        }
    }

    void CheckDamageDealer(Vector3 damageDealerPos, int damage)
    {
        Collider[] hitColliders = Physics.OverlapSphere(damageDealerPos, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PlayerOne"))
            {
                playersDamage[0] += damage;
                if (playersDamage[0] >= reAggroDamageThreshold)
                {
                    GetComponent<EnemyAi>().ReAggro(hitCollider.transform);
                    playersDamage[0] = 0;
                    playersDamage[1] = 0;
                }
            }
            else if (hitCollider.CompareTag("PlayerTwo"))
            {
                playersDamage[1] += damage;
                if (playersDamage[1] >= reAggroDamageThreshold)
                {
                    GetComponent<EnemyAi>().ReAggro(hitCollider.transform);
                    playersDamage[0] = 0;
                    playersDamage[1] = 0;
                }
            }
        }
    }


    void Die()
    {
        if (!doOnce)
        {
            Debug.Log("Enemy Died");
            doOnce = true;
            if (HealthPickup)
            {
                RaycastHit groundCheck;
                if (Physics.Raycast(transform.position, Vector3.down, out groundCheck, 100, LayerMask.GetMask("Default")))
                {
                    if (RollRandom.RollForProbability(5))
                    {
                        GameObject pickup = Instantiate(HealthPickup, groundCheck.point, Quaternion.identity);
                    }
                }
            }

            GameManager.gameManager.kills++;

            //if (decalHandler != null)
            //{
            //    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.ConeTrailsLong);
            //}
            switch (ai.enemyType)
            {
                case EnemyAi.EnemyType.CyberDemon:
                    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.ConeTrailsLong);
                    Destroy(gameObject);
                    break;
                case EnemyAi.EnemyType.Mancubus:
                    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.ConeTrailsLong);
                    Destroy(gameObject);
                    break;
                case EnemyAi.EnemyType.Fencer:
                    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.ConeTrailsLong);
                    Destroy(gameObject);
                    break;
                case EnemyAi.EnemyType.SharkDog:
                    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.ConeTrailsLong);
                    Destroy(gameObject);
                    break;
                case EnemyAi.EnemyType.FlyingBlob:
                    decalHandler.GetComponent<DecalHandler>().EnemyGib(transform, BloodBurst.SphereTrailsLong);
                    Destroy(GetComponent<FlyingEnemyPosition>().parentTransform.gameObject);
                    Destroy(this.gameObject);
                    break;
            }

        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
