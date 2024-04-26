using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PepperAnimationEditor;

public class MotorValueParser
{
    struct MotorValues
    {
        public string type;

        public string kneePitch;

        public string hipPitch;
        public string hipRoll;

        public string headPitch;
        public string headYaw;

        public string shoulderPitchLeft;
        public string shoulderRollLeft;
        public string shoulderPitchRight;
        public string shoulderRollRight;

        public string elbowJawLeft;
        public string elbowRollLeft;
        public string elbowJawRight;
        public string elbowRollRight;

        public string wristJawLeft;
        public string wristJawRight;

        public string handLeft;
        public string handRight;
    }

    struct Parameters
    {
        public string type;
        public string sa;//stiffnessAxis;
        public string s;//stiffness;
        public string ma;//movementAxis;
        public string mv;//movementValue;
        public string sp;//speed;
        public string st;//sleepTime;
    }

    struct Parameters2
    {
        public string type;
        public string sa;//stiffnessAxis;
        public string s;//stiffness;
        public string ma;//movementAxis;
        public string mv;//movementValue;
        public float sp;//speed;
        public string st;//sleepTime;
    }

    struct HandParameters
    {
        public string type;
        public string sa;//stiffnessAxis;
        public float s;//stiffness;
        public string ma;//movementAxis;
        public bool mv;//movementValue;
        public float sp;//speed;
        public float st;//sleepTime;
    }

    public enum valueType
    {
        KNEE_PITCH,

        HIP_PITCH,
        HIP_ROLL,

        HEAD_PITCH,
        HEAD_YAW,
        
        SHOULDER_PITCH_LEFT,
        SHOULDER_ROLL_LEFT,
        SHOULDER_PITCH_RIGHT,
        SHOULDER_ROLL_RIGHT,

        ELBOW_JAW_LEFT,
        ELBOW_ROLL_LEFT,
        ELBOW_JAW_RIGHT,
        ELBOW_ROLL_RIGHT,

        WRIST_JAW_LEFT,
        WRIST_JAW_RIGHT,

        HAND_LEFT,
        HAND_RIGHT
    }

