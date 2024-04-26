using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StandardContent : Content
{
    public string gameObject;
    public float vector3x;
    public float vector3y;
    public float vector3z;
    public float value;
    public bool boolean;
    public string text;

    public StandardContent(GameObject gameObject, Vector3 vector3, float value, bool boolean, string text)
    {
        this.gameObject = gameObject.name;
        this.vector3x = vector3.x;
        this.vector3y = vector3.y;
        this.vector3z = vector3.z;
        this.value = value;
        this.boolean = boolean;
        this.text = text;
    }
}
