using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    public Transform shooter;
    public GameObject explosionObj;
    public float knockback;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerOne") || other.CompareTag("PlayerTwo"))
        {
            other.GetComponent<IHealth>().TakeDamage(damage, shooter.position);
            other.transform.GetComponent<Rigidbody>().AddForce((other.transform.position - shooter.position).normalized * knockback, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerOne") || collision.collider.CompareTag("PlayerTwo"))
        {
            collision.collider.GetComponent<IHealth>().TakeDamage(damage, shooter.position);
            collision.transform.GetComponent<Rigidbody>().AddForce((collision.transform.position - shooter.position).normalized * knockback, ForceMode.Impulse);
            shooter.GetComponent<EnemyAi>().specialDamageDealt = true;
        }
        //Debug.Log("AAAAAAAAAAAAAAAAAA " + collision.gameObject);

        Instantiate(explosionObj, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
