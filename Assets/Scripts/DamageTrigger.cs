using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public int damageOnEnter = 10000;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("COLLISION " + other.gameObject);
        if (other.GetComponent<IHealth>() != null)
        {
            other.GetComponent<IHealth>().TakeDamage(damageOnEnter);
        }
        if (other.transform.GetChild(0).GetComponent<IHealth>() != null)
        {
            other.transform.GetChild(0).GetComponent<IHealth>().TakeDamage(damageOnEnter);
        }
    }
}
