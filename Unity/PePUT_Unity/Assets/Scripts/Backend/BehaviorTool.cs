using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTool : MessageClient
{
    public struct BehaviorInfo
    {
        public BehaviorInfo(BehaviorInfoType type, List<string> values)
        {
            this.type = type.ToString();
            this.values = values;
        }

        public enum BehaviorInfoType
        {
            BODY_TALK,
            EMOTIONS,
            GESTURES,
            REACTIONS,
            WAITING,
            MISC,
        }
        public string type;
        public List<string> values;

        public void CleanStrings()
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Contains("\'")) values[i] = values[i].Replace("\'", string.Empty);
                if (values[i].Contains("[")) values[i] = values[i].Replace("[", string.Empty);
                if (values[i].Contains("]")) values[i] = values[i].Replace("]", string.Empty);
                if (values[i].Contains(" ")) values[i] = values[i].Replace(" ", string.Empty);
            }
        }

        public void Append(List<string> values)
        {
            this.values.AddRange(values);
        }
    }
    public BehaviorInfo bodyTalkInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.BODY_TALK, null);
    public BehaviorInfo emotionsInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.EMOTIONS, null);
    public BehaviorInfo gesturesInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.GESTURES, null);
    public BehaviorInfo reactionsInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.REACTIONS, null);
    public BehaviorInfo waitingInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.WAITING, null);
    public BehaviorInfo miscInfo = new BehaviorInfo(BehaviorInfo.BehaviorInfoType.MISC, null);

    public BehaviorTool(TCPClientTopic client) : base(client)
    {
    }

    #region Sample Behavior
    public void SampleBehavior_1() //Hey
    {
        PlayBehavior("animations/Stand/Gestures/Hey_1");

    }
    public void SampleBehavior_2() //Happy
    {
        PlayBehavior("animations/Stand/Emotions/Positive/Hysterical_1");
    }
    public void SampleBehavior_3() //Embarrassed
    {
        PlayBehavior("animations/Stand/Gestures/Desperate_1");
    }
    #endregion

    public void GetBehaviorList()
    {
        BehaviorContent content = new BehaviorContent(BehaviorContent.BEHAVIOR_COMMAND.GET_BEHAVIORS);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_BEHAVIOR_TOOL, 1, content);

        Debug.Log(client);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void PlayBehavior(string behaviorName)
    {
        if (!ClientExists()) return;

        BehaviorContent content = new BehaviorContent(BehaviorContent.BEHAVIOR_COMMAND.START, behaviorName, true);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_BEHAVIOR_TOOL, 1, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void PlayBehavior(STANDARD_BEHAVIORS behavior)
    {
        PlayBehavior(behavior_lookup[behavior]);
    }

    public void StopAllBehaviors()
    {
        if (!ClientExists()) return;

        BehaviorContent content = new BehaviorContent(BehaviorContent.BEHAVIOR_COMMAND.STOP_ALL);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_BEHAVIOR_TOOL, 1, content);
        
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void StopBehavior(string behaviorName)
    {
        if (!ClientExists()) return;

        BehaviorContent content = new BehaviorContent(BehaviorContent.BEHAVIOR_COMMAND.STOP, behaviorName);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_BEHAVIOR_TOOL, 1, content);

        client.SendMessage(tcpContent);
    }

    int gestures = 0;
    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();

        try
        {
            JsonUtility.FromJson<BehaviorInfo>(message);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        BehaviorInfo input = JsonUtility.FromJson<BehaviorInfo>(message);

        if (message.Contains("BODY_TALK"))
        {
            bodyTalkInfo = input;
            bodyTalkInfo.CleanStrings();
        }
        else if (message.Contains("EMOTIONS"))
        {
            emotionsInfo = input;
            emotionsInfo.CleanStrings();
        }
        else if (message.Contains("GESTURES"))
        {
            if(gestures == 0)
            {
                gesturesInfo = input;
                gestures++;
            }
            else
            {
                gesturesInfo.Append(input.values);
            }
            gesturesInfo.CleanStrings();
        }
        else if (message.Contains("REACTIONS"))
        {
            reactionsInfo = input;
            reactionsInfo.CleanStrings();
        }
        else if (message.Contains("WAITING"))
        {
            waitingInfo = input;
            waitingInfo.CleanStrings();
        }
        else if (message.Contains("MISC"))
        {
            miscInfo = input;
            miscInfo.CleanStrings();
        }
        else
        {
            Debug.LogError("ERROR: BEHAVIOR MESSAGE INVALID");
            return;
        }

        NotifySubscribers(ref input);
    }

    protected override void NotifySubscribers<BehaviorInfo>(ref BehaviorInfo behaviorInfo)
    {
        bool allValuesFilled = false;
        if (bodyTalkInfo.values != null && emotionsInfo.values != null && gesturesInfo.values != null && reactionsInfo.values != null && waitingInfo.values != null && miscInfo.values != null) allValuesFilled = true;

        if (!allValuesFilled) return;

        foreach (TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref behaviorInfo);
        }
    }

    string behaviorPath = "pepperBehaviors";
    public void WriteGesturesToFile()
    {
        FileReaderWriter.WriteLinesToFile(behaviorPath, bodyTalkInfo.values);
        FileReaderWriter.WriteLinesToFile(behaviorPath, emotionsInfo.values);
        FileReaderWriter.WriteLinesToFile(behaviorPath, gesturesInfo.values);
        FileReaderWriter.WriteLinesToFile(behaviorPath, reactionsInfo.values);
        FileReaderWriter.WriteLinesToFile(behaviorPath, waitingInfo.values);
        FileReaderWriter.WriteLinesToFile(behaviorPath, miscInfo.values);
    }

    public List<string> ReadGesturesFromFile()
    {
        return FileReaderWriter.ReadLinesFromFile(behaviorPath);
    }

    public enum STANDARD_BEHAVIORS
    {

    }

    public Dictionary<STANDARD_BEHAVIORS, string> behavior_lookup = new Dictionary<STANDARD_BEHAVIORS, string>()
    {

    };
}
