using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEditor.Timeline;
using TMPro;

//Pauli

//Kostin koodausmuistiinpanot: AudioScenen loadaaminen, bankkien loadaaminen? (tuleeko nykäsy jos vaihtaa menu scenestä gamesceneen?)



public class GameManager : MonoBehaviour
{
    public int kills;
    public int deaths;
    public float gameTime;
    public int roundsBeaten;
    public string localPlayerName;
    public string secondPlayerName;

    public bool gameOver;
    [System.Serializable]
    public class PlayerObj
    {
        public weapon primaryWeapon;
        public weapon secondaryWeapon;
        public weapon thirdWeapon;
        public int chosenWeapon = 0;
        public int ammo;
        public int[] ammos;
        public int health;
    }
    public Vector3 spawnPointOne;
    public Vector3 spawnPointTwo;
    public bool debugMode;
    bool startCounterIsActive = true;
    [HideInInspector]
    public static GameManager gameManager;
    public float waitBeforeGameStarts;
    [HideInInspector]
    public float gameStartCounter = 0;
    [HideInInspector]
    public TMP_Text startCounter;
    [HideInInspector]
    public GameObject gameOverMenu;
    [HideInInspector]
    public string gameCounterText;
    public string gameStartText;
    public string nextWaveText;

    public bool playersFound = false;
    bool doneOnce = false;
    public int menuSceneIndex;
    public int playerCount;
    public GameObject playerOne;
    public GameObject playerTwo;

    public int difficulty;

    public float pointPoolLevelOne;
    public float pointPoolLevelTwo;
    public float pointPoolLevelThree;
    public int activeSpawnersOne;
    public int activeSpawnersTwo;
    public int activeSpawnersThree;
    public float multiplyPointpool;
    public int activeSpawners;
    public int twoPlayersMultiplyPointpool;

    public int roundsForGameover;

    public List<PlayerObj> players;

    public int enemiesAlive;

    public bool gamePaused;

    public int playersAlive;

    public GameObject localPlayer;

    public GameObject cameraOne;
    public Vector3 cameraOnePosition;
    public GameObject cameraTwo;
    public Vector3 cameraTwoPosition;

    public int healInRespawn;
    public int lives;
    public float respawnTime;
    public float respawnCounter;

    public int grapplingOrRocketOne;
    public int grapplingOrRocketTwo;

    public bool hostChoseBig;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (gameManager == null)
        {
            DontDestroyOnLoad(gameObject);
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }


       

