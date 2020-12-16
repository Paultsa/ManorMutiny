using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontGrabTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //TODO check that player cant climb on enemies
    private void OnTriggerStay(Collider other)
    {
        transform.GetComponentInParent<PlayerMovement>().frontGrabColliding = true;
        transform.GetComponentInParent<PlayerMovement>().climbCollide(other);
        transform.GetComponentInParent<PlayerMovement>().ledgeGrabBufferFrame = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //transform.GetComponentInParent<PlayerMovement>().frontGrabColliding = false;
        //transform.GetComponentInParent<PlayerMovement>().climbCollide(null);
        transform.GetComponentInParent<PlayerMovement>().ledgeGrabBufferFrame = false;
    }
}
