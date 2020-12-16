using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pauli
public class WeaponSpawner : MonoBehaviour
{
    public int ignoreFirstXChildren = 2;
    public GunSpawnerController gunSpawnerController;
    public int spawned;
    public bool spawn;
    public bool lastTimeSpawned = false;
    public float spawnHeight;

    //KostinKoodi
    public FMOD.Studio.EventInstance SpawnerSound;

    // Start is called before the first frame update
    void Start()
    {
        gunSpawnerController = transform.parent.GetComponent<GunSpawnerController>();
        //KostinKoodi
        SpawnerSound = FMODUnity.RuntimeManager.CreateInstance("event:/SX/Weapons/Spawn");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SpawnerSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        SpawnerSound.start();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount-ignoreFirstXChildren >= 0)
        {
            spawned = transform.childCount - ignoreFirstXChildren;

            if (spawned > 0)
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        
       
    }

    public void SpawnGun(GameObject spawnGun)
    {
        lastTimeSpawned = true;
        Vector3 spawnPoint = transform.position;
        spawnPoint = new Vector3(spawnPoint.x, transform.position.y + spawnHeight, spawnPoint.z);

        GameObject gun = Instantiate(spawnGun, spawnPoint, transform.rotation);
        gun.transform.SetParent(transform);

        //KostinKoodi
        FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Weapons/SpawnStinger", this.transform.position);

        SpawnerSound.setParameterByName("Spawned", 1);

    }

}
