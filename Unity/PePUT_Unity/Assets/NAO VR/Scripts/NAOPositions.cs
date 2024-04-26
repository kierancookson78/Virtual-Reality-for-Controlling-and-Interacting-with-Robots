using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;

public class NAOPositions : MonoBehaviour
{
    public InputDevice targetDevice;

    [Serializable] public class PositionEvent : UnityEvent<Boolean>{}

    [SerializeField] PositionEvent standingPosition, crouchingPosition;
    // Start is called before the first frame update
    void Start()
    {

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);


        foreach (var device in devices)
        {
            //Filtering the list of input devices to only the right controller            
            if (device.name.ToLower().Contains("right") && device.name.ToLower().Contains("controller"))
            {
                targetDevice = device;
                Debug.Log(device.name + ": " + device.role);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Takes the output data of the device as a boolean from the primary button (A)
        //Sets the robot to the postion required
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if(primaryButtonValue)
        {
            standingPosition.Invoke(primaryButtonValue);
        }

        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
        if(secondaryButtonValue)
        {
            crouchingPosition.Invoke(secondaryButtonValue);
        }

    }
}
