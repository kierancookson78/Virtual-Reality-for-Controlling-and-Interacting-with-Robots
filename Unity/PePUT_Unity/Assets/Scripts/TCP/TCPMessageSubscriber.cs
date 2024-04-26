using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TCPMessageSubscriber
{
    void ReceiveMessage<T>(ref T message);
}
