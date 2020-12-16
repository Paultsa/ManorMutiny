using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to store data from server about connected users
public class TransformUpdateQue
{
    public byte[] userIDs;
    public Vector3[] positions;
    public Quaternion[] rotations;
    public Vector3[] scales;

    // locks multithreaded updating..
    public bool Lock { get; set; } = false;
    public bool Processed { get; set; } // Flag is this transform update que processed yet?

    public void Set(byte[] playerIds, Vector3[] posQue, Quaternion[] rotQue, Vector3[] scaleQue)
    {
        if (Lock)
            return;

        userIDs = playerIds;
        positions = posQue;
        rotations = rotQue;
        scales = scaleQue;

        Processed = false;
    }

    public void Reset()
    {
        Lock = false;
        Processed = false;
        userIDs = null;
        positions = null;
        rotations = null;
    }
}