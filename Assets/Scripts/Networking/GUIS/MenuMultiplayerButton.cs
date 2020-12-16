using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuMultiplayerButton : MonoBehaviour
{

    GameObject obj_gameManager;

    // Start is called before the first frame update
    void Start()
    {

        obj_gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_MultiplayerMenu()
    {
        SceneManager.LoadScene("ConnectToServer", LoadSceneMode.Single);
    }
}
