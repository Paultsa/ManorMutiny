using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pauli
public class Spin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(44 * Time.deltaTime, 32 *Time.deltaTime, 28*Time.deltaTime));
    }
}
