using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class General : MessageClient
{
    public General(TCPClientTopic client) : base(client)
    {
    }

    public void StartScriptRunner(string PepperIP)
    {
        PythonScriptRunner psr = new PythonScriptRunner();
        psr.RunServer();

        StartIDE(PepperIP);
    }

    public void StartIDE(string PepperIP)
    {
        client.SubscribeToMessageReceived(this);

        client.ConnectToTcpServer();

        Thread subscribeThread = new Thread(() => SendConnectCommand());
        subscribeThread.Start();
    }

    private void SendConnectCommand()
    {
        while (!client.DATA_THREAD_STARTED)
        {
            continue;
        }
        client.SendMessage("connect");
    }

    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();
        NotifySubscribers(ref message);
    }

    protected override void NotifySubscribers<T>(ref T message)
    {
        foreach(TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref message);
        }
    }
}
