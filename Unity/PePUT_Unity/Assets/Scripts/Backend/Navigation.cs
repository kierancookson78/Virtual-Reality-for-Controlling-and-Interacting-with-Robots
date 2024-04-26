using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MessageClient
{
    struct NavigationInfo
    {
        public enum InfoType
        {
            IS_ACTIVE,
            MOVE_FINISHED,
            ROBOT_POSITION,
            NEXT_POSITION,
            VELOCITY,
            MOVE_CONFIG
        }

        public InfoType infoType;
        public string value;
    }
    NavigationInfo currentInfo;

    public Navigation(TCPClientTopic client) : base(client)
    {
    }

    public void MoveTo(float x, float y, float theta)
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_moveTo, x, y, theta);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void StopMove()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_stopMove);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 2, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    #region Move
    public void Move(float x, float y, float theta)
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_move, x, y, theta);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void MoveForeward(float speed = 0.1f)
    {
        Move(1f * speed, 0f, 0f);
    }

    public void MoveBackward(float speed = 0.1f)
    {
        Move(-1f * speed, 0f, 0f);
    }

    public void MoveLeft(float speed = 0.1f)
    {
        Move(0f, 1f * speed, 0f);
    }

    public void MoveRight(float speed = 0.1f)
    {
        Move(0f, -1f * speed, 0f);
    }

    public void TurnLeft(float speed = 0.1f)
    {
        Move(0f, 0f, 90f * speed);
    }

    public void TurnRight(float speed = 0.1f)
    {
        Move(0f, 0f, -90f * speed);
    }
    #endregion

    #region Test
    public void MoveUsingConfig(float x, float y, float theta)
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_move, x, y, theta, new List<MoveContent.MoveConfig>() { new MoveContent.MoveConfig(MoveContent.MoveConfig.ValueType.MaxAccXY, 0.3f) });
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void MoveToward(float x, float y, float theta)
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_toward, x, y, theta);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void IsActive()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_isActive);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void WaitMoveFinished()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_waitMoveFinished);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void GetRobotPosition()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_getRobotPosition);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void getNextPosition()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_getNextRobotPosition);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void getVelocity()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_getRobotVelocity);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void getMoveConfig()
    {
        if (!ClientExists()) return;

        MoveContent moveContent = new MoveContent(MoveContent.MOVE_COMMAND.move_getMoveConfig);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, moveContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }
    #endregion

    public void Calibrate(PlayerController playerController)
    {
        playerController.reCalibrateTrackerAngleOffset = true;
        playerController.calibrated = false;
        playerController.calibrationStarted = false;
    }

    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();
        Debug.Log(message);

        try
        {
            JsonUtility.FromJson<NavigationInfo>(message);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        NavigationInfo info = JsonUtility.FromJson<NavigationInfo>(message);
        Debug.Log("Navigation Info: " + info.infoType + " " + info.value);

        currentInfo = info;

        NotifySubscribers(ref info);
    }

    protected override void NotifySubscribers<NavigationInfo>(ref NavigationInfo navigationInfo)
    {
        foreach (TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref navigationInfo);
        }
    }
}
