using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EmmisMenuButtons : MonoBehaviour
{
    public string sceneName = "YourScene";
 

    public void ChangeScene()
    {
        Debug.Log("Loaded scene");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        
    }

    public void OnClick()
    {

        Debug.Log("I want to close the game");
        //Application.Quit();  //en tieda toimiiko?
 
    }
}

