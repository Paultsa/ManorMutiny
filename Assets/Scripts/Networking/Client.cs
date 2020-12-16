using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

//asd
public class Client
{
    private string ip; // Client's ip
    private int port; // Client's port

    //private string serverIP; // Server's port
    //private int serverPort; // Server's port

    public bool IsConnected { get; set; } = false;
    public bool IsRunning { get; set; } = true;

    public byte Id { get; set; } // For identifying this specific client on the server side
    public byte RoomId { get; set; }

    public UdpClient socket;
    public IPEndPoint endPoint; // ..."end point of the server"???

    private static Client instance = null;

    MessageHandler messageHandler;
    
    Client()
    {
        messageHandler = new MessageHandler();
    }
    ~Client()
    {
        Debug.Log("Client destroyed");
    }

    public static Client Instance
    {
        get
        {
            if (instance == null)
                instance = new Client();

            return instance;
        }
    }

    public bool ConnectTo(
        string serverIP, int serverPort, int clientPort, 
        NetworkingCommon.ConnectArgs connArgs = NetworkingCommon.ConnectArgs.DirectConnectRequest,
        string roomName = "Default",
        string userName = "Username"
        )
    {
        Debug.Log("Attempting to create udp socket. ServerIP = " + serverIP + " Server port = " + serverPort + " Client port =" + clientPort);
        try
        {
            endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            Debug.Log("endpoint address family : " + endPoint.AddressFamily.ToString());
            
            socket = new UdpClient(clientPort);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            SendConnectRequest(connArgs, roomName, userName);
            
            return true;
        }
        catch (SocketException e)
        {
            string message = "Failed to create udp socket\n" + e.ToString();
            MultiplayerGUI.ErrorMessageText.text = message;
            Debug.Log(message);

            return false;
        }
    }

    public void SendConnectRequest(
        NetworkingCommon.ConnectArgs connArgs,
        string roomName,
        string userName
        )
    {
        // Attempt to begin the connection
        // #POSSIBLE_PROBLEM
        // We may want to continuously send this until we get responce, since we can never trust udp to arrive..
        byte[] userName_raw = Encoding.ASCII.GetBytes(userName);
        byte[] roomName_raw = Encoding.ASCII.GetBytes(roomName);

        ByteBuffer namesCombined = new ByteBuffer();
        namesCombined.Add(new byte[] { (byte)roomName_raw.Length });
        namesCombined.Add(roomName_raw);
        namesCombined.Add(new byte[] { (byte)userName_raw.Length });
        namesCombined.Add(userName_raw);

        // By default we ask for directly connecting..
        //Package package = new Package(1, 0, NetworkingCommon.MESSAGE_TYPE__ConnectRequest, (ushort)userName.Length, userName_raw);
        Package package = new Package(0, 0, NetworkingCommon.Convert_ConnArgs_To_Byte(connArgs), (ushort)namesCombined.Bytes.Length, namesCombined.Bytes);

        SendData(package.Buffer.Bytes);
    }

    // Sends disconnect message to server and closes the socket
    public void DisconnectFromServer()
    {
        Package package = new Package(RoomId, Id, NetworkingCommon.MESSAGE_TYPE__Disconnect);
        SendData(package.Buffer.Bytes);
        Debug.Log("Disconnected from server!");
        CloseSocket();
        IsConnected = false;
    }
    public void CloseSocket()
    {
        socket.Close();
        Debug.Log("Socket closed!");
    }

    public void SendData(byte[] data)
    {
        try
        {
            if (socket != null)
            {
                socket.BeginSend(data, data.Length, null, null);
            }
            else
            {
                string message = "Tried to send data to server but the client's socket was null!";
                MultiplayerGUI.ErrorMessageText.text = message;
                Console.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Console.Write("Failed to send data to the server!\n" + e.ToString() + "\n");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            byte[] receivedData = socket.EndReceive(result, ref endPoint);
            // When ending receive, we want to begin receiving immediately again..
            socket.BeginReceive(ReceiveCallback, null);

            // Shit quick way to determine is there even a packet to handle.. !FIX LATER!
            if (receivedData.Length < 1)
            {
                // Disconnect maybe? <- but why the fuck?...
                return;
            }

            messageHandler.HandleMessage(receivedData);
        }
        catch (Exception e)
        {
            Console.Write("Encountered exception in ReceiveCallback(IAsyncResult)\n" + e.ToString() + "\n");
            // Also, if we ever get here, we might want to disconnect!
        }
    }
    
}