using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pauli
public class EnemySpawner : MonoBehaviour
{
    [HideInInspector]
    public WaveController waveController;
    public int alive;
    public bool spawn;
    public bool lastTimeSpawned = false;
    public float spawnSpreadFactor;

    public float flyingSpawnHeight;
    public float groundSpawnHeight;


    // Start is called before the first frame update
    void Start()
    {
        waveController = transform.parent.GetComponent<WaveController>();
    }

    // Update is called once per frame
    void Update()
    {
        alive = transform.childCount;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, spawnSpreadFactor);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x, transform.position.y + flyingSpawnHeight, transform.position.z));
    }
    public List<GameObject> SpawnEnemy(List<GameObject> spawnables, int spawnAmount)
    {
        lastTimeSpawned = true;
        for (int i = 0; i < spawnAmount; i++)
        {
            if (spawnables.Count <= 0)
            {
                return spawnables;
            }
            float height = groundSpawnHeight;

            if (spawnables[0].GetComponent<SpriteDirController>() == null)
            {
                if(spawnables[0].transform.GetChild(0).GetComponent<SpriteDirController>() != null)
                {
                    if(spawnables[0].transform.GetChild(0).GetComponent<SpriteDirController>().flying)
                    {
                        height = flyingSpawnHeight;
                    }
                }
                else if(spawnables[0].transform.GetChild(0).GetChild(0).GetComponent<SpriteDirController>() != null)
                {
                    if (spawnables[0].transform.GetChild(0).GetChild(0).GetComponent<SpriteDirController>() != null)
                    {
                        if (spawnables[0].transform.GetChild(0).GetChild(0).GetComponent<SpriteDirController>().flying)
                        {
                            height = flyingSpawnHeight;
                        }
                    }
                }
            }
            Vector3 spawnPoint = transform.position + Random.insideUnitSphere * spawnSpreadFactor;
            spawnPoint = new Vector3(spawnPoint.x, transform.position.y + height, spawnPoint.z);

            GameObject enemy = Instantiate(spawnables[0], spawnPoint, transform.rotation);
            enemy.transform.SetParent(transform);
            spawnables.RemoveAt(0);
        }
        return spawnables;
    }
}
