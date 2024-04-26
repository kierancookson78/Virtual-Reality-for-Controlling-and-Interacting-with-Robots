using UnityEngine;
using System;

[Serializable]
public class MotorContent : Content
{
    public enum MOTOR_COMMAND
    {
        SET,
        RECEIVE,
        SLEEP,
        WAKEUP,
        STAND_INIT,
        CROUCH_INIT,
        SIT_RELAX_INIT,
        LYING_BACK_INIT,
        SHOULDER_PITCH_RIGHT_INIT,
        SHOULDER_PITCH_LEFT_INIT,
        SHOULDER_ROLL_RIGHT_INIT,
        SHOULDER_ROLL_LEFT_INIT,
        ELBOW_ROLL_LEFT_INIT,
        ELBOW_ROLL_RIGHT_INIT,
        HEAD_PITCH_INIT,
        HEAD_YAW_INIT,

    }

    public string command = "";

    /**
    public MotorSubContent kp;    //knee pitch
    public MotorSubContent hipp;  //hip pitch
    public MotorSubContent hipr;  //hip roll
    public MotorSubContent headp; //head pitch
    public MotorSubContent heady; //head yaw
    public MotorSubContent spl;   //shoulder pitch left
    public MotorSubContent srl;   //shoulder roll left
    public MotorSubContent spr;   //shoulder pitch right
    public MotorSubContent srr;   //shoulder roll right
    public MotorSubContent eyl;   //elbow yaw left
    public MotorSubContent erl;   //elbow roll left
    public MotorSubContent eyr;   //elbow yaw right
    public MotorSubContent err;   //elbow roll right
    public MotorSubContent wyl;   //wrist yaw left
    public MotorSubContent wyr;   //wrist yaw right
    public MotorSubContent hl;    //hand left
    public MotorSubContent hr;    //hand right
    **/

    
    //Head
    public MotorSubContent heady; //Head Yaw
    public MotorSubContent headp; //head pitch
    //Left Arm
    public MotorSubContent spl;   //shoulder pitch left
    public MotorSubContent srl;   //shoulder roll left
    public MotorSubContent eyl;   //elbow yaw left
    public MotorSubContent erl;   //elbow roll left
    public MotorSubContent wyl;   //wrist yaw left
    public MotorSubContent hl;    //hand left
    //Right Arm
    public MotorSubContent spr;   //shoulder pitch right
    public MotorSubContent srr;   //shoulder roll right
    public MotorSubContent eyr;   //elbow yaw right
    public MotorSubContent err;   //elbow roll right
    public MotorSubContent wyr;   //wrist yaw right
    public MotorSubContent hr;    //hand right
    //Pelvis
    public MotorSubContent hipypl; //hip yaw pitch left
    public MotorSubContent hipypr; //hip yaw pitch right
    //Left Leg
    public MotorSubContent hiprl; //hip roll left
    public MotorSubContent hippl; //hip pitch left
    public MotorSubContent kpl; //knee pitch left
    public MotorSubContent apl; //ankle pitch left
    public MotorSubContent arl; //ankle roll left
    //Right Leg
    public MotorSubContent hiprr; //hip roll right
    public MotorSubContent hippr; //hip pitch right
    public MotorSubContent kpr; //knee pitch right
    public MotorSubContent apr; //ankle pitch right
    public MotorSubContent arr; //ankel roll right
    




    //public MotorSubContent kp;

