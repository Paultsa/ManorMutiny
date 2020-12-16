
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Pauli

[System.Serializable]
public class Gun
{
    public GameObject gun;
    public int chance;
}
public class GunSpawnerController : MonoBehaviour
{


    public int maxSpawnsAlive;
    WeaponSpawner[] spawners;
    bool spawning = false;

    int weaponsSpawned;

    public Gun[] guns;

    // Start is called before the first frame update
    void Start()
    {
        maxSpawnsAlive = GameManager.gameManager.activeSpawners;
        spawners = gameObject.GetComponentsInChildren<WeaponSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponsSpawned = 0;
        foreach (WeaponSpawner s in spawners)
        {
            weaponsSpawned += s.spawned;
        }
        if (weaponsSpawned < maxSpawnsAlive && !spawning)
        {
            SpawnWeapon();
        }
    }

    public void SpawnWeapon()
    {
        List<WeaponSpawner> availableSpawners = new List<WeaponSpawner>();
        foreach (WeaponSpawner spwnr in spawners)
        {
            if (spwnr.spawned == 0)
            {
                if(spwnr.lastTimeSpawned)
                {
                    spwnr.lastTimeSpawned = false;
                    continue;
                }
                availableSpawners.Add(spwnr);
            }
        }
        WeaponSpawner spawner = availableSpawners[RollRandom.RollBetween(0, availableSpawners.Count)];


        int[] weights = new int[guns.Length];
        foreach (Gun gun in guns)
        {
            weights.Append(gun.chance);
        }
        Gun g = GetRandomWeightedIndex(guns);
        spawner.SpawnGun(g.gun);
        spawning = false;
    }


    public Gun GetRandomWeightedIndex(Gun[] guns)
    {
        int totalWeight = 1;
        foreach (Gun g in guns)
        {
            totalWeight += g.chance;
        }
        int randomWeight = RollRandom.RollBetween(0, totalWeight);
        Debug.Log("Random Weight: " + randomWeight);
        int currentWeight = 0;

        foreach(Gun g in guns)
        {
            currentWeight += g.chance;
            if(randomWeight <= currentWeight)
            {
                return g;
            }
        }
        Debug.Log("Not found");
        return guns[0];
    }


}
