using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ByteBuffer
{
    public byte[] Bytes { get; private set; }

    public ByteBuffer()
    {
        Bytes = new byte[0];
    }

    public ByteBuffer(byte[] buffer, int position, int length)
    {
        Bytes = new byte[length];
        Buffer.BlockCopy(buffer, position, Bytes, 0, length); // memcpy
    }

    public ByteBuffer(byte[] otherBytes)
    {
        Bytes = new byte[otherBytes.Length];
        Buffer.BlockCopy(otherBytes, 0, Bytes, 0, Bytes.Length); // memcpy
    }

    public byte Convert_byte(int position)
    {
        return Bytes[position];
    }
    public float Convert_float(int position)
    {
        return BitConverter.ToSingle(Bytes, position);
    }
    public int Convert_int(int position)
    {
        return BitConverter.ToInt32(Bytes, position);
    }
    public string Convert_string(int position, int stringLength)
    {
        byte[] stringBytes = new byte[stringLength];
        for (int ptr = position; ptr < position + stringLength; ptr++)
            stringBytes[ptr - position] = Bytes[ptr];

        return Encoding.ASCII.GetString(stringBytes);
    }

    public void Add(byte[] data)
    {
        // Resize our byte array
        byte[] newBytes = new byte[Bytes.Length + data.Length];
        Bytes.CopyTo(newBytes, 0);

        data.CopyTo(newBytes, Bytes.Length);

        Bytes = newBytes;
    }

    public void Add(float val)
    {
        byte[] data = BitConverter.GetBytes(val);
        // Resize our byte array
        byte[] newBytes = new byte[Bytes.Length + data.Length];
        Bytes.CopyTo(newBytes, 0);

        data.CopyTo(newBytes, Bytes.Length);

        Bytes = newBytes;
    }

    public void Add(Vector3 vec)
    {
        Add(vec.x);
        Add(vec.y);
        Add(vec.z);
    }
    public void Add(Quaternion quat)
    {
        Add(quat.x);
        Add(quat.y);
        Add(quat.z);
        Add(quat.w);
    }

    public void Add(Transform t)
    {
        Add(t.position.x);
        Add(t.position.y);
        Add(t.position.z);

        Add(t.rotation.x);
        Add(t.rotation.y);
        Add(t.rotation.z);
        Add(t.rotation.w);

        Add(t.lossyScale.x);
        Add(t.lossyScale.y);
        Add(t.lossyScale.z);
    }
}