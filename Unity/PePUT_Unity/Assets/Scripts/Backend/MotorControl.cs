using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorControl : MessageClient
{
    public MotorControl(TCPClientTopic client) : base(client)
    {
    }
 

    public void WakeUp()
    {
        if (!ClientExists()) return;

        MotorContent motorContent = new MotorContent(MotorContent.MOTOR_COMMAND.WAKEUP);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, motorContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void Sleep()
    {
        if (!ClientExists()) return;

        MotorContent motorContent = new MotorContent(MotorContent.MOTOR_COMMAND.SLEEP);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, motorContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }
    public void StandInit()
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.STAND_INIT);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);

        Debug.Log("Sending stand command: " + tcpContent.toJSONMessage());


        client.SendMessage(tcpContent);
    }   

    //New Methods
    public void CrouchInit()
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.CROUCH_INIT);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);
    }

    public void SitRelaxInit()
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.SIT_RELAX_INIT);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);
    }

    public void LyingBackInit()
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.LYING_BACK_INIT);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);
    }

    //Method to set the pitch of the right shoulder and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetShoulderPitchRightInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.SHOULDER_PITCH_RIGHT_INIT,             
            0f,  // Head Yaw
            0f,  // Head Pitch

            0f,  // Left Shoulder Pitch
            0f,  // Left Shoulder Roll
            0f,  // Left Elbow Yaw
            0f,  // Left Elbow Roll
            0f,  // Left Wrist Yaw
            0f,  // Left Hand

            angle,  // Right Shoulder Pitch
            0f,     // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,  // Hip Yaw Pitch Left
            0f,  // Hip Yaw Pitch Right

            0f,  // Left Hip Roll
            0f,  // Left Hip Pitch
            0f,  // Left Knee Pitch
            0f,  // Left Ankle Pitch
            0f,  // Left Ankle Roll

            0f,  // Right Hip Roll
            0f,  // Right Hip Pitch
            0f,  // Right Knee Pitch
            0f,  // Right Ankle Pitch
            0f   // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the pitch of the left shoulder and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetShoulderPitchLeftInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.SHOULDER_PITCH_LEFT_INIT,             
            0f,  // Head Yaw
            0f, // Head Pitch

            angle,  // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,     // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right: Hip Roll
            0f,     // Right : Hip Pitch
            0f,     // Right : Knee Pitch
            0f,     // Right : Ankle Pitch
            0f      // Right : Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the roll of the left shoulder and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetShoulderRollLeftInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.SHOULDER_ROLL_LEFT_INIT,             
            0f,  // Head Yaw
            0f,  // Head Pitch

            0f,     // Left Shoulder Pitch
            angle,  // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,     // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the roll of the right shoulder and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetShoulderRollRightInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.SHOULDER_ROLL_RIGHT_INIT,             
            0f,     // Head Yaw
            0f,     // Head Pitch

            0f,     // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            angle,  // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the roll of the left elbow and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetElbowRollLeftInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.ELBOW_ROLL_LEFT_INIT,             
            0f,     // Head Yaw
            0f,     // Head Pitch

            0f,     // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            angle,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,  // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the roll of the right elbow and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetElbowRollLRightInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.ELBOW_ROLL_RIGHT_INIT,             
            0f,     // Head Yaw
            0f,     // Head Pitch

            0f,     // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,  // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            angle,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the pitch of the head and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetHeadPitchInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.HEAD_PITCH_INIT,             
            0f,     // Head Yaw
            angle,  // Head Pitch

            0f,     // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,     // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    //Method to set the yaw of the head and send the command to the python server using the relevant enum in motor content
    //All other motors are given a 0 value to sigify no change
    public void SetHeadYawInit(float angle)
    {
        if (!ClientExists()) return;

        MotorContent content = new MotorContent(MotorContent.MOTOR_COMMAND.HEAD_YAW_INIT,             
            angle,    // Head Yaw
            0f,     // Head Pitch

            0f,     // Left Shoulder Pitch
            0f,     // Left Shoulder Roll
            0f,     // Left Elbow Yaw
            0f,     // Left Elbow Roll
            0f,     // Left Wrist Yaw
            0f,     // Left Hand
            
            0,      // Right Shoulder Pitch
            0f,     // Right Shoulder Roll
            0f,     // Right Elbow Yaw
            0f,     // Right Elbow Roll
            0f,     // Right Wrist Yaw
            0f,     // Right Hand

            0f,     // Hip Yaw Pitch Left
            0f,     // Hip Yaw Pitch Right

            0f,     // Left Hip Roll
            0f,     // Left Hip Pitch
            0f,     // Left Knee Pitch
            0f,     // Left Ankle Pitch
            0f,     // Left Ankle Roll

            0f,     // Right Hip Roll
            0f,     // Right Hip Pitch
            0f,     // Right Knee Pitch
            0f,     // Right Ankle Pitch
            0f      // Right Ankle Roll
        );

        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, content);
        client.SendMessage(tcpContent);

    }

    protected override void NotifySubscribers<T>(ref T message)
    {
        
    }

    public override void ReceiveMessage<T>(ref T message)
    {

    }
}
