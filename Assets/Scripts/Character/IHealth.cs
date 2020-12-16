using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public interface IHealth
{

    void TakeDamage(int damage);
    void TakeDamage(int damage, Vector3 damageDealer);
    bool HealDamage(int heal);
    int GetCurrentHealth();
}
