using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy

public class GrapplingHookCollision : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Transform shooter;

    Rigidbody rb;
    //public float pullDuration = 0f;
    LayerMask layerMask;
    void Start()
    {
        //layerMask = ~layerMask;
        layerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Geometry");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    RaycastHit rope;
    void Update()
    {
        if (Physics.Raycast(transform.position, (shooter.position - transform.position), out rope, (shooter.position - transform.position).magnitude, layerMask))
        {
            //Debug.DrawRay(transform.position, (shooter.position - transform.position), Color.red, 10);
            //Debug.Log(rope.collider.gameObject);
            //Destroy(rope.collider.gameObject);
            Destroy(gameObject);
        }
        else
        {

        }
    }

    private void OnCollisionEnter(Collision collision)      //Setup ignore collision in project settings -> gravity for grappling hook to pass through unwanted objects such as enemies
    {
        shooter.GetComponent<GrapplingHook>().hookFlying = false;
        shooter.GetComponent<GrapplingHook>().hookHit = true;
        shooter.GetComponent<GrapplingHook>().durationTimer = 0f;
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Player/Small/Player_small_grapling_Hit", transform.position);
    }
    //This is for the grenade. will be removed. just a memo
    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("AddDamage");
        }
    }
    private void OnDestroy()
    {
        shooter.GetComponent<GrapplingHook>().hookHit = false;
        shooter.GetComponent<GrapplingHook>().hookFlying = false;
        shooter.GetComponent<GrapplingHook>().grapplingRope.enabled = false;
        shooter.parent.GetComponent<PlayerMovement>().grappling = false;
        shooter.parent.GetComponent<PlayerMovement>().specialAbilityDisables = false;
    }
}
