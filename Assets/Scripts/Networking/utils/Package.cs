using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class Package
{
    public ByteBuffer Buffer { get; set; }
    


    public Package(byte roomID, byte clientID, byte messageType, ushort dataLength, byte[] data)
    {
        Buffer = new ByteBuffer();

        Buffer.Add(new byte[1] { roomID });
        Buffer.Add(new byte[1] { clientID });
        Buffer.Add(new byte[1] { messageType });
        Buffer.Add(BitConverter.GetBytes(dataLength));
        Buffer.Add(data);
    }

    public Package(byte roomID, byte clientID, byte messageType)
    {
        Buffer = new ByteBuffer();

        Buffer.Add(new byte[1] { roomID });
        Buffer.Add(new byte[1] { clientID });
        Buffer.Add(new byte[1] { messageType });
    }

    public Package(byte roomID, byte clientID, byte messageType, Transform t)
    {
        Buffer = new ByteBuffer();

        Buffer.Add(new byte[1] { roomID });
        Buffer.Add(new byte[1] { clientID });
        Buffer.Add(new byte[1] { messageType });
        ushort dataLength = sizeof(float) * 10;
        Buffer.Add(BitConverter.GetBytes(dataLength));
        Buffer.Add(t);
    }

    public Package(byte roomID, byte clientID, byte messageType, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        Buffer = new ByteBuffer();

        Buffer.Add(new byte[1] { roomID });
        Buffer.Add(new byte[1] { clientID });
        Buffer.Add(new byte[1] { messageType });
        ushort dataLength = sizeof(float) * 10;
        Buffer.Add(BitConverter.GetBytes(dataLength));

        Buffer.Add(pos);
        Buffer.Add(rot);
        Buffer.Add(scale);

    }
}
