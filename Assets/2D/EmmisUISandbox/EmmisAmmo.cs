using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisAmmo : MonoBehaviour
{
    public int emmiAmmoAmount = 100;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            emmiAmmoAmount = emmiAmmoAmount - 1;
            //Debug.Log("ammo " + emmiAmmoAmount);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            emmiAmmoAmount = emmiAmmoAmount + 25;
            //Debug.Log("ammo " + emmiAmmoAmount);
        }
    }
}