    public MotorContent(
        MOTOR_COMMAND motor_command,
        /**
        float kneePitch = 0.0f, 
        float hipPitch = 0.0f,
        float hipRoll = 0.0f,
        float headPitch = 0.0f, 
        float headYaw = 0.0f,
        float shoulderPitchLeft = 0.0f,
        float shoulderRollLeft = 0.0f,
        float shoulderPitchRight = 0.0f,
        float shoulderRollRight = 0.0f,
        float elbowYawLeft = 0.0f,
        float elbowRollLeft = 0.0f,
        float elbowYawRight = 0.0f,
        float elbowRollRight = 0.0f,
        float wristYawLeft = 0.0f,
        float wristYawRight = 0.0f,
        float handLeft = 0.0f,
        float handRight = 0.0f)
        **/

        float headYaw = 0.0f,
        float headPitch = 0.0f,
        
        float shoulderPitchLeft = 0.0f,
        float shoulderRollLeft = 0.0f,
        float elbowYawLeft = 0.0f,
        float elbowRollLeft = 0.0f,
        float wristYawLeft = 0.0f,
        float handLeft = 0.0f,

        float shoulderPitchRight = 0.0f,
        float shoulderRollRight = 0.0f,
        float elbowYawRight = 0.0f,
        float elbowRollRight = 0.0f,
        float wristYawRight = 0.0f,
        float handRight = 0.0f,

        float hipYawPitchLeft = 0.0f,
        float hipYawPitchRight = 0.0f,

        float hipRollLeft = 0.0f,
        float hipPitchLeft = 0.0f,
        float kneePitchLeft = 0.0f,
        float anklePitchLeft = 0.0f,
        float ankleRollLeft = 0.0f,

        float hipRollRight = 0.0f,
        float hipPitchRight = 0.0f,
        float kneePitchRight = 0.0f,
        float anklePitchRight = 0.0f,
        float ankleRollRight = 0.0f)
        
