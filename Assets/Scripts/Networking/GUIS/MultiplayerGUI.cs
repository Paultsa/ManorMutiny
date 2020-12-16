using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiplayerGUI : MonoBehaviour
{

    struct ConnectProperties
    {
        public string serverIP;
        public int serverPort;
        public int clientPort;
        public string userName;
        public string roomName;
        public NetworkingCommon.ConnectArgs connectArgs;
    }

    ConnectProperties connectProperties; // determines how connection is going to be established..
    
    public GameObject obj_difficultySelection;
    public GameObject obj_loadingScreen;

    public InputField inputField_serverIP;
    public InputField inputField_serverPort;
    public InputField inputField_clientPort;

    public InputField inputField_userName;
    public InputField inputField_roomName;


    public Text errorMessageText;
    public static Text ErrorMessageText;
    
    public Button button_back;

    bool isWaitingForConnection = false;
    
    bool triggeredSceneSwitch = false;
    bool triggeredBack = false;

    bool creatingNewServer = false;

    float connectRequestCooldown = 2.0f;
    float maxConnRequestCooldown = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        ErrorMessageText = errorMessageText;

        // Just to cheat, so we dont get errors from game manager.. 
        // because it attempts to "determine scene players" always if not in menu scene..
        GameManager.gameManager.playersFound = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Client.Instance.IsConnected && !triggeredSceneSwitch)
        {
            AsyncOperation progressOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            triggeredSceneSwitch = true;
            //SceneManager.LoadScene("MultiplayerScene");
        }
        else
        {
            if (MultiplayerSceneProperties.Instance.isServerCreator)
            {
                if (connectRequestCooldown <= 0.0f)
                {
                    if (!isWaitingForConnection)
                        ConnectToServer();
                }
                else
                {
                    connectRequestCooldown -= 1.0f * Time.deltaTime;
                }
            }
        }
        //else
        // drawSome "wait for connection" indicator ()
    }

    private void LateUpdate()
    {
        if(triggeredBack)
            GameManager.gameManager.playersFound = false; // always reset this back..
    }

    public void OnClick_Back()
    {
        triggeredBack = true;
        Time.timeScale = 1;
        Destroy(GameManager.gameManager);
        Destroy(GameObject.Find("Music"));
        SceneManager.LoadScene(0);
    }

    public void OnClick_SetDifficulty(int val)
    {
        GameManager.gameManager.ChangeDifficulty(val);

    }

    public void OnClick_CreateRoom()
    {
        if (isWaitingForConnection) return;
        //ConnectToServer(NetworkingCommon.ConnectArgs.CreateRoomRequest);
        StoreConnectProperties(NetworkingCommon.ConnectArgs.CreateRoomRequest);
        obj_difficultySelection.SetActive(true);
    }

    // if user is either the host..
    public void OnClick_Host_Start()
    {
        if (isWaitingForConnection) return;
        if (!creatingNewServer)
        {
            ConnectToServer();
            obj_loadingScreen.SetActive(true);
        }
        else
        {
            MultiplayerSceneProperties.Instance.serverProcess = Process.Start(new ProcessStartInfo(NetworkingCommon.DEFAULT_SERVER_APP_NAME)
            {
                Arguments = String.Format(@"""{0}""", connectProperties.serverPort.ToString())
            }
            );

            obj_loadingScreen.SetActive(true);
            MultiplayerSceneProperties.Instance.isServerCreator = true;
        }
    }

    public void OnClick_JoinRoom()
    {
        if (isWaitingForConnection) return;
        //ConnectToServer(NetworkingCommon.ConnectArgs.JoinRoomRequest);
        StoreConnectProperties(NetworkingCommon.ConnectArgs.JoinRoomRequest);
        ConnectToServer();
        obj_loadingScreen.SetActive(true);
    }

    public void OnClick_CreateNewServer()
    {
        creatingNewServer = true;
        StoreConnectProperties(NetworkingCommon.ConnectArgs.CreateRoomRequest);
        obj_difficultySelection.SetActive(true);

        /*
        Process firstProc = new Process();
        firstProc.StartInfo.FileName = "notepad.exe";
        firstProc.EnableRaisingEvents = true;

        firstProc.Start();

        firstProc.WaitForExit();
        */
    }

    private void ConnectToServer()
    {
        Client.Instance.ConnectTo(
                connectProperties.serverIP,
                connectProperties.serverPort,
                connectProperties.clientPort,
                connectProperties.connectArgs,
                connectProperties.roomName,
                connectProperties.userName
            );
        
        isWaitingForConnection = true;
    }

    private void StoreConnectProperties(NetworkingCommon.ConnectArgs connectArgs)
    {
        string serverIP = inputField_serverIP.text;
        string serverPort_str = inputField_serverPort.text.ToString();
        string clientPort_str = inputField_clientPort.text.ToString();
        string userName = inputField_userName.text.ToString();
        string roomName = inputField_roomName.text.ToString();

        if (serverIP.Length <= 0) serverIP = inputField_serverIP.placeholder.gameObject.GetComponent<Text>().text;
        if (serverPort_str.Length <= 0) serverPort_str = inputField_serverPort.placeholder.gameObject.GetComponent<Text>().text;
        if (clientPort_str.Length <= 0) clientPort_str = inputField_clientPort.placeholder.gameObject.GetComponent<Text>().text;
        if (userName.Length <= 0) userName = inputField_userName.placeholder.gameObject.GetComponent<Text>().text;
        if (roomName.Length <= 0) roomName = inputField_roomName.placeholder.gameObject.GetComponent<Text>().text;

        int serverPort = Int32.Parse(serverPort_str);
        int clientPort = Int32.Parse(clientPort_str);


        connectProperties.serverIP = serverIP;
        connectProperties.serverPort = serverPort;
        connectProperties.clientPort = clientPort;

        connectProperties.userName = userName;
        connectProperties.roomName = roomName;

        connectProperties.connectArgs = connectArgs;
    }
}
