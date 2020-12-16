using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


//Pauli
[System.Serializable]

public class Enemies
{
    public GameObject[] enemyUnits;

}
[System.Serializable]
public class RandomRounds
{
    public int minSpawners;
    public int maxEnemiesPerWave;
    [HideInInspector]
    public float waves;
    public int pointpool;
    public int increasePointPool;
    public int everyXRoundBoss;
    public GameObject[] randomForced;
    public int firstForcedAmount;

}
[System.Serializable]
public class Rounds
{
    public float spawnersActive;
    public float waves;
    public int pointpool;
    public int maxCost;
    public GameObject[] forcedSpawns;
}
public class WaveController : MonoBehaviour
{
    public int healBetweenWaves;
    bool gameManagerTimerSet = true;
    [HideInInspector]
    public List<GameObject> spawnables;
    [HideInInspector]
    public TMP_Text startCounter;
    bool doneOnce = false;
    [HideInInspector]
    public int[] chosenSpawners;
    public float spawnersNearPlayerPct;
    [HideInInspector]
    public GameManager gameManager;
    public int randomizeOnRound;
    public bool checkToStart;
    bool gameHasStarted;
    [HideInInspector]
    public int currentRound;
    [HideInInspector]
    public int alive;
    public float timeBetweenWaves;
    public float timeBetweenRounds;
    [HideInInspector]
    public float roundTimer = 0;
    [HideInInspector]
    public int alreadyChosen = 0;

    [HideInInspector]
    public int pointsLeft;

    public Enemies[] enemiesByCost;
    public EnemySpawner[] spawners;
    public Rounds[] rounds;
    public RandomRounds randomRounds;

    public bool isMultiplayerScene = false;

    bool enableRunning = true;

    //KostinKoodi
    public string megaphonePath = "event:/VO/MEGAPHONE/VoiceLineMegaphone";
    FMOD.Studio.EventInstance megaphone;

    public GameObject megaphoneObject;

    public FMODUnity.StudioEventEmitter musicObject;
   


