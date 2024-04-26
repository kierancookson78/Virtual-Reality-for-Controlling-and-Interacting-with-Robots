using System;
using System.Globalization;
using UnityEngine;

public class Speech : MessageClient
{
    public struct SpeechInfo
    {
        public string speech_status;
        public string current_sentence;
    }
    SpeechInfo currentInfo;

    public Speech(TCPClientTopic client) : base(client)
    {
    }

    public void StartSpeechSendAll(string text, float volume, SpeechContent.LANGUAGE language, SpeechContent.VOICE voice, float pitch, float speed, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendAll(SpeechContent.Command.SEND_ALL, priority, text, volume, language, voice, pitch, speed);
    }

    public void StartSpeech(string text, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.SPEAK, priority, text);
    }

    public void StopSpeech(TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.STOP, priority);
    }

    public void SetVolume(float value, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.VOLUME, priority, value);
    }

    public void SetLanguage(SpeechContent.LANGUAGE value, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.LANGUAGE, priority, value);
    }

    public void SetVoice(SpeechContent.VOICE value, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.VOICE, priority, value);
    }

    public void SetSpeed(float value, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.SPEED, priority, value);
    }

    public void SetPitch(float value, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.PITCH, priority, value);
    }

    public void StartSpeechSkipQueue(string text, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.SKIP_QUEUE, priority, text);
    }

    public void ClearQueue(TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        SendSpeechCommand(SpeechContent.Command.CLEAR_QUEUE, priority);
    }

    private void SendSpeechCommand(SpeechContent.Command command, TCPContent.PRIORITY priority, string text)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command, text);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    private void SendSpeechCommand(SpeechContent.Command command, TCPContent.PRIORITY priority, float value)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command, value.ToString(CultureInfo.InvariantCulture));
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    private void SendSpeechCommand(SpeechContent.Command command, TCPContent.PRIORITY priority)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    private void SendSpeechCommand(SpeechContent.Command command, TCPContent.PRIORITY priority, SpeechContent.LANGUAGE language)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command, language);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    private void SendSpeechCommand(SpeechContent.Command command, TCPContent.PRIORITY priority, SpeechContent.VOICE voice)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command, voice);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    private void SendAll(SpeechContent.Command command, TCPContent.PRIORITY priority, string text, float volume, SpeechContent.LANGUAGE language, SpeechContent.VOICE voice, float pitch, float speed)
    {
        if (!ClientExists()) return;

        SpeechContent content = new SpeechContent(command, text, volume, language, voice, pitch, speed);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, (int)priority, content);

        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void SayTextFromTextFile(string filePath, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        Debug.Log("Saying text from file: " + filePath);

        string text = FileReaderWriter.ReadFromFile(filePath);

        StartSpeech(text, priority);
    }

    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();
        try
        {
            JsonUtility.FromJson<SpeechInfo>(message);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        SpeechInfo info = JsonUtility.FromJson<SpeechInfo>(message);
        Debug.Log("Speech Info: " + info.speech_status + " " + info.current_sentence);

        currentInfo = info;

        NotifySubscribers(ref info);
    }

    public SpeechInfo GetSpeechInfo()
    {
        return currentInfo;
    }

    protected override void NotifySubscribers<SpeechInfo>(ref SpeechInfo speechInfo)
    {
        foreach (TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref speechInfo);
        }
    }
}
