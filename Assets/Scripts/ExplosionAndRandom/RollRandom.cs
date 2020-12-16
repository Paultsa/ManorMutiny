using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RollRandom
{ 
    public static bool RollForProbability(int chance)
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);
        int roll = Random.Range(0, 101);
        if (roll <= chance)
            return true;
        return false;
    }

    public static int RollBetween(int min, int max)
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);
        int roll = Random.Range(min, max);
        return roll;
    }

}
