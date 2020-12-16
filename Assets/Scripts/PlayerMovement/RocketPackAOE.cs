using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPackAOE : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA");
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAi>().inRocketPackAoe();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA");
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAi>().inRocketPackAoe();
        }
    }
}


