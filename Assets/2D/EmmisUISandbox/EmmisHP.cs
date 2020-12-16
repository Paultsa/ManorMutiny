using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisHP : MonoBehaviour
{

    public int emmiHealthAmount = 10;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            emmiHealthAmount = emmiHealthAmount - 1;
            //Debug.Log("health " + emmiHealthAmount);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            emmiHealthAmount = emmiHealthAmount + 1;
            //Debug.Log("health " + emmiHealthAmount);
        }
    }
}
