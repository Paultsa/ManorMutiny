using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopGrabTrigger : MonoBehaviour
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
        transform.GetComponentInParent<PlayerMovement>().topGrabColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        transform.GetComponentInParent<PlayerMovement>().topGrabColliding = false;
    }
}