    {
        this.command = motor_command.ToString();

        /**
        kp = new MotorSubContent(MotorSubContent.valueType.KNEE_PITCH, (float)Math.Round(kneePitch, 2));
        hipp = new MotorSubContent(MotorSubContent.valueType.HIP_PITCH, (float)Math.Round(hipPitch, 2));
        hipr = new MotorSubContent(MotorSubContent.valueType.HIP_ROLL, (float)Math.Round(hipRoll, 2));
        headp = new MotorSubContent(MotorSubContent.valueType.HEAD_PITCH, (float)Math.Round(headPitch, 2));
        heady = new MotorSubContent(MotorSubContent.valueType.HEAD_YAW, (float)Math.Round(headYaw, 2));
        spl = new MotorSubContent(MotorSubContent.valueType.SHOULDER_PITCH_LEFT, (float)Math.Round(shoulderPitchLeft, 2));
        srl = new MotorSubContent(MotorSubContent.valueType.SHOULDER_ROLL_LEFT, (float)Math.Round(shoulderRollLeft, 2));
        spr = new MotorSubContent(MotorSubContent.valueType.SHOULDER_PITCH_RIGHT, (float)Math.Round(shoulderPitchRight, 2));
        srr = new MotorSubContent(MotorSubContent.valueType.SHOULDER_ROLL_RIGHT, (float)Math.Round(shoulderRollRight, 2));
        eyl = new MotorSubContent(MotorSubContent.valueType.ELBOW_JAW_LEFT, (float)Math.Round(elbowYawLeft, 2));
        erl = new MotorSubContent(MotorSubContent.valueType.ELBOW_ROLL_LEFT, (float)Math.Round(elbowRollLeft, 2));
        eyr = new MotorSubContent(MotorSubContent.valueType.ELBOW_JAW_RIGHT, (float)Math.Round(elbowYawRight, 2));
        err = new MotorSubContent(MotorSubContent.valueType.ELBOW_ROLL_RIGHT, (float)Math.Round(elbowRollRight, 2));
        wyl = new MotorSubContent(MotorSubContent.valueType.WRIST_JAW_LEFT, (float)Math.Round(wristYawLeft, 2));
        wyr = new MotorSubContent(MotorSubContent.valueType.WRIST_JAW_RIGHT, (float)Math.Round(wristYawRight, 2));
        hl = new MotorSubContent(MotorSubContent.valueType.HAND_LEFT, (float)Math.Round(handLeft, 2));
        hr = new MotorSubContent(MotorSubContent.valueType.HAND_RIGHT, (float)Math.Round(handRight, 2));
        **/

        
        heady = new MotorSubContent(MotorSubContent.valueType.HEAD_YAW, (float)Math.Round(headYaw, 2));
        headp = new MotorSubContent(MotorSubContent.valueType.HEAD_PITCH, (float)Math.Round(headPitch, 2));

        spl = new MotorSubContent(MotorSubContent.valueType.SHOULDER_PITCH_LEFT, (float)Math.Round(shoulderPitchLeft, 2));
        srl = new MotorSubContent(MotorSubContent.valueType.SHOULDER_ROLL_LEFT, (float)Math.Round(shoulderRollLeft, 2));
        eyl = new MotorSubContent(MotorSubContent.valueType.ELBOW_YAW_LEFT, (float)Math.Round(elbowYawLeft, 2));
        erl = new MotorSubContent(MotorSubContent.valueType.ELBOW_ROLL_LEFT, (float)Math.Round(elbowRollLeft, 2));
        wyl = new MotorSubContent(MotorSubContent.valueType.WRIST_YAW_LEFT, (float)Math.Round(wristYawLeft, 2));
        hl = new MotorSubContent(MotorSubContent.valueType.HAND_LEFT, (float)Math.Round(handLeft, 2));

        spr = new MotorSubContent(MotorSubContent.valueType.SHOULDER_PITCH_RIGHT, (float)Math.Round(shoulderPitchRight, 2));
        srr = new MotorSubContent(MotorSubContent.valueType.SHOULDER_ROLL_RIGHT, (float)Math.Round(shoulderRollRight, 2));
        eyr = new MotorSubContent(MotorSubContent.valueType.ELBOW_YAW_RIGHT, (float)Math.Round(elbowYawRight, 2));
        err = new MotorSubContent(MotorSubContent.valueType.ELBOW_ROLL_RIGHT, (float)Math.Round(elbowRollRight, 2));
        wyr = new MotorSubContent(MotorSubContent.valueType.WRIST_YAW_RIGHT, (float)Math.Round(wristYawRight, 2));
        hr = new MotorSubContent(MotorSubContent.valueType.HAND_RIGHT, (float)Math.Round(handRight, 2));

        hipypl = new MotorSubContent(MotorSubContent.valueType.HIP_YAW_PITCH_LEFT, (float)Math.Round(hipYawPitchLeft, 2));
        hipypr = new MotorSubContent(MotorSubContent.valueType.HIP_YAW_PITCH_RIGHT, (float)Math.Round(hipYawPitchRight, 2));

        hiprl = new MotorSubContent(MotorSubContent.valueType.HIP_ROLL_LEFT, (float)Math.Round(hipRollLeft, 2));
        hippl = new MotorSubContent(MotorSubContent.valueType.HIP_PITCH_LEFT, (float)Math.Round(hipPitchLeft, 2));
        kpl = new MotorSubContent(MotorSubContent.valueType.KNEE_PITCH_LEFT, (float)Math.Round(kneePitchLeft, 2));
        apl = new MotorSubContent(MotorSubContent.valueType.ANKLE_PITCH_LEFT, (float)Math.Round(anklePitchLeft, 2));
        arl = new MotorSubContent(MotorSubContent.valueType.ANKLE_ROLL_LEFT, (float)Math.Round(ankleRollLeft, 2));

        hiprr = new MotorSubContent(MotorSubContent.valueType.HIP_ROLL_RIGHT, (float)Math.Round(hipRollRight, 2));
        hippr = new MotorSubContent(MotorSubContent.valueType.HIP_PITCH_RIGHT, (float)Math.Round(hipPitchRight, 2));
        kpr = new MotorSubContent(MotorSubContent.valueType.KNEE_PITCH_RIGHT, (float)Math.Round(kneePitchRight, 2));
        apr = new MotorSubContent(MotorSubContent.valueType.ANKLE_PITCH_RIGHT, (float)Math.Round(anklePitchRight, 2));
        arr = new MotorSubContent(MotorSubContent.valueType.ANKLE_ROLL_RIGHT, (float)Math.Round(ankleRollRight, 2));
        

    }
}
