using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisPauseGame : MonoBehaviour
{

    public GameObject pauseMenu;
    public bool menuOnOff;

    void Awake ()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        menuOnOff = false;
        pauseMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Debug.Log("Pressed P");
            menuOnOff = !menuOnOff;

            if (menuOnOff)
            {
                //Debug.Log("Paused");
                pauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0;
            }

            else
            {
                //Debug.Log("Not paused");
                pauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1;
            }


        }
    }
}