    public string AllValuesToMessage(PepperPose pepperPose)
    {
        MotorValues motorValues;

        motorValues.type = "mv";

        motorValues.kneePitch = ValueToMessage(valueType.KNEE_PITCH, RoundToDecimals(pepperPose.pepperKneePitch, 3));

        motorValues.hipPitch = ValueToMessage(valueType.HIP_PITCH, RoundToDecimals(pepperPose.pepperHipPitch, 3));
        motorValues.hipRoll = ValueToMessage(valueType.HIP_ROLL, RoundToDecimals(pepperPose.pepperHipRoll, 3));

        motorValues.headPitch = ValueToMessage(valueType.HEAD_PITCH, RoundToDecimals(pepperPose.pepperHeadPitch, 3));
        motorValues.headYaw = ValueToMessage(valueType.HEAD_YAW, RoundToDecimals(pepperPose.pepperHeadYaw, 3));

        motorValues.shoulderPitchLeft = ValueToMessage(valueType.SHOULDER_PITCH_LEFT, RoundToDecimals(pepperPose.pepperShoulderPitchL, 3));
        motorValues.shoulderRollLeft = ValueToMessage(valueType.SHOULDER_ROLL_LEFT, RoundToDecimals(pepperPose.pepperShoulderRollL, 3));
        motorValues.shoulderPitchRight = ValueToMessage(valueType.SHOULDER_PITCH_RIGHT, RoundToDecimals(pepperPose.pepperShoulderPitchR, 3));
        motorValues.shoulderRollRight = ValueToMessage(valueType.SHOULDER_ROLL_RIGHT, RoundToDecimals(pepperPose.pepperShoulderRollR, 3));

        motorValues.elbowJawLeft = ValueToMessage(valueType.ELBOW_JAW_LEFT, RoundToDecimals(pepperPose.pepperElbowYawL, 3));
        motorValues.elbowRollLeft = ValueToMessage(valueType.ELBOW_ROLL_LEFT, RoundToDecimals(pepperPose.pepperElbowRollL, 3));
        motorValues.elbowJawRight = ValueToMessage(valueType.ELBOW_JAW_RIGHT, RoundToDecimals(pepperPose.pepperElbowYawR, 3));
        motorValues.elbowRollRight = ValueToMessage(valueType.ELBOW_ROLL_RIGHT, RoundToDecimals(pepperPose.pepperElbowRollR, 3));

        motorValues.wristJawLeft = ValueToMessage(valueType.WRIST_JAW_LEFT, RoundToDecimals(pepperPose.pepperWristJawL, 3));
        motorValues.wristJawRight = ValueToMessage(valueType.WRIST_JAW_RIGHT, RoundToDecimals(pepperPose.pepperWristJawR, 3));

        motorValues.handLeft = ValueToMessage(valueType.HAND_LEFT, RoundToDecimals(pepperPose.pepperHandLOpen, 3));
        motorValues.handRight = ValueToMessage(valueType.HAND_RIGHT, RoundToDecimals(pepperPose.pepperHandROpen, 3));

        string message = "{\"type\" : \"mv\", \"kp\":" + motorValues.kneePitch +
                ", \"hipp\":" + motorValues.hipPitch +
                ", \"hipr\":" + motorValues.hipRoll +

                ", \"headp\":" + motorValues.headPitch +
                ", \"heady\":" + motorValues.headYaw +

                ", \"spl\":" + motorValues.shoulderPitchLeft +
                ", \"srl\":" + motorValues.shoulderRollLeft +
                ", \"spr\":" + motorValues.shoulderPitchRight +
                ", \"srr\":" + motorValues.shoulderRollRight +

                ", \"ejl\":" + motorValues.elbowJawLeft +
                ", \"erl\":" + motorValues.elbowRollLeft +
                ", \"ejr\":" + motorValues.elbowJawRight +
                ", \"err\":" + motorValues.elbowRollRight +

                ", \"wjl\":" + motorValues.wristJawLeft +
                ", \"wjr\":" + motorValues.wristJawRight +

                ", \"hl\":" + motorValues.handLeft +
                ", \"hr\":" + motorValues.handRight +
            "}?";

        Debug.Log("Not Rounded Value: " + pepperPose.pepperKneePitch + " Rounded Value: " + RoundToDecimals(pepperPose.pepperKneePitch, 3));

        return message;
    }

    private float RoundToDecimals(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }

    private Parameters ValueToParameters(valueType type, float motorValue, float speed = 0.1f, float sleepTime = 3.0f)
    {
        speed = Mathf.Clamp(speed, 0.0f, 1.0f);

        Parameters p;
        p.type = "motor"; //command type for server
        p.sa = valueTypeToStiffnessAxis[type];
        p.s = 1.0f.ToString();
        p.ma = valueTypeToMovementAxis[type];
        p.mv = RoundToDecimals(motorValue, 3).ToString().Replace(',', '.');
        p.sp = RoundToDecimals(speed, 3).ToString().Replace(',', '.');
        p.st = RoundToDecimals(sleepTime, 3).ToString().Replace(',', '.');

        return p;
    }

    public string ValueToMessage(valueType type, float motorValue, float speed = 0.1f, float sleepTime = 3.0f)
    {
        speed = Mathf.Clamp(speed, 0.0f, 1.0f);

        Parameters p;
        p.type = "motor"; //command type for server
        p.sa = valueTypeToStiffnessAxis[type];
        p.s = 1.0f.ToString();
        p.ma = valueTypeToMovementAxis[type];
        p.mv = RoundToDecimals(motorValue, 3).ToString().Replace(',', '.');
        p.sp = RoundToDecimals(speed, 3).ToString().Replace(',', '.');
        p.st = RoundToDecimals(sleepTime, 3).ToString().Replace(',', '.');

        Parameters2 p2;
        p2.type = "motor"; //command type for server
        p2.sa = valueTypeToStiffnessAxis[type];
        p2.s = 1.0f.ToString();
        p2.ma = valueTypeToMovementAxis[type];
        p2.mv = RoundToDecimals(motorValue, 3).ToString();
        p2.sp = speed;
        p2.st = RoundToDecimals(sleepTime, 3).ToString().Replace(',', '.');

        Debug.Log(motorValue + " Rounded Value: " + p.sp + " JSON Value " + JsonUtility.ToJson(p2));

        return JsonUtility.ToJson(p);
    }

