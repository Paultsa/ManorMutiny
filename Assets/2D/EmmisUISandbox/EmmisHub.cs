using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmmisHub : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text ammoText;


    public EmmisHP healthScript;
    int textHealth;
    public EmmisAmmo ammoScript;
    int textAmmo;


    void Start()
    {

        textHealth = healthScript.emmiHealthAmount;
        textAmmo = ammoScript.emmiAmmoAmount;


        healthText.text = "HP: " + textHealth;
        ammoText.text = "Ammo: " + textAmmo;
       
    }

    void Update()
    {
        textHealth = healthScript.emmiHealthAmount;
        textAmmo = ammoScript.emmiAmmoAmount;
        healthText.text = "HP: " + textHealth;
        ammoText.text = "Ammo: " + textAmmo ;

    }
   
}
