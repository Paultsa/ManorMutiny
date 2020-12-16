using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileWallCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(transform.parent.gameObject);
    }
}
