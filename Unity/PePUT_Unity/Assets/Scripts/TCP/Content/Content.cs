using System;
using UnityEngine;

[Serializable]
public abstract class Content
{
    public string toJSONString()
    {
        return JsonUtility.ToJson(this);
    }
}
