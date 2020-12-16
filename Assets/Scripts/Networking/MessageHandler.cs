using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DataTypeUtils;

// *is responsible for handling received messages
public class MessageHandler
{
    public MessageHandler()
    {
    }
    
    public void HandleMessage(byte[] rawData)
    {
        byte roomID = 0;
        byte userID = 0;
        byte messageType = 0;
        ushort dataLength = 0;
        byte[] data = new byte[0];
        
        ParseRawMessage(rawData, ref roomID, ref userID, ref messageType, ref dataLength, ref data);

        if (messageType != NetworkingCommon.MESSAGE_TYPE__ServerPlayerTransforms)
            Debug.Log("Received message of type: " + messageType);

        switch (messageType)
        {
            case NetworkingCommon.MESSAGE_TYPE__ConnectConfirm:         OnMessageType_ConnectConfirm(roomID, userID, data);   break;
            
            case NetworkingCommon.MESSAGE_TYPE__Disconnect:             OnMessageType_Disconnect(userID);               break;
            case NetworkingCommon.MESSAGE_TYPE__ServerPlayerTransforms: OnMessageType_ServerPlayerTransforms(data);     break;
            case NetworkingCommon.MESSAGE_TYPE__ServerPlayerAttack:     OnMessageType_ServerPlayerAttack(userID, data); break;
            default: break;
        }
    }

    // Converts received "raw" data into our format and stores it into "outBuffer"
    // returns false if conversion fails, true otherwise
    private bool ParseRawMessage(
        byte[] rawData,
        ref byte outRoomID, ref byte outUserID, ref byte outMessageType, ref ushort outDataLength, ref byte[] outData
    )
    {
        if (rawData.Length < 3)
            return false;

        outRoomID = rawData[NetworkingCommon.BUFFER_POS__HEADER__ROOM_ID];
        outUserID = rawData[NetworkingCommon.BUFFER_POS__HEADER__USER_ID];
        outMessageType = rawData[NetworkingCommon.BUFFER_POS__HEADER__MESSAGE_TYPE];

        if (rawData.Length > 3)
        {
            uint dataPos = NetworkingCommon.BUFFER_POS__HEADER__DATA_START;
            outDataLength = rawData[NetworkingCommon.BUFFER_POS__HEADER__DATA_LENGTH];
            outData = new byte[outDataLength];
            for (uint i = 0; i < outDataLength; i++)
                outData[i] = rawData[dataPos + i];
        }

        return true;
    }

    private void OnMessageType_ConnectRequest()
    { }
    private void OnMessageType_ConnectConfirm(byte roomID, byte userID, byte[] data)
    {
        Client.Instance.IsConnected = true;
        Client.Instance.RoomId = roomID;
        Client.Instance.Id = userID;
        Console.WriteLine("Connected successfully!");

        // Extract all gameplay related init stuff from the data..
        int randomSeed = BitConverter.ToInt32(data, 0);
        MultiplayerSceneProperties.Instance.VariablesQue.Set(randomSeed);
    }
    private void OnMessageType_Disconnect(byte disconnectedUserID)
    {
        MultiplayerSceneProperties.Instance.DestroyPlayer(disconnectedUserID);
    }

    private void OnMessageType_String(ByteBuffer messageBuffer)
    { }
    
    private void OnMessageType_ServerPlayerTransforms(byte[] data)
    {
        // Size of a single player's transform's data inside the data array
        int entrySize = sizeof(byte) + sizeof(float) * 10;
        int entryCount = data.Length / entrySize;

        byte[] userIDs = new byte[entryCount];
        Vector3[] positions = new Vector3[entryCount];
        Quaternion[] rotations = new Quaternion[entryCount];
        Vector3[] scales = new Vector3[entryCount];

        int entryIndex = 0;
        for (int bufferPos = 0; bufferPos < data.Length; bufferPos += entrySize)
        {
            ByteBuffer buffer = new ByteBuffer(data, bufferPos, entrySize);
            byte userID = buffer.Bytes[0];
            float posX = buffer.Convert_float(1);
            float posY = buffer.Convert_float(1 + sizeof(float));
            float posZ = buffer.Convert_float(1 + sizeof(float) * 2);

            float quatX = buffer.Convert_float(1 + sizeof(float) * 3);
            float quatY = buffer.Convert_float(1 + sizeof(float) * 4);
            float quatZ = buffer.Convert_float(1 + sizeof(float) * 5);
            float quatW = buffer.Convert_float(1 + sizeof(float) * 6);

            float scaleX = buffer.Convert_float(1 + sizeof(float) * 7);
            float scaleY = buffer.Convert_float(1 + sizeof(float) * 8);
            float scaleZ = buffer.Convert_float(1 + sizeof(float) * 9);

            Vector3 pos = new Vector3(posX, posY, posZ);
            Quaternion rot = new Quaternion(quatX, quatY, quatZ, quatW);

            userIDs[entryIndex] = userID;
            positions[entryIndex] = pos;
            rotations[entryIndex] = rot;

            entryIndex++;
        }

        MultiplayerSceneProperties.Instance.TransformsUpdateQue.Set(userIDs, positions, rotations, scales);
    }

    
    private void OnMessageType_ServerPlayerAttack(byte userID, byte[] data)
    {
        byte weaponID = data[0];
        weapon weaponName = NetworkPlayer.CastByteToWeaponName(weaponID);

        MultiplayerSceneProperties.Instance.ActionsQue.Add(userID, PlayerActionQue.PlayerActions.Attack, weaponName);
    }
}