        //Disable secondary player camera and rocketpack/grapplinghook
        players = new List<PlayerObj>();
        menuSceneIndex = 0;
        

        





    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != menuSceneIndex && !playersFound)
        {
            playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
            cameraOne = playerOne.transform.GetChild(0).gameObject;
            cameraOnePosition = cameraOne.transform.localPosition;
            players.Add(new PlayerObj());
            players[0].ammos = new int[2];
            localPlayer = playerOne;
            if (!hostChoseBig)
            {
                playerOne.transform.GetChild(0).GetComponent<GrapplingHook>().enabled = true;
                playerOne.transform.GetChild(0).GetComponent<RocketPack>().enabled = false;
                playerOne.GetComponent<AnimatorController>().UsingDukeSprite();
                grapplingOrRocketOne = 0;
            }
            else
            {
                playerOne.transform.GetChild(0).GetComponent<RocketPack>().enabled = true;
                playerOne.transform.GetChild(0).GetComponent<GrapplingHook>().enabled = false;
                playerOne.GetComponent<AnimatorController>().UsingAssistantSprite();
                grapplingOrRocketOne = 1;
            }
            if (playerCount == 2)
            {
                playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
                cameraTwo = playerTwo.transform.GetChild(0).gameObject;
                cameraTwoPosition = cameraTwo.transform.localPosition;
                players.Add(new PlayerObj());
                players[1].ammos = new int[2];
                if (hostChoseBig)
                {
                    playerTwo.transform.GetChild(0).GetComponent<GrapplingHook>().enabled = true;
                    playerTwo.transform.GetChild(0).GetComponent<RocketPack>().enabled = false;
                    playerTwo.GetComponent<AnimatorController>().UsingDukeSprite();
                    grapplingOrRocketTwo = 0;
                }
                else
                {
                    playerTwo.transform.GetChild(0).GetComponent<RocketPack>().enabled = true;
                    playerTwo.transform.GetChild(0).GetComponent<GrapplingHook>().enabled = false;
                    playerTwo.GetComponent<AnimatorController>().UsingAssistantSprite();
                    grapplingOrRocketTwo = 1;
                }
                if (localPlayer == playerOne)
                {
                    playerTwo.transform.GetChild(0).GetChild(0).GetComponent<SpriteAnimator>().enabled = false;
                    playerTwo.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    playerOne.transform.GetChild(0).GetChild(0).GetComponent<SpriteAnimator>().enabled = false;
                    playerOne.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }

            if(localPlayer != null)
            {
                playersFound = true;
            }
            
        }
        if (startCounter != null)
        {
            if (gameStartCounter >= 0)
            {
                if (!startCounterIsActive)
                {
                    startCounter.transform.parent.gameObject.SetActive(true);
                    startCounterIsActive = true;
                }
                gameStartCounter -= Time.deltaTime;
                startCounter.SetText(gameCounterText + (Mathf.CeilToInt(gameStartCounter)).ToString());
            }
            if (gameStartCounter <= 0 && startCounterIsActive)
            {
                startCounter.transform.parent.gameObject.SetActive(false);
                startCounterIsActive = false;
            }
        }


        if ((SceneManager.GetActiveScene().buildIndex != menuSceneIndex) && !doneOnce)
        {
            playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
            if (playerCount == 2)
            {
                playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
            }
            if (playerOne != null)
            {
                doneOnce = true;
            }
            gameOver = false;
        }
        if (doneOnce && SceneManager.GetActiveScene().buildIndex != menuSceneIndex)
        {
            if(!gameOver)
            {
                gameTime += Time.deltaTime;
            }
            
            if (playerCount == 1)
            {
                playersAlive = System.Convert.ToInt32(playerOne.GetComponent<PlayerHealth>().alive);
            }
            else
            {
                playersAlive = System.Convert.ToInt32(playerOne.GetComponent<PlayerHealth>().alive) + System.Convert.ToInt32(playerTwo.GetComponent<PlayerHealth>().alive);

                if (playersAlive == 0)
                {
                    GameOver();
                }
            }
            
        }

    }
    public void TemperPointpool()
    {
        switch (difficulty)
        {
            case 1:
                multiplyPointpool = pointPoolLevelOne;
                activeSpawners = activeSpawnersOne;
                break;
            case 2:
                multiplyPointpool = pointPoolLevelTwo;
                activeSpawners = activeSpawnersTwo;
                break;
            case 3:
                multiplyPointpool = pointPoolLevelThree;
                activeSpawners = activeSpawnersThree;
                break;
        }
        if (playerCount == 2)
        {
            multiplyPointpool += twoPlayersMultiplyPointpool;
        }
    }

    public void ChangeToGameScene()
    {

        AsyncOperation progressOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void ChangeDifficulty(int diff)
    {
        difficulty = diff;
    }

    public void SetPlayerCount(int count)
    {
        playerCount = count;
    }

    public void HostChose(string name)
    {
        if (name == "big")
        {
            hostChoseBig = true;
        }
        else
        {
            hostChoseBig = false;
        }
    }
    public void RespawnPlayers()
    {
        if(!playerOne.GetComponent<PlayerHealth>().alive)
        {
            playerOne.GetComponent<PlayerHealth>().HealDamage(healInRespawn);
            playerOne.transform.position = spawnPointOne;
            playerOne.GetComponent<PlayerHealth>().alive = true;
            cameraOne.transform.GetChild(0).gameObject.SetActive(true);
            cameraOne.transform.parent = playerOne.transform;
            cameraOne.transform.localPosition = cameraOnePosition;
            cameraOne.transform.SetSiblingIndex(0);
            Explosion.UnobstructedExplosionDamage(playerOne.transform.position, 20, 0, 20, 5, false);
            if(grapplingOrRocketOne == 0)
            {
                cameraOne.GetComponent<GrapplingHook>().enabled = true;
            }
            else
            {
                cameraOne.GetComponent<RocketPack>().enabled = true;
            }
        }
        if(playerCount >= 2)
        {
            if (!playerTwo.GetComponent<PlayerHealth>().alive)
            {
                playerTwo.GetComponent<PlayerHealth>().HealDamage(healInRespawn);
                playerTwo.transform.position = spawnPointTwo;
                playerTwo.GetComponent<PlayerHealth>().alive = true;
                cameraTwo.transform.GetChild(0).gameObject.SetActive(true);
                cameraTwo.transform.parent = playerTwo.transform;
                cameraTwo.transform.localPosition = cameraTwoPosition;
                cameraTwo.transform.SetSiblingIndex(0);
                
            }

            if (grapplingOrRocketTwo == 0)
            {
                cameraTwo.GetComponent<GrapplingHook>().enabled = true;
            }
            else
            {
                cameraTwo.GetComponent<RocketPack>().enabled = true;
            }

            EnemyAi[] enemies = FindObjectsOfType<EnemyAi>() as EnemyAi[];
            foreach (EnemyAi enemy in enemies)
            {
                enemy.ReAggro();
            }
        }
        
        
    }

    public void SetLocalPlayerName(string name)
    {
        if(name != string.Empty)
        {
            if (name.Length > 12)
            {
                name = name.Substring(0, 12);
            }
            localPlayerName = name;
        }
        
    }

    public void SetSecondPlayerName(string name)
    {
        if(name != string.Empty)
        {
            if(name.Length > 12)
            {
                name = name.Substring(0, 12);
            }
            secondPlayerName = name;
        }
    }
    
    public void GameOver()
    {
        gameOver = true;
        gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Time.timeScale = 0;

        LeaderboardScript leaderboard = GameObject.Find("Leaderboard").GetComponent<LeaderboardScript>();

        if (leaderboard != null)
        {
            if (playerCount > 1)
            {
                LeaderboardEntryMP entryMP = new LeaderboardEntryMP();

                entryMP.nameP1 = localPlayerName;
                entryMP.nameP2 = secondPlayerName;
                entryMP.kills = kills;
                entryMP.wave = roundsBeaten;
                entryMP.time = gameTime;

                leaderboard.AddLeaderboardEntryMP(entryMP);
            }
            else
            {
                LeaderboardEntry entrySP = new LeaderboardEntry();

                entrySP.name = localPlayerName;
                entrySP.kills = kills;
                entrySP.wave = roundsBeaten;
                entrySP.time = gameTime;

                leaderboard.AddLeaderboardEntry(entrySP);
            }
        }
        else
        {
            Debug.Log("Leaderboard prefab not found!");
        }

       // FMODUnity.RuntimeManager.GetBus("bus:/main").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
         
        
       

        Destroy(this.gameObject);
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void AddToPlayersList()
    {
        players.Add(new PlayerObj());
    }
}

