using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.GetComponentInParent<PlayerMovement>().grounded);
    }

    private void OnTriggerStay(Collider other)
    {
        transform.GetComponentInParent<PlayerMovement>().grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        transform.GetComponentInParent<PlayerMovement>().grounded = false;
    }
}
