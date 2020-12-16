using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerVariablesQue
{
    public int RandomSeed { get; private set; }

    // locks multithreaded updating..
    public bool Lock { get; set; } = false;
    public bool Processed { get; set; }

    public void Set(int randomSeed)
    {
        if (Lock)
            return;

        RandomSeed = randomSeed;

        Processed = false;
    }

    public void Reset()
    {
        Lock = false;
        Processed = false;
    }
}