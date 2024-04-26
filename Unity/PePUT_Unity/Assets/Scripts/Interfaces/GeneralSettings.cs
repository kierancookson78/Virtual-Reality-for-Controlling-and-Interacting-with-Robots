using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour, TCPMessageSubscriber
{
    [SerializeField] InputField IPInput;
    [SerializeField] Toggle scriptRun;
    [SerializeField] Toggle IDE;
    [SerializeField] Dropdown autonomousMode;

    private string IP = "";
    static int port = 9559;
    public TCPClientTopic client = null;
    public bool connected = false;

    float battery_charge = 0.0f;

    Autonomy autonomy;
    MotorControl motorControl;
    General general;
    Tablet tablet;
    MemoryData memoryData;

    private void Start()
    {
        string ipConfig = FileReaderWriter.ReadFromFile("config");
        if(ipConfig != "")
        {
            IP = ipConfig;
            IPInput.text = ipConfig;
        }

        string current = Directory.GetCurrentDirectory();
        current += "\\..\\..\\Python";
        Debug.Log(current);
    }

    public void SetIP()
    {
        IP = IPInput.text;
        Debug.Log("IP set to: " + IP);
    }

    private RobotAutonomyContent.AUTONOMY_MODE mode = RobotAutonomyContent.AUTONOMY_MODE.SOLITARY;
    public void SetAutonomousMode()
    {
        mode = (RobotAutonomyContent.AUTONOMY_MODE)autonomousMode.value;
        autonomy.SetAutonomousMode(mode);
    }

    public void WakeUp()
    {
        motorControl.WakeUp();
    }

    public void Sleep()
    {
        motorControl.Sleep();
    }

    public void Connect()
    {
        client = new TCPClientTopic(IP);

        general = new General(client);

        if (scriptRun.isOn)
        {
            general.StartScriptRunner(IP);
        }
        else if (IDE.isOn)
        {
            Debug.Log(IP);
            Debug.Log(general);
            general.StartIDE(IP);
        }
        else
        {
            Debug.LogError("GENERAL SETTINGS ERROR: NO START OPTION SELECTED");
        }

        general.Subscribe(this);

        autonomy = new Autonomy(client);
        motorControl = new MotorControl(client);
        tablet = new Tablet(client);

        memoryData = new MemoryData(client);
        memoryData.Subscribe(this);

        Thread batteryThread = new Thread(() => RequestBatteryInfo());
        batteryThread.Start();
    }

    public void CloseTabletBrowser()
    {
        tablet.CloseTabletBrowser();
    }

    public void StandInit()
    {
        motorControl.StandInit();
    }

    //Method to call the crouching position
    public void CrouchInit()
    {
        motorControl.CrouchInit();
    }

    //Method to call the sitting position
    public void SitRelaxInit()
    {
        motorControl.SitRelaxInit();
    }

    //Method to call lying on bck position
    public void LyingBackInit()
    {
        motorControl.LyingBackInit();
    }

    //Method that takes in a float parameter to set the pitch of the right shoulder
    public void AdjustShoulderPitchRightInit(float angle)
    {
        motorControl.SetShoulderPitchRightInit(angle);
    }

    //Method that takes in a float parameter to set the pitch of the left shoulder
    public void AdjustShoulderPitchLeftInit(float angle)
    {
        motorControl.SetShoulderPitchLeftInit(angle);
    }

    //Method that takes in a float parameter to set the roll of the right shoulder
    public void AdjustShoulderRollLeftInit(float angle)
    {
        motorControl.SetShoulderRollLeftInit(angle);
    }

    //Method that takes in a float parameter to set the roll of the left shoulder
    public void AdjustShoulderRollRightInit(float angle)
    {
        motorControl.SetShoulderRollRightInit(angle);
    }

    //Method that takes in a float parameter to set the roll of the right elbow
    public void AdjustElbowRollRightInit(float angle)
    {
        motorControl.SetElbowRollLRightInit(angle);
    }

    //Method that takes in a float parameter to set the roll of the left elbow
    public void AdjustlbowRollLeftInit(float angle)
    {
        motorControl.SetElbowRollLeftInit(angle);
    }

    //Method that takes in a float parameter to set the pitch of the head
    public void AdjustHeadPitchInit(float angle)
    {
        motorControl.SetHeadPitchInit(angle);
    }

    //Method that takes in a float parameter to set the Yaw of the head
    public void AdjustHeadYawInit(float angle)
    {
        motorControl.SetHeadYawInit(angle);
    }

    public void ReceiveMessage<T>(ref T message_T)
    {
        if (typeof(T) == typeof(MemoryData.MemoryDataInfo))
        {
            MemoryData.MemoryDataInfo info = (MemoryData.MemoryDataInfo)Convert.ChangeType(message_T, typeof(MemoryData.MemoryDataInfo));
            UpdateBatteryCharge(info);
        }
        else
        {
            string message = message_T.ToString();
            if (message == "accept")
            {
                connected = true;
            }
        }
    }

    private void UpdateBatteryCharge(MemoryData.MemoryDataInfo batteryInfo)
    {
        //battery_charge = float.Parse(batteryInfo.value);
        //Debug.Log("Battery Charge: " + battery_charge);
    }

    private void RequestBatteryInfo()
    {
        while (connected)
        {
            memoryData.RequestData(Memory_Data.getBatteryKey(Memory_Data.Battery_Value.Charge));
            Thread.Sleep(300000);
        }
    }
}
