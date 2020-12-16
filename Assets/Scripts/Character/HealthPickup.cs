using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 5;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerOne") || other.CompareTag("PlayerTwo"))
        {
            if (other.GetComponent<IHealth>().HealDamage(healAmount))
            {
                Destroy(gameObject);
            }
        }
    }
}
