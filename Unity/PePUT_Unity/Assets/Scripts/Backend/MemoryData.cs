using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryData : MessageClient
{
    public struct MemoryDataInfo
    {
        public string key;
        public string value;
    }

    public MemoryData(TCPClientTopic client) : base(client)
    {
    }

    public void RequestData(string key)
    {
        if (!ClientExists()) return;

        Memory_DataConent memoryDataContent = new Memory_DataConent(key);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MEMORY_DATA, 1, memoryDataContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void ExecuteFunction(Memory_DataConent.FUNCTION function)
    {
        if (!ClientExists()) return;

        Memory_DataConent memoryDataContent = new Memory_DataConent(function);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MEMORY_DATA, 1, memoryDataContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();

        try
        {
            JsonUtility.FromJson<MemoryDataInfo>(message);
        }
        catch (Exception e)
        {
            return;
        }

        MemoryDataInfo info = JsonUtility.FromJson<MemoryDataInfo>(message);
        NotifySubscribers(ref info);
    }

    protected override void NotifySubscribers<MemoryDataInfo>(ref MemoryDataInfo message)
    {
        foreach(TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref message);
        }
    }
}
