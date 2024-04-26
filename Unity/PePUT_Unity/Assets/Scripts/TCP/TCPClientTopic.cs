using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

//The initial TCP implementation is based on the code from this source:
//https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb

public class TCPClientTopic
{
	public bool connectAtStartup = true;
	private bool isStartup = true;
    public bool DATA_THREAD_STARTED = false;

	public string ipAddress = "localhost";
	public int port = 65431;

	public static Queue<string> messageQueue;
	private Queue<string> outGoingMessageQueue;
	public string lastMessage = "";
	private List<TCPMessageSubscriber> subscribers = new List<TCPMessageSubscriber>();

	#region private members 	
	protected TcpClient socketConnection;
	private Thread clientReceiveThread;
	private Thread outgoingMessageDequeueThread;
	#endregion

	private string pepper_ip;
	private int pepper_port;
	private RobotAutonomyContent.AUTONOMY_MODE pepper_autonomy_mode;
	private bool connected = false;
	public TCPClientTopic(string ip, RobotAutonomyContent.AUTONOMY_MODE autonomy_mode = RobotAutonomyContent.AUTONOMY_MODE.SAFEGUARD, int port = 9559)
    {
		pepper_ip = ip;
		pepper_port = port;
		pepper_autonomy_mode = autonomy_mode;
    }

	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	public void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();

			//Queue for outgoing messages
			outGoingMessageQueue = new Queue<string>();

			outgoingMessageDequeueThread = new Thread(new ThreadStart(DequeueOutgoingMessage));
			outgoingMessageDequeueThread.IsBackground = true;
			outgoingMessageDequeueThread.Start();
			//Remove if this causes bugs

			Debug.Log("Client thread started");
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	protected void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(ipAddress, port);
            DATA_THREAD_STARTED = true;
            Byte[] bytes = new Byte[4 * 1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						lastMessage = serverMessage;
						MessageReceived(serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException.ToString().Substring(0, 146) + "Wahrscheinlich läuft der Server noch nicht.");
		}
	}

	/// <summary> 	
	/// Process message from server. 	
	/// </summary> 	
	protected void MessageReceived(string message)
	{
		Debug.Log("server message received as: " + message);

		// Subscriber pattern:
		lastMessage = message;
		try
		{
			SerializedContent textContent = JsonUtility.FromJson<SerializedContent>(message);
		}
		catch (Exception e)
		{
			Debug.Log("Received message that was not JSON\n" + message + "\n" + e.Message);
		}
		if (isStartup)
		{
			isStartup = false;
		}

        if (!connected)
        {
			Accept(message);
        }

		Debug.Log("subscriber count: " + subscribers.Count);
		for (int i = 0; i < subscribers.Count; i++)
		{
			subscribers[i].ReceiveMessage(ref message);
		}
	}

    public void SendMessage(TCPContent tcpContent)
    {
        SendMessage(tcpContent.toJSONMessage());
    }

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage(string message)
	{
		message = "~" + message + "#";

		Debug.Log(message);
		if (socketConnection == null)
		{
			Debug.Log("NO SOCKET CONNECTION");
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	public void EnqueueOutgoingMessage(string message)
	{
		outGoingMessageQueue.Enqueue(message);
	}

	private void DequeueOutgoingMessage()
	{
		while (true)
		{
			if (outGoingMessageQueue.Count > 0)
			{
				SendMessage(outGoingMessageQueue.Dequeue());
				Thread.Sleep(10); //minimum time between messages
			}
		}
	}

	public void SetPort(int port)
	{
		this.port = port;
	}

	public void SubscribeToMessageReceived(TCPMessageSubscriber subscriber)
    {
		subscribers.Add(subscriber);
    }

	public void UnubscribeToMessageReceived(TCPMessageSubscriber subscriber)
	{
		subscribers.Remove(subscriber);
	}

	void Accept(string message)
    {
		if (message == "accept")
		{
			ConnectionContent connectionContent = new ConnectionContent(pepper_ip, pepper_port, pepper_autonomy_mode);
			TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.INIT, TCPContent.SUBTOPIC.NONE, 1, connectionContent);
			SendMessage(tcpContent.toJSONMessage());

			connected = true;
		}
	}

	public bool IsConnected()
    {
		return connected;
    }
}
