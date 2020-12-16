using System;
using System.Collections.Generic;
using System.Text;

public static class NetworkingCommon
{
    public static readonly string DEFAULT_SERVER_APP_NAME = "TheDukeServer.exe";
    public static readonly string DEFAULT_SERVER_IP = "127.0.0.1";
    public static readonly int DEFAULT_SERVER_PORT = 30000;
    public static readonly int DEFAULT_CLIENT_PORT = 30001;
    
    public enum ConnectArgs
    {
        DirectConnectRequest,
        CreateRoomRequest,
        JoinRoomRequest
    };

    public static byte Convert_ConnArgs_To_Byte(ConnectArgs args)
    {
        switch (args)
        {
            case ConnectArgs.DirectConnectRequest: return MESSAGE_TYPE__ConnectRequest;
            case ConnectArgs.JoinRoomRequest: return MESSAGE_TYPE__ConnectRequest_NamedRoom;
            case ConnectArgs.CreateRoomRequest: return MESSAGE_TYPE__CreateRoomRequest;
            default: return MESSAGE_TYPE__ConnectRequest;
        }
    }

    public static readonly uint BUFFER_POS__HEADER__ROOM_ID = 0;   //   unsigned byte = 1 byte
    public static readonly uint BUFFER_POS__HEADER__USER_ID = 1;   //   unsigned byte = 1 byte
    public static readonly uint BUFFER_POS__HEADER__MESSAGE_TYPE = 2;   //   unsigned byte = 1 byte
    public static readonly uint BUFFER_POS__HEADER__DATA_LENGTH = 3;   //   unsigned short = 2 bytes
    public static readonly uint BUFFER_POS__HEADER__DATA_START = 5;

    /*
			Each message always contains at least 3 bytes

			0 : id of the room (byte)
			1 : id of the client (if id was assigned by the server) (byte)
			2 : message type (byte)
			3 : length in bytes of the data portion of the message (unsigned short)
		*/

    /* Message name: MESSAGE_TYPE__CreateRoomRequest
        * sent by client to attempt to create new game room to the server.
        This also is a connect request to that room

        Contains:
            0 : 0 (to become room id)
            1 : 0 (to become client id)
            2 : message type
            3 : data part length
            5 : room name length (byte)
            6 : -> room name requested
            6 + room name length : -> user name requesting the room

    */
    public const byte MESSAGE_TYPE__CreateRoomRequest = 0x9;


    public const byte MESSAGE_TYPE__Error = 0x0;

    /* Message name: MESSAGE_TYPE__ConnectRequest
        * sent by client to attempt to establish connection

        Contains:
            0 : room id
            1 : 0 (to become client id)
            2 : message type
            3 : data part length
            5 : -> user name
    */
    public const byte MESSAGE_TYPE__ConnectRequest = 0x10;

    /* Message name: MESSAGE_TYPE__ConnectRequest_NamedRoom
        * sent by client to attempt to establish connection to a specific room

        Contains:
            0 : 0 (to become room id)
            1 : 0 (to become client id)
            2 : message type
            3 : data part length
            5 : room name length
            6 : -> room name
            6 + room name length: -> user name
    */
    public const byte MESSAGE_TYPE__ConnectRequest_NamedRoom = 0x11;

    /* Message name: MESSAGE_TYPE__ServerRoomID
        * response for client's MESSAGE_TYPE__ConnectRequest_NamedRoom. Contains room id requested by the client

        Contains:
            0 : room id
            1 : 0 (to become client id)
            2 : message type
            (3 : data part length)
    */
    public const byte MESSAGE_TYPE__ServerRoomID = 0x12;

    /* Message name: MESSAGE_TYPE__ConnectConfirm
        *Sends confirmation to client that the connection request was successful

        Contains:
            0 : room id
            1 : id assigned by the server for the client
            2 : message type
    */
    public const byte MESSAGE_TYPE__ConnectConfirm = 0x13;

    /* Message name: MESSAGE_TYPE__Disconnect
        *Sent by client to disconnect from the server

        Contains:
            0 : client id to disconnect
            1 : message type
    */
    public const byte MESSAGE_TYPE__Disconnect = 0x14;

    /* 
        This was originally for testing, not currently in use
    */
    public const byte MESSAGE_TYPE__String = 0x15;

    /* Message name: MESSAGE_TYPE__StorePlayerTransform
        *Sent by client to store the client player's transform on the server

        Contains:
            0 : room id
            1 : client id
            2 : message type
            3 : transform size (should be sizeof(float) * 10)
            5 : all transform data
            ...
    */
    public const byte MESSAGE_TYPE__StorePlayerTransform = 0x16; // 

    /* Message name: MESSAGE_TYPE__ServerPlayerTransforms
        *Sent by server for each client and contains each player's transform

        Contains:
            0 : room id
            1 : 0 (sent by server to all, so no need for client id)
            2 : message type
            3 : data length (sizeof(byte) + sizeof(transform) * clientCount)
            5 : client id
            6 : beginning of transform data (order is 1.position 2.rotation 3.scale)
            ...
            sizeof(byte) + sizeof(float) * 10 : next entry of connected clients (starting again with the client's id)
            ...
    */
    public const byte MESSAGE_TYPE__ServerPlayerTransforms = 0x17;

    /* Message name: MESSAGE_TYPE__StorePlayerAttack
        *Sent by client to the server and contains data about player's attacking/shooting.
        This immediately triggers sending data back to all connected clients about the attack/shooting.

        Contains:
            0 : room id
            1 : client id
            2 : message type
            3 : data length (should be equal to 1, since this message holds only weapon type)
            5 : weapon type (1 byte)
    */
    public const byte MESSAGE_TYPE__StorePlayerAttack = 0x18;

    /* Message name: MESSAGE_TYPE__ServerPlayerAttack
        *Sent by server to all connected clients and contains info about attack that a single player did.

        Contains:
            0 : room id
            1 : attacking client id (client who did the attack)
            2 : message type
            3 : data length (should be equal to 1, since this message holds only weapon type)
            5 : attacking weapon type (1 byte)
    */
    public const byte MESSAGE_TYPE__ServerPlayerAttack = 0x19;

    /* Message name: MESSAGE_TYPE__ServerPlayerNames
        *Sent by server to all connected clients and contains each connected player's name and id.
        This is sent immediately for each connected player, when a new player connects

        Contains:
            0 : 0 (room id)
            1 : 0 (client id)
            2 : message type
            3 : data length
            5 : player1 id
            6 : player1 name length
            7 : player1 name
            x : player2 id
            ...
    */
    public const byte MESSAGE_TYPE__ServerPlayerNames = 0x20;
}