using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationContent : Content
{
    public string text; //command, ALAnimationPlayer, e.g. run, or ALPosture e.g., goToPosture, stopMove
    public string animationName;
    public float speed;

    public AnimationContent(string text, string animationName, float speed = 100.0f)
    {
        this.text = text.ToString();
        this.animationName = animationName.ToString();

        this.speed = speed;

    }
}
