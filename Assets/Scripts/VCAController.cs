using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCAController : MonoBehaviour
{

    FMOD.Studio.VCA mainVca;
    FMOD.Studio.VCA muVca;
    FMOD.Studio.VCA atVca;
    FMOD.Studio.VCA sxVca;
    FMOD.Studio.VCA voVca;

    [SerializeField]
    [Range(0, 1)]
    private float mainVcaVolume;
    
    [SerializeField]
    [Range(0, 1)]
    private float muVcaVolume;
   
    [SerializeField]
    [Range(0, 1)]
    private float atVcaVolume;
   
    [SerializeField]
    [Range(0, 1)]
    private float sxVcaVolume;
   
    [SerializeField]
    [Range(0, 1)]
    private float voVcaVolume;


    // Start is called before the first frame update
    void Start()
    {
        mainVca = FMODUnity.RuntimeManager.GetVCA("vca:/Main");
        muVca = FMODUnity.RuntimeManager.GetVCA("vca:/Mu");
        sxVca = FMODUnity.RuntimeManager.GetVCA("vca:/Sx");
        atVca = FMODUnity.RuntimeManager.GetVCA("vca:/At");
        voVca = FMODUnity.RuntimeManager.GetVCA("vca:/Vo");
    }

    public void SetMainVolume(float volume)
    { 
        mainVca.setVolume(volume);
    }
     public void SetMuVolume(float volume)
    { 
        muVca.setVolume(volume);
    }
     public void SetSxVolume(float volume)
    { 
       sxVca.setVolume(volume);
    }
     public void SetAtVolume(float volume)
    { 
        atVca.setVolume(volume);
    }
     public void SetVoVolume(float volume)
    { 
        voVca.setVolume(volume);
    }

    public float GetMainVolume()
    {
        float vol;
        mainVca.getVolume(out vol);
        return vol;
    }
    public float GetMuVolume()
    {
        float vol;
        muVca.getVolume(out vol);
        return vol;
    }
    public float GetSxVolume()
    {
        float vol;
        sxVca.getVolume(out vol);
        return vol;
    }
    public float GetAtVolume()
    {
        float vol;
        atVca.getVolume(out vol);
        return vol;
    }
    public float GetVoVolume()
    {
        float vol;
        voVca.getVolume(out vol);
        return vol;
    }
}
