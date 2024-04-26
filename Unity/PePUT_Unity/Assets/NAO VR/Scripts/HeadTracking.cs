using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using System.Numerics;

public class HeadTracking : MonoBehaviour
{
    private InputDevice targetDevice;

    public XRRig headset;

    [Serializable] public class HeadMovementEvent : UnityEvent<UnityEngine.Quaternion>{}


    [SerializeField] private HeadMovementEvent onNewHeadPitchUp, onNewHeadPitchDown;

    bool isHeadPitchUpEngaged = false;
    bool isHeadPitchDownEngaged = false;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        headset = GetComponent<XRRig>();


        foreach (var device in devices)
        {
            
            if (device.name.ToLower().Contains("") && device.name.ToLower().Contains("")){
                targetDevice = device;
                Debug.Log(device.name + ": " + device.role);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out UnityEngine.Quaternion deviceRotationValue);

        //Sets headpitch to up if the z rotation is above the threshold
        if(deviceRotationValue.z > 0.4)
        {
            if(!isHeadPitchUpEngaged)
            {
                onNewHeadPitchUp.Invoke(deviceRotationValue);
                isHeadPitchUpEngaged = true;
            }
        }
        else
        {
            isHeadPitchUpEngaged = false;
        }
        
        //sets headpitch to down if the z rotation is below the threshold
        if(deviceRotationValue.z < -0.4)
        {
            if(!isHeadPitchDownEngaged)
            {
                onNewHeadPitchDown.Invoke(deviceRotationValue);
                isHeadPitchDownEngaged = true;
            }
        }
        else
        {
            isHeadPitchDownEngaged = false;
        }

    }
}