    // Start is called before the first frame update
    void Start()
    {
        spawners = gameObject.GetComponentsInChildren<EnemySpawner>();
        gameManager = GameManager.gameManager;
        gameManager.gameStartCounter = gameManager.waitBeforeGameStarts;
        gameManager.startCounter = startCounter;
        gameManager.gameCounterText = gameManager.gameStartText;

        isMultiplayerScene = GameObject.Find("MultiplayerSceneHandler") != null;

        //KostinKoodi
        GameObject tempObj = GameObject.Find("Music");
        if(tempObj != null)
        {
            musicObject = tempObj.GetComponent<FMODUnity.StudioEventEmitter>();
        }
        
       
        megaphoneObject = GameObject.Find("VoiceLineMegaphone");
        megaphone = FMODUnity.RuntimeManager.CreateInstance(megaphonePath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(megaphone, megaphoneObject.transform, megaphoneObject.GetComponent<Rigidbody>());
        


    }

    // Update is called once per frame
    void Update()
    {
        // If this is multiplayer scene -> wait for both players to be connected
        if (isMultiplayerScene)
            enableRunning = gameManager.playerOne != null && gameManager.playerTwo != null;
        else
            enableRunning = true;


        if (enableRunning)
            UpdateWaves();
    }

    // *NEW 
    // Siirsin kaiken Update() kontentin tähän funktioon 
    void UpdateWaves()
    {
        gameManager.enemiesAlive = alive;
        gameManager.roundsBeaten = currentRound;
        if (gameManager.gameStartCounter <= 0 && !doneOnce)
        {
            doneOnce = true;
            checkToStart = true;

        }
        if (checkToStart)
        {
            checkToStart = false;
            StartNewRound();
            gameHasStarted = true;
        }
        if (gameHasStarted)
        {
            alive = 0;
            foreach (EnemySpawner s in spawners)
            {
                alive += s.alive;
            }
            if (alive == 0 && spawnables.Count == 0)
            {
                if (!gameManagerTimerSet)
                {
                    gameManager.gameStartCounter = timeBetweenRounds;
                    gameManager.gameCounterText = gameManager.nextWaveText;
                    gameManagerTimerSet = true;
                    if (gameManager.playerOne != null)
                    { 
                        if (gameManager.playerOne.GetComponent<PlayerHealth>().alive) 
                        { 
                            gameManager.playerOne.GetComponent<PlayerHealth>().HealDamage(healBetweenWaves); 
                        } 
                    }
                    if (gameManager.playerTwo != null) 
                    { 
                        if (gameManager.playerTwo.GetComponent<PlayerHealth>().alive)
                        { 
                            gameManager.playerTwo.GetComponent<PlayerHealth>().HealDamage(healBetweenWaves); 
                        } 
                    }
                    gameManager.RespawnPlayers();
                }
                roundTimer += Time.deltaTime;
                if (roundTimer >= timeBetweenRounds)
                {
                    currentRound++;
                    StartNewRound();
                    roundTimer = 0;
                }
            }
            else
            {
                gameManagerTimerSet = false;

                roundTimer = 0;
            }

            if (currentRound > gameManager.roundsForGameover && gameManager.roundsForGameover != 0)
            {
                gameHasStarted = false;
                gameManager.GameOver();
            }
        }
    }

    void StartNewRound()
    {

        if (randomizeOnRound != 0 && randomizeOnRound <= currentRound + 1)
        {
            chosenSpawners = new int[Random.Range(randomRounds.minSpawners, spawners.Length)];
        }
        else
        {
            chosenSpawners = new int[(int)rounds[currentRound].spawnersActive];
        }

        if (chosenSpawners.Length != spawners.Length)
        {
            for (int i = 0; i < chosenSpawners.Length; i++)
            {
                chosenSpawners[i] = -1;
            }

            alreadyChosen = Mathf.CeilToInt(spawnersNearPlayerPct * gameManager.playerCount / 100 * chosenSpawners.Length);
            Debug.Log("AreadyChosen: " + alreadyChosen);
            GameObject[] nearPlayer = new GameObject[alreadyChosen];
            List<float> temp = new List<float>();
            for (int i = 0; i < spawners.Length; i++)
            {
                if(gameManager.playerOne != null) // *NEW
                    temp.Add((spawners[i].gameObject.transform.position - gameManager.playerOne.transform.position).magnitude);
            }
            temp.Sort();
            foreach (float f in temp)
            {
                Debug.Log("Temp: " + f);
            }

            if (gameManager.playerCount == 2)
            {

                for (int i = 0; i <= alreadyChosen / 2; i++)
                {
                    for (int j = 0; j < spawners.Length; j++)
                    {
                        if (gameManager.playerOne != null) // *NEW
                        {
                            if (temp[i] >= (spawners[j].gameObject.transform.position - gameManager.playerOne.transform.position).magnitude)
                            {
                                if (!chosenSpawners.Contains(j))
                                {
                                    //temp[i] = (spawners[j].gameObject.transform.position - gameManager.playerOne.transform.position).magnitude;
                                    chosenSpawners[i] = j;
                                }
                            }
                        }
                    }
                }
                temp = new List<float>();
                for (int i = 0; i < spawners.Length; i++)
                {
                    if(gameManager.playerTwo != null) // *NEW
                        temp.Add((spawners[i].gameObject.transform.position - gameManager.playerTwo.transform.position).magnitude);
                }
                temp.Sort();
                for (int i = 0 + alreadyChosen / 2; i < alreadyChosen; i++)
                {
                    for (int j = 0; j < spawners.Length; j++)
                    {
                        if (gameManager.playerTwo != null) // *NEW
                        {
                            if (temp[i] >= (spawners[j].gameObject.transform.position - gameManager.playerTwo.transform.position).magnitude)
                            {
                                if (!chosenSpawners.Contains(j))
                                {
                                    //temp[i] = (spawners[j].gameObject.transform.position - gameManager.playerTwo.transform.position).magnitude;
                                    chosenSpawners[i] = j;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < alreadyChosen; i++)
                {
                    for (int j = 0; j < spawners.Length; j++)
                    {
                        if (gameManager.playerOne != null) // *NEW
                        {
                            if (temp[i] >= (spawners[j].gameObject.transform.position - gameManager.playerOne.transform.position).magnitude)
                            {
                                if (!chosenSpawners.Contains(j))
                                {
                                    //temp[i] = (spawners[j].gameObject.transform.position - gameManager.playerOne.transform.position).magnitude;
                                    chosenSpawners[i] = j;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0 + alreadyChosen; i < chosenSpawners.Length; i++)
            {
                int rand;
                do
                {
                    rand = RollRandom.RollBetween(0, spawners.Length);
                }
                while (chosenSpawners.Contains(rand));
                chosenSpawners[i] = rand;
            }
        }
        else
        {
            for (int i = 0; i < chosenSpawners.Length; i++)
            {
                chosenSpawners[i] = i;
            }
        }

        //KostinKoodi
        if (megaphoneObject != null)
        {
            megaphone.setParameterByName("RoundVoiceLine", currentRound);
            megaphone.start();
            Debug.Log("MegaphoneSOUND" + currentRound);
        }
        else
        {
            Debug.Log("Megaphone not found");
        }
        if (musicObject != null)
        {
            if (currentRound < 4)
            {
                musicObject.SetParameter("Music Stage", currentRound);
            }
            if (currentRound == 4)
            {
                musicObject.SetParameter("Music Intensity", 1);
            }
        }
        StartCoroutine(SpawnWaves(chosenSpawners));
    }

    IEnumerator SpawnWaves(int[] chosenSpawners)
    {
        spawnables = new List<GameObject>();



        //Define the points left to use on "buying" enemies
        pointsLeft = 0;

        if (randomizeOnRound != 0 && randomizeOnRound <= currentRound + 1)
        {
            randomRounds.pointpool += randomRounds.increasePointPool;
            pointsLeft = randomRounds.pointpool;
            if(gameManager.multiplyPointpool > 0) pointsLeft= Mathf.CeilToInt(pointsLeft * gameManager.multiplyPointpool);
        }
        else
        {
            pointsLeft = rounds[currentRound].pointpool;
            if (gameManager.multiplyPointpool > 0) pointsLeft = Mathf.CeilToInt(pointsLeft * gameManager.multiplyPointpool);
        }


        while (pointsLeft > 0)
        {
            if (pointsLeft == 1)
            {
                spawnables.Add(enemiesByCost[0].enemyUnits[RollRandom.RollBetween(0, enemiesByCost[0].enemyUnits.Length)]);
                pointsLeft -= 1;
                Debug.Log("Points Left: " + pointsLeft);
                break;
            }
            int pointsCost;
            if (randomizeOnRound != 0 && randomizeOnRound <= currentRound + 1)
            {
                pointsCost = RollRandom.RollBetween(0, enemiesByCost.Length);
                if (pointsLeft < enemiesByCost.Length) { pointsCost = RollRandom.RollBetween(0, pointsLeft); }
            }
            else
            {
                pointsCost = RollRandom.RollBetween(0, rounds[currentRound].maxCost);
                if (pointsLeft < rounds[currentRound].maxCost) { pointsCost = RollRandom.RollBetween(0, pointsLeft); }
            }
            if (enemiesByCost[pointsCost].enemyUnits.Count() != 0)
            {
                pointsLeft -= (pointsCost + 1);
                GameObject[] chosenPrice = enemiesByCost[pointsCost].enemyUnits;
                spawnables.Add(chosenPrice[RollRandom.RollBetween(0, chosenPrice.Length)]);
            } 
        }
        if (randomizeOnRound != 0 && randomizeOnRound <= currentRound + 1)
        {
            if (randomRounds.everyXRoundBoss != 0 && currentRound % randomRounds.everyXRoundBoss == 0)
            {
                for (int i = 0; i < randomRounds.firstForcedAmount; i++)
                {
                    if(randomRounds.randomForced.Length != 0)
                    {
                        spawnables.Add(randomRounds.randomForced[RollRandom.RollBetween(0, randomRounds.randomForced.Length)]);
                    }
                }
                randomRounds.firstForcedAmount++;
            }
        }
        else
        {
            foreach (GameObject forced in rounds[currentRound].forcedSpawns)
            {
                spawnables.Add(forced);
            }
        }

        spawnables = Shuffle(spawnables);
        //Add all forced spawns to the list

        int spawnsAmount;
        int waves;
        if (randomizeOnRound != 0 && randomizeOnRound <= currentRound + 1)
        {
            do
            {
                randomRounds.waves++;
                spawnsAmount = Mathf.CeilToInt(spawnables.Count / randomRounds.waves / chosenSpawners.Length);
            }
            while (spawnsAmount / chosenSpawners.Length > randomRounds.maxEnemiesPerWave);
            waves = (int)randomRounds.waves;
        }
        else
        {
            spawnsAmount = Mathf.CeilToInt(spawnables.Count / rounds[currentRound].waves / chosenSpawners.Length);
            waves = (int)rounds[currentRound].waves;
        }

        for (int i = 0; i < waves; i++)
        {
            foreach (int chosen in chosenSpawners)
            {
                if (spawnables.Count > 0)
                {
                    if(chosen >= 0) // *NEW
                        spawnables = spawners[chosen].SpawnEnemy(spawnables, spawnsAmount);
                }
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    public List<GameObject> Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = RollRandom.RollBetween(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }



}


