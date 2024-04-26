using System;

[Serializable]
public class SerializedContent
{
    public string topic;
    //public string content;
    public string gameObject;
    public float value;
    public bool boolean;
    public string text;
    public int priority;

    public SerializedContent(string topic, string gameObject, float value, bool boolean, string text, int priority) //string content,
    {
        this.topic = topic;
        //this.content = content;
        this.gameObject = gameObject;
        this.value = value;
        this.boolean = boolean;
        this.text = text;
        this.priority = priority;
    }
}
