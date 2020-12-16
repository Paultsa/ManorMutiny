using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisCoinSpin : MonoBehaviour
{

    public float spinSpeed = 10f;


    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0, 0, spinSpeed * Time.deltaTime); 

    }
}