    public string ValueToMessage(valueType type, bool motorValue, float speed = 0.1f, float sleepTime = 3.0f)
    {
        speed = Mathf.Clamp(speed, 0.0f, 1.0f);

        HandParameters p;
        p.type = "motor"; //command type for server
        p.sa = valueTypeToStiffnessAxis[type];
        p.s = 1.0f;
        p.ma = valueTypeToMovementAxis[type];
        p.mv = motorValue; //TODO if not bool round value
        p.sp = speed;
        p.st = sleepTime;

        return JsonUtility.ToJson(p);
    }

    private Dictionary<valueType, string> valueTypeToStiffnessAxis = new Dictionary<valueType, string>() 
    {
        {valueType.KNEE_PITCH, "Knee" },

        {valueType.HIP_PITCH, "Hip" },
        {valueType.HIP_ROLL, "Hip" },

        {valueType.HEAD_PITCH, "Head" },
        {valueType.HEAD_YAW, "Head" },

        {valueType.SHOULDER_PITCH_LEFT, "LShoulder" },
        {valueType.SHOULDER_ROLL_LEFT, "LShoulder" },
        {valueType.SHOULDER_PITCH_RIGHT, "RShoulder" },
        {valueType.SHOULDER_ROLL_RIGHT, "RShoulder" },

        {valueType.ELBOW_JAW_LEFT, "LElbow" },
        {valueType.ELBOW_ROLL_LEFT, "LElbow" },
        {valueType.ELBOW_JAW_RIGHT, "RElbow" },
        {valueType.ELBOW_ROLL_RIGHT, "RElbow" },

        {valueType.WRIST_JAW_LEFT, "LWrist" },
        {valueType.WRIST_JAW_RIGHT, "RWrist" },

        {valueType.HAND_LEFT, "LHand" },
        {valueType.HAND_RIGHT, "RHand" },
    };

    //https://developer.softbankrobotics.com/pepper-naoqi-25/naoqi-developer-guide/naoqi-apis/naoqi-motion/almotion/general-tools#general-tools-jointnames
    //go here for reference
    private Dictionary<valueType, string> valueTypeToMovementAxis = new Dictionary<valueType, string>()
    {
        {valueType.KNEE_PITCH, "KneePitch" },

        {valueType.HIP_PITCH, "HipPitch" },
        {valueType.HIP_ROLL, "HipRoll" },

        {valueType.HEAD_PITCH, "HeadPitch" },
        {valueType.HEAD_YAW, "HeadYaw" },

        {valueType.SHOULDER_PITCH_LEFT, "LShoulderPitch" },
        {valueType.SHOULDER_ROLL_LEFT, "LShoulderRoll" },
        {valueType.SHOULDER_PITCH_RIGHT, "RShoulderPitch" },
        {valueType.SHOULDER_ROLL_RIGHT, "RShoulderRoll" },

        {valueType.ELBOW_JAW_LEFT, "LElbowYaw" },
        {valueType.ELBOW_ROLL_LEFT, "LElbowRoll" },
        {valueType.ELBOW_JAW_RIGHT, "RElbowYaw" },
        {valueType.ELBOW_ROLL_RIGHT, "RElbowRoll" },

        {valueType.WRIST_JAW_LEFT, "LWristYaw" },
        {valueType.WRIST_JAW_RIGHT, "RWristYaw" },

        {valueType.HAND_LEFT, "LHand" },
        {valueType.HAND_RIGHT, "RHand" },
    };
}
