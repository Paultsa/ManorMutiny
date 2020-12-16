using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EmmisPauseButtons : MonoBehaviour
{
    public string sceneName = "YourScene";

    void Update()
    {

    }

    public void ChangeScene()
    {
        Debug.Log("Loaded scene");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

    }



}

