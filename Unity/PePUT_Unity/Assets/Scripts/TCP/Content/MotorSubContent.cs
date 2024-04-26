using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MotorSubContent : Content
{
    public string sa;//stiffnessAxis;
    public string s;//stiffness;
    public string ma;//movementAxis;
    public string mv;//movementValue;
    public string sp;//speed;
    public string st;//sleepTime;

    public MotorSubContent(valueType type, float motorValue, float speed = 0.1f, float sleepTime = 3.0f)
    {
        if (!valueTypeToStiffnessAxis.ContainsKey(type))
        {
            // Handle the case where the valueType is not found
            Debug.LogError("Invalid valueType: " + type);
            return;
        }
        sa = valueTypeToStiffnessAxis[type];
        s = 1.0f.ToString();
        ma = valueTypeToMovementAxis[type];
        mv = RoundToDecimals(motorValue, 3).ToString().Replace(',', '.');
        sp = RoundToDecimals(speed, 3).ToString().Replace(',', '.');
        st = RoundToDecimals(sleepTime, 3).ToString().Replace(',', '.');
    }

    private float RoundToDecimals(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }

    //Rewritten list of enums to accomodate for NAO robots motor list
    //Commented out is the enum list for pepper provided by the toolkit
    public enum valueType
    {

        /**
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
        **/

        
        //Head Joints
        HEAD_YAW,
        HEAD_PITCH,

        //Left Arm Joints
        SHOULDER_PITCH_LEFT,
        SHOULDER_ROLL_LEFT,
        ELBOW_YAW_LEFT,
        ELBOW_ROLL_LEFT,
        WRIST_YAW_LEFT,
        HAND_LEFT,

        //Right Arm Joints
        SHOULDER_PITCH_RIGHT,
        SHOULDER_ROLL_RIGHT,
        ELBOW_YAW_RIGHT,
        ELBOW_ROLL_RIGHT,
        WRIST_YAW_RIGHT,
        HAND_RIGHT,

        //Pelvic Joints
        HIP_YAW_PITCH_LEFT,
        HIP_YAW_PITCH_RIGHT,

        //Left Leg Joints
        HIP_ROLL_LEFT,
        HIP_PITCH_LEFT,
        KNEE_PITCH_LEFT,
        ANKLE_PITCH_LEFT,
        ANKLE_ROLL_LEFT,

        //Right Leg Joints
        HIP_ROLL_RIGHT,
        HIP_PITCH_RIGHT,
        KNEE_PITCH_RIGHT,
        ANKLE_PITCH_RIGHT,
        ANKLE_ROLL_RIGHT,
        

    }

    //Rewritten Dictionary to accomodate for NAO robots motor list to set the stiffness of joints
    //Commented out section is the motor list for Pepper provided by the toolkit
    private Dictionary<valueType, string> valueTypeToStiffnessAxis = new Dictionary<valueType, string>()
    {
        
        /**
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
        **/
        
        
        {valueType.HEAD_YAW, "HeadYaw"},
        {valueType.HEAD_PITCH, "HeadPitch"},

        {valueType.SHOULDER_PITCH_LEFT, "LShoulderPitch"},
        {valueType.SHOULDER_ROLL_LEFT, "LShoulderRoll"},
        {valueType.ELBOW_YAW_LEFT, "LElbowYaw"},
        {valueType.ELBOW_ROLL_LEFT, "LElbowRoll"},
        {valueType.WRIST_YAW_LEFT, "LWristYaw"},
        {valueType.HAND_LEFT, "LHand"},

        {valueType.SHOULDER_PITCH_RIGHT, "RShoulderPitch"},
        {valueType.SHOULDER_ROLL_RIGHT, "RShoulderRoll"},
        {valueType.ELBOW_YAW_RIGHT, "RElbowYaw"},
        {valueType.ELBOW_ROLL_RIGHT, "RElbowRoll"},
        {valueType.WRIST_YAW_RIGHT, "RWristYaw"},
        {valueType.HAND_RIGHT, "RHand"},

        {valueType.HIP_YAW_PITCH_LEFT, "LHipYawPitch"},
        {valueType.HIP_YAW_PITCH_RIGHT, "RHipYawPitch"},

        {valueType.HIP_ROLL_LEFT, "LHipRoll"},
        {valueType.HIP_PITCH_LEFT, "LHipPitch"},
        {valueType.KNEE_PITCH_LEFT, "LKneePitch"},
        {valueType.ANKLE_PITCH_LEFT, "LAnklePitch"},
        {valueType.ANKLE_ROLL_LEFT, "LAnkleRoll"},

        {valueType.HIP_ROLL_RIGHT, "RHipRoll"},
        {valueType.HIP_PITCH_RIGHT, "RHipPitch"},
        {valueType.KNEE_PITCH_RIGHT, "RKneePitch"},
        {valueType.ANKLE_PITCH_RIGHT, "RAnklePitch"},
        {valueType.ANKLE_ROLL_RIGHT, "RAnkleRoll"},
        

    };

    //https://developer.softbankrobotics.com/pepper-naoqi-25/naoqi-developer-guide/naoqi-apis/naoqi-motion/almotion/general-tools#general-tools-jointnames
    //go here for reference

    //Rewritten Dictionary to accomodate for NAO robots motor list to set the stiffness of joints
    //Commented out section is the motor list for Pepper provided by the toolkit
    private Dictionary<valueType, string> valueTypeToMovementAxis = new Dictionary<valueType, string>()
    {
        /**
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
        **/
    

        
        {valueType.HEAD_YAW, "HeadYaw"},
        {valueType.HEAD_PITCH, "HeadPitch"},

        {valueType.SHOULDER_PITCH_LEFT, "LShoulderPitch"},
        {valueType.SHOULDER_ROLL_LEFT, "LShoulderRoll"},
        {valueType.ELBOW_YAW_LEFT, "LElbowYaw"},
        {valueType.ELBOW_ROLL_LEFT, "LElbowRoll"},
        {valueType.WRIST_YAW_LEFT, "LWristYaw"},
        {valueType.HAND_LEFT, "LHand"},

        {valueType.SHOULDER_PITCH_RIGHT, "RShoulderPitch"},
        {valueType.SHOULDER_ROLL_RIGHT, "RShoulderRoll"},
        {valueType.ELBOW_YAW_RIGHT, "RElbowYaw"},
        {valueType.ELBOW_ROLL_RIGHT, "RElbowRoll"},
        {valueType.WRIST_YAW_RIGHT, "RWristYaw"},
        {valueType.HAND_RIGHT, "RHand"},

        {valueType.HIP_YAW_PITCH_LEFT, "LHipYawPitch"},
        {valueType.HIP_YAW_PITCH_RIGHT, "RHipYawPitch"},

        {valueType.HIP_ROLL_LEFT, "LHipRoll"},
        {valueType.HIP_PITCH_LEFT, "LHipPitch"},
        {valueType.KNEE_PITCH_LEFT, "LKneePitch"},
        {valueType.ANKLE_PITCH_LEFT, "LAnklePitch"},
        {valueType.ANKLE_ROLL_LEFT, "LAnkleRoll"},

        {valueType.HIP_ROLL_RIGHT, "RHipRoll"},
        {valueType.HIP_PITCH_RIGHT, "RHipPitch"},
        {valueType.KNEE_PITCH_RIGHT, "RKneePitch"},
        {valueType.ANKLE_PITCH_RIGHT, "RAnklePitch"},
        {valueType.ANKLE_ROLL_RIGHT, "RAnkleRoll"},
        

    };
}
