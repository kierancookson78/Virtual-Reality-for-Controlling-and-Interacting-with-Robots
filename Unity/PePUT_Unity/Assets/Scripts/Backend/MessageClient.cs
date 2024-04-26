using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class MessageClient : TCPMessageSubscriber
{
    protected bool subscribed = false;
    protected TCPClientTopic client = null;
    protected List<TCPMessageSubscriber> subscribers = new List<TCPMessageSubscriber>();

    public MessageClient(TCPClientTopic client)
    {
        this.client = client;
        Thread subscribeThread = new Thread(() => TrySubscribeThread());
        subscribeThread.Start();
    }

    protected bool ClientExists()
    {
        if (client == null)
        {
            Debug.LogError("CLIENT NOT INITIALIZED");
            return false;
        }
        return true;
    }

    public abstract void ReceiveMessage<T>(ref T message);

    public void Subscribe(TCPMessageSubscriber subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(TCPMessageSubscriber subscriber)
    {
        subscribers.Remove(subscriber);
    }

    protected abstract void NotifySubscribers<T>(ref T message);

    protected void TrySubscribeThread()
    {
        while (!subscribed)
        {
            if (client == null) continue;

            if (client.IsConnected() && !subscribed)
            {
                subscribed = true;
                Debug.Log("subscribing...");
                client.SubscribeToMessageReceived(this);
            }
        }
    }
}
