using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Megaphone : MonoBehaviour
{
    // Start is called before the first frame update
    [FMODUnity.EventRef]
    public string startQuipPath = "";
    FMOD.Studio.EventInstance startQuip;
    /*[FMODUnity.EventRef]
    public string wave1QuipPath = "";
    FMOD.Studio.EventInstance wave1Quip;
    [FMODUnity.EventRef]
    public string wave2QuipPath = "";
    FMOD.Studio.EventInstance wave2Quip;
    [FMODUnity.EventRef]
    public string wave3QuipPath = "";
    FMOD.Studio.EventInstance wave3Quip;
    [FMODUnity.EventRef]
    public string gameOverQuipPath = "";
    FMOD.Studio.EventInstance gameOverQuip;*/


    /*[FMODUnity.EventRef]
    public string smallReaction1QuipPath = "";
    FMOD.Studio.EventInstance smallReaction1Quip;*/

    GameObject Small;

    WaveController WaveController;

    void Start()
    {
        //Small = GameObject.Find("Player");

        //WaveController = GameObject.Find("WaveController").GetComponent<WaveController>();
    }

    // Update is called once per frame
    void Update()
    {

       //Debug.Log(WaveController.roundTimer);
       /* if (WaveController.roundTimer > 3f && WaveController.currentRound == 0)

        {
            Debug.Log("StartQuip");
            startQuip.start();
        }*/
    }
}
