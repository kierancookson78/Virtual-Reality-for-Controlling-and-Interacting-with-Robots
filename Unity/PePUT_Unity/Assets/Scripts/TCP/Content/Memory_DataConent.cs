using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory_DataConent : Content
{
    public enum FUNCTION
    {
        GO_TO_CHARGING_STATION,
        LEAVE_CHRAGING_STATION,
    }

    private Dictionary<FUNCTION, string> functionLookup = new Dictionary<FUNCTION, string>()
    {
        {FUNCTION.GO_TO_CHARGING_STATION, "goToChargingStation" },
        {FUNCTION.LEAVE_CHRAGING_STATION, "leaveChargingStation" },
    };

    public string function;

    public Memory_DataConent(FUNCTION function)
    {
        this.function = functionLookup[function];
    }

    public string data_key;

    public Memory_DataConent(string data_key)
    {
        this.data_key = data_key;
    }

    #region Joints & Actuators
    public enum Joints_Actuators
    {
        HeadYaw,
        HeadPitch,
        LElbowYaw,
        LElbowRoll,
        RElbowYaw,
        RElbowRoll,
        LHand,
        LWristYaw,
        RHand,
        RWristYaw,
        LShoulderPitch,
        LShoulderRoll,
        RShoulderPitch,
        RShoulderRoll,
        HipRoll,
        HipPitch,
        KneePitch,
        WheelFL,
        WheelFR,
        WheelB
    }

    public enum Joints_Actuators_Value
    {
        Position_Actuator,
        Position_Sensor,
        Electric_Current,
        Temperature_Value,
        Stiffness,
        Temperature_Status,
    }

    private Dictionary<Joints_Actuators_Value, string> joints_actuators_lookup = new Dictionary<Joints_Actuators_Value, string>()
    {
        {Joints_Actuators_Value.Position_Actuator, "Position/Actuator/Value" },
        {Joints_Actuators_Value.Position_Sensor, "Position/Sensor/Value" },
        {Joints_Actuators_Value.Electric_Current, "ElectricCurrent/Sensor/Value" },
        {Joints_Actuators_Value.Temperature_Value, "Temperature/Sensor/Value" },
        {Joints_Actuators_Value.Stiffness, "Hardness/Actuator/Value" },
        {Joints_Actuators_Value.Temperature_Status, "Temperature/Sensor/Status" }
    };

    public string getJointsActuatorsKey(Joints_Actuators joints_actuators, Joints_Actuators_Value joints_actuators_value)
    {
        return $"Device/SubDeviceList/{joints_actuators}/{joints_actuators_lookup[joints_actuators_value]}";
    }
    #endregion

    #region Wheels
    public enum Wheels
    {
        WheelFL,
        WheelFR,
        WheelB
    }

    public enum Wheels_Value
    {
        Speed_Actuator,
        Speed_Sensor,
        Electric_Current,
        Temperature_Value,
        Stiffness,
        Temperature_Status,
    }

    private Dictionary<Wheels_Value, string> wheels_lookup = new Dictionary<Wheels_Value, string>()
    {
        {Wheels_Value.Speed_Actuator, "Speed/Actuator/Value" },
        {Wheels_Value.Speed_Sensor, "Speed/Sensor/Value" },
        {Wheels_Value.Electric_Current, "ElectricCurrent/Sensor/Value" },
        {Wheels_Value.Temperature_Value, "Temperature/Sensor/Value" },
        {Wheels_Value.Stiffness, "Stiffness/Actuator/Value" },
        {Wheels_Value.Temperature_Status, "Temperature/Sensor/Status" },
    };

    public string getWheelKey(Wheels wheels, Wheels_Value wheels_value)
    {
        return $"Device/SubDeviceList/{wheels}/{wheels_lookup[wheels_value]}";
    }
    #endregion

    #region Touch Sensors
    public enum Touch_Sensors_Hand
    {
        LHand,
        RHand
    }

    public enum Touch_Sensors_Head
    {
        Front,
        Rear,
        Middle,
    }

    public string getTouchSensorsHeadKey(Touch_Sensors_Head touch_sensors_head)
    {
        return $"Device/SubDeviceList/Head/Touch/{touch_sensors_head}/Sensor/Value";
    }

    public string getTouchSensorsHandKey(Touch_Sensors_Hand touch_sensors_hand)
    {
        return $"Device/SubDeviceList/{touch_sensors_hand}/Touch/Back/Sensor/Value";
    }
    #endregion

    #region Switches
    public string getChestButtonKey()
    {
        return "Device/SubDeviceList/ChestBoard/Button/Sensor/Value";
    }

    public enum Bumpers
    {
        FrontRight,
        FrontLeft,
        Back
    }

    public string getBumperKey(Bumpers bumpers)
    {
        return $"Device/SubDeviceList/Platform/{bumpers}/Bumper/Sensor/Value";
    }
    #endregion

    #region Inertial Sensors
    public enum Inertial_Sensor
    {
        GyroscopeX,
        GyroscopeY,
        GyroscopeZ,

        AngleX,
        AngleY,
        AngleZ,

        AccelerometerX,
        AccelerometerY,
        AccelerometerZ
    }

    public string getInertialSensorKey(Inertial_Sensor inertial_sensor)
    {
        return $"Device/SubDeviceList/InertialSensorBase/{inertial_sensor}/Sensor/Value";
    }
    #endregion

    #region LEDs
    public enum LED_Color
    {
        Red,
        Green,
        Blue
    }

    public enum LED_Position
    {
        Left,
        Right
    }

    public enum LED_Degree
    {
        Deg0,
        Deg45,
        Deg90,
        Deg135,
        Deg180,
        Deg225,
        Deg270,
        Deg315
    }

    public Dictionary<LED_Degree, string> ledDegreeLookup = new Dictionary<LED_Degree, string>()
    {
        {LED_Degree.Deg0, "0Deg" },
        {LED_Degree.Deg45, "45Deg" },
        {LED_Degree.Deg90, "90Deg" },
        {LED_Degree.Deg135, "135Deg" },
        {LED_Degree.Deg180, "180Deg" },
        {LED_Degree.Deg225, "225Deg" },
        {LED_Degree.Deg270, "270Deg" },
        {LED_Degree.Deg315, "315Deg" },
    };

    public string getFaceLEDKey(LED_Color led_color, LED_Position led_position, LED_Degree led_degree)
    {
        return $"Device/SubDeviceList/Face/Led/{led_color}/{led_position}/{ledDegreeLookup[led_degree]}/Actuator/Value";
    }

    public string getEarLEDKey(LED_Position led_position, LED_Degree led_degree)
    {
        return $"Device/SubDeviceList/Ears/Led/{led_position}/{ledDegreeLookup[led_degree]}/Actuator/Value";
    }

    public string getChestLEDKey(LED_Color led_color)
    {
        return $"Device/SubDeviceList/ChestBoard/Led/{led_color}/Actuator/Value";
    }

    #endregion

    #region Sonars
    public enum Sonar_Position
    {
        Front,
        Back
    }

    public string getSonarKey(Sonar_Position sonar_position)
    {
        return $"Device/SubDeviceList/Platform/{sonar_position}/Sonar/Sensor/Value";
    }
    #endregion

    #region Battery
    public enum Battery_Value
    {
        Current,
        Charge,
        Temperature,
        TemperatureSensorInternal,
        TemperatureSensor1,
        TemperatureSensor2,
        TemperatureSensor3,
        Voltage1,
        Voltage2,
        Voltage3,
        Voltage4,
        Voltage5,
        Voltage6,
        Voltage7,
        Voltage8
    }

    public string getBatteryKey(Battery_Value battery_value)
    {
        return $"Device/SubDeviceList/Battery/{battery_value}/Sensor/Value";
    }
    #endregion

    #region Power Hatch
    public string getPowerHatchKey()
    {
        return "Device/SubDeviceList/Platform/ILS/Sensor/Value";
    }
    #endregion

    #region Infra-red
    public enum Infra_Red_Position
    {
        Left,
        Right
    }

    public string getInfraRedKey(Infra_Red_Position infra_red_position)
    {
        return $"Device/SubDeviceList/Platform/InfraredSpot/{infra_red_position}/Sensor/Value";
    }
    #endregion

    #region Lasers
    public enum Laser_Value_Front
    {
        Status,
        BoardTemp,
        FrameCount,
        AssertFailCount,
        RunMode,
        CalibrDataVer,
        OperationMode_Actuator,
        OperationMode_Sensor
    }

    private Dictionary<Laser_Value_Front, string> laserValueFrontLookup = new Dictionary<Laser_Value_Front, string>() {
        {Laser_Value_Front.Status, "Status/Sensor" },
        {Laser_Value_Front.BoardTemp, "BoardTemp/Sensor" },
        {Laser_Value_Front.FrameCount, "FrameCount/Sensor" },
        {Laser_Value_Front.AssertFailCount, "AssertFailCount/Sensor" },
        {Laser_Value_Front.RunMode, "RunMode/Sensor" },
        {Laser_Value_Front.CalibrDataVer, "CalibrDataVer/Sensor" },
        {Laser_Value_Front.OperationMode_Actuator, "OperationMode/Actuator" },
        {Laser_Value_Front.OperationMode_Sensor, "OperationMode/Sensor" }
    };

    public string getFrontLaserKey(Laser_Value_Front laser_value_front)
    {
        return $"Device/SubDeviceList/Platform/LaserSensor/Front/Reg/{laserValueFrontLookup[laser_value_front]}/Value";
    }

    //Horizontal obstacle data
    public enum Laser_Position
    {
        Front,
        Left,
        Right
    }

    public enum Obstacle_Type
    {
        Horizontal,
        UncertainObstacle,
        AnnoyingReflection
    }

    public enum Front_Obstacle_Data_Segment
    {
        Seg01,
        Seg02,
        Seg03,
        Seg04,
        Seg05,
        Seg06,
        Seg07,
        Seg08,
        Seg09,
        Seg10,
        Seg11,
        Seg12,
        Seg13,
        Seg14,
        Seg15,
        Data_Status,
        Time
    }

    public enum Axis
    {
        X,
        Y
    }


    public string getFrontLaserKey(Laser_Position laser_position, Obstacle_Type obstacle_type, Front_Obstacle_Data_Segment horizontal_obstacle_data_segment, Axis axis = Axis.X)
    {
        if(horizontal_obstacle_data_segment == Front_Obstacle_Data_Segment.Data_Status)
        {
            return $"Device/SubDeviceList/Platform/LaserSensor/{laser_position}/{obstacle_type}/Reg/DataStatus/Sensor/Value";
        }
        else if(horizontal_obstacle_data_segment == Front_Obstacle_Data_Segment.Time)
        {
            return $"Device/SubDeviceList/Platform/LaserSensor/{laser_position}/{obstacle_type}/Reg/Time/Sensor/Value";
        }
        else
        {
            return $"Device/SubDeviceList/Platform/LaserSensor/{laser_position}/{obstacle_type}/{horizontal_obstacle_data_segment}/{axis}/Sensor/Value";
        }
    }

    public enum Shovel_Obstacle_Data_Segment
    {
        Seg01,
        Seg02,
        Seg03
    }

    public string getShovelLaserKey(Shovel_Obstacle_Data_Segment shovel_obstacle_data_segment, Axis axis)
    {
        return $"Device/SubDeviceList/Platform/LaserSensor/Front/Shovel/{shovel_obstacle_data_segment}/{axis}/Sensor/Value";
    }

    public enum Vertical_Laser_Position
    {
        Left,
        Right
    }

    public string getVerticalLaserKey(Vertical_Laser_Position vertical_laser_position, Axis axis)
    {
        return $"Device/SubDeviceList/Platform/LaserSensor/Front/Vertical/{vertical_laser_position}/Seg01/{axis}/Sensor/Value";
    }
    #endregion
}
