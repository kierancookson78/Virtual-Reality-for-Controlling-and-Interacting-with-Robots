using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Navigation_Interface : MonoBehaviour, TCPMessageSubscriber
{
    [SerializeField] GeneralSettings generalSettings;
    [SerializeField] Text debugText;
    [SerializeField] Slider speed;
    [SerializeField] Slider acceleration;
    private string debugMessage = "";
    private string lastMessage = "";
    [SerializeField] InputField x;
    [SerializeField] InputField y;
    [SerializeField] InputField theta;
    [SerializeField] PlayerController playerController;
    bool should_move = false;

    Navigation navigation;
    MotorControl motor;
    private void Start()
    {
        Thread waitForClientThread = new Thread(() => waitForClient());
        waitForClientThread.Start();

        //x.text = "0";
        //y.text = "0";
        //theta.text = "0";
    }

    private void waitForClient()
    {
        while (generalSettings.client == null)
        {
            continue;
        }
        navigation = new Navigation(generalSettings.client);
        navigation.Subscribe(this);
    }

    public void Update()
    {
        if(lastMessage != debugMessage)
        {
            debugMessage = lastMessage;
            debugText.text = debugMessage;
        }

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W WAS PRESSED!!!!!!");
            should_move = true;
            navigation.MoveForeward(speed.value);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            should_move = true;
            navigation.MoveLeft(speed.value);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            should_move = true;
            navigation.MoveBackward(speed.value);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            should_move = true;
            navigation.MoveRight(speed.value);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            should_move = true;
            navigation.TurnLeft(speed.value);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            should_move = true;
            navigation.TurnRight(speed.value);
        }
        else if(should_move == true)
        {
            should_move = false;
            navigation.StopMove();
        }

        if (Input.GetMouseButtonUp(0))
        {
            should_move = false;
            if(navigation != null)
            {
                navigation.StopMove();
            }
        }
    }

    public void ReceiveMessage<T>(ref T message)
    {

    }

    public void MoveForeward()
    {
        navigation.MoveForeward(speed.value);
    }

    public void MoveLeft()
    {
        navigation.MoveLeft(speed.value);
    }

    public void MoveBackward()
    {
        navigation.MoveBackward(speed.value);
    }

    public void MoveRight()
    {
        navigation.MoveRight(speed.value);
    }

    public void TurnLeft()
    {
        navigation.TurnLeft(speed.value);
    }

    public void TurnRight()
    {
        navigation.TurnRight(speed.value);
    }

    public void StopMove()
    {
        navigation.StopMove();
    }
    public void Calibrate()
    {
        if(playerController != null)
        {
            navigation.Calibrate(playerController);
        }
        else
        {
            Debug.LogError("No PlayerController Assigned.");
        }
    }
}
