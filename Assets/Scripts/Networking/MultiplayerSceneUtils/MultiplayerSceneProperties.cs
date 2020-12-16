
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTypeUtils;

/*
    All sorts of multiplayer scene's properties, we may want to use 'n shit, even the 
    MultiplayerSceneHandler GameObject hasn't yet been created!
*/
public class MultiplayerSceneProperties
{
    // Did the user create the server via this application?
    public bool isServerCreator = false;
    public Process serverProcess;

    // This is true after the multiplayer scene has been successfully updated once
    public bool updatedOnce = false;

    public List<byte> previousPlayerList;
    public List<byte> currentPlayerList;

    public List<Pair<byte, GameObject>> players;
    public TransformUpdateQue TransformsUpdateQue { get; set; }
    public PlayerActionQue ActionsQue { get; set; }
    public ServerVariablesQue VariablesQue { get; set; }

    private static MultiplayerSceneProperties instance = null;
    public static MultiplayerSceneProperties Instance
    {
        get
        {
            if (instance == null)
                instance = new MultiplayerSceneProperties();
            return instance;
        }
    }

    // FUCKING SHIT -> FIX LATERR!
    public bool IsMultiplayerGame()
    {
        return GameObject.Find("MultiplayerSceneHandler") != null;
    }

    MultiplayerSceneProperties()
    {
        players = new List<Pair<byte, GameObject>>();
        TransformsUpdateQue = new TransformUpdateQue();
        ActionsQue = new PlayerActionQue();
        VariablesQue = new ServerVariablesQue();
    }

    // Adds player without actually creating the player yet
    public void AddPlayerSlot(byte userID)
    {
        // If this user id didn't exist in the players list already -> add it as new user
        foreach (Pair<byte, GameObject> player in players)
        {
            if (player.first == userID)
            {
                UnityEngine.Debug.Log("Tried to add new userID to players, but the userID was already occupied! UserID = " + userID); // this should never happen!
                return;
            }
        }
        players.Add(new Pair<byte, GameObject>(userID, null));

        UnityEngine.Debug.Log("New user connected successfully to the server! Current user count is: " + players.Count);

        MultiplayerSceneHandler.ShouldUpdateLocalPlayersList = true;
    }

    // Sets newly connected player to either player 1 or player 2
    public void AssignNewPlayer(int playerIndex)
    {
        if (playerIndex > players.Count)
        {
            UnityEngine.Debug.Log("Tried to access player non existing player index(" + playerIndex);
            return;
        }
        Pair<byte, GameObject> p = players[playerIndex];
        
        if (p.second == null) // Don't even try to reassign already assigned player
        {
            GameObject playerContainer = null;
            if (p.first == 1)
                playerContainer = GameObject.Find("PlayerContainer1");
            else
                playerContainer = GameObject.Find("PlayerContainer2");

            if (playerContainer == null)
            {
                UnityEngine.Debug.Log("Tried to assing player " + playerIndex + ", failed to find PlayerContainer for it!");
                return;
            }

            playerContainer.SetActive(true);

            GameObject playerObj = ObjectUtils.FindChildObj(playerContainer, "Player");
            
            // Each player in multiplayer needs to have NetworkPlayer script
            NetworkPlayer nwPlayer = playerContainer.GetComponent<NetworkPlayer>();
            nwPlayer.ID = p.first;
            // Is this our local client's id? 
            //      -> if it is it means we control this player. We also want to enable some stuff for local player
            //      This function also flags all other players to "non local" in this function (since all players were instantiated in the creation of this scene...)
            if (Client.Instance.Id == p.first)
                nwPlayer.StartLocal();

            p.second = playerContainer;
        }
    }

    // Creates player game object only for player slots that doesn't yet have a GameObject 
    // (Doesnt recreate all the player objects, if many players, but only one is without GameObject yet for example)
    /*public void SpawnNewPlayers(Transform spawnpoint, GameObject playerContainerPrefab)
    {
        foreach (Pair<byte, GameObject> p in players)
        {
            if (p.second == null)
            {
                GameObject playerContainer = GameObject.Instantiate(playerContainerPrefab);
                GameObject playerObj = FindActiveChild(playerContainer, "Player");
                GameObject cameraObj = FindActiveChild(playerObj, "Main Camera");
                GameObject gunObj = FindActiveChild(cameraObj, "Gun");

                // Disable gun for now, because it causes errors...
                //gunObj.SetActive(false);

                // Enable mesh renderer for now, so we can see other players..
                playerObj.GetComponent<MeshRenderer>().enabled = true;

                Transform playerTransform = playerObj.transform;
                // just quick hack so players doesnt spawn inside each others..
                playerTransform.position = new Vector3(
                    spawnpoint.position.x + players.Count * 3,
                    spawnpoint.position.y,
                    spawnpoint.position.z
                );

                // Each player in multiplayer needs to have NetworkPlayer script
                playerContainer.AddComponent<NetworkPlayer>();
                NetworkPlayer nwPlayer = playerContainer.GetComponent<NetworkPlayer>();
                nwPlayer.Init();
                nwPlayer.ID = p.first;
                // Is this our local client's id? -> if it is it means we control this player..
                if (Client.Instance.Id == p.first)
                {
                    nwPlayer.IsLocalPlayer = true;
                    nwPlayer.AttachLocalAnimComponentWebHELL();
                }
                else
                {
                    nwPlayer.IsLocalPlayer = false;
                    // Disable some stuff, for non local players
                    playerContainer.GetComponentInChildren<PlayerMovement>().enabled = false;
                    playerContainer.GetComponentInChildren<MouseLook>().enabled = false;

                    cameraObj.GetComponent<Camera>().enabled = false; //  *We do need the camera "GameObject"
                }

                p.second = playerContainer;
            }
        }
    }*/


    // Destroys player and removes it from the players list
    public void DestroyPlayer(byte userID)
    {
        foreach (Pair<byte, GameObject> p in players)
        {
            if (p.first == userID)
            {
                GameObject playerObj = p.second;
                if (playerObj != null)
                    GameObject.Destroy(playerObj);

                players.Remove(p);

                MultiplayerSceneHandler.ShouldUpdateLocalPlayersList = true;

                return;
            }
        }
    }

    public Pair<byte, GameObject> GetPlayer(byte userID)
    {
        return players.Find(x => x.first == userID);
    }

    public void Reset()
    {
        isServerCreator = false;
        updatedOnce = false;

        previousPlayerList.Clear();
        currentPlayerList.Clear();

        players.Clear();
        TransformsUpdateQue.Reset();
        ActionsQue.Reset();
        VariablesQue.Reset();
    }
}
