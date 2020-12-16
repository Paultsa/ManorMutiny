using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUtils
{
    public static GameObject FindChildObj(GameObject obj, string name)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject o = obj.transform.GetChild(i).gameObject;
            if (o.name == name)
                    return o;
        }
        return null;
    }
}
