using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnpoint : MonoBehaviour
{

    public GameObject playerPrefab;

    List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
