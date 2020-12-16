using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//Dartti
public class Target : MonoBehaviour
{

    public float health = 50f;

    public void TakeDamage(float amount)
    {
        Debug.Log("Damage: " + amount);
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
