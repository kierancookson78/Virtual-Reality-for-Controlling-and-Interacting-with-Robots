using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;

public class ArmTrackingLeftHand : MonoBehaviour
{
    private InputDevice targetDevice;
    [Serializable] public class ArmMovementEvent : UnityEvent<Vector3>{}

    [SerializeField] private ArmMovementEvent onNewShoulderPitch, onNewShoulderRoll;

    bool isShoulderPitchEngaged = false;
    bool isShoulderRollEngaged = false;


    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);


        foreach (var device in devices)
        {
            //Filtering the list of input devices for just the left controller            
            if (device.name.ToLower().Contains("left") && device.name.ToLower().Contains("controller")){
                targetDevice = device;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {    
        //retrieving the ouput data from the left controllers position as a Vector3 value
        //Using boolean flags to prevent methods from being called repeatedly and preent the server from overloading
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePositionValue);
        if(devicePositionValue.y > 0.0f){
            if (!isShoulderPitchEngaged)
            {
                onNewShoulderPitch.Invoke(devicePositionValue);
                isShoulderPitchEngaged = true;
            }       
        }
        else
        {
            isShoulderPitchEngaged = false;
        }

        if(devicePositionValue.x < -0.5f){
            if (!isShoulderRollEngaged)
            {
                onNewShoulderRoll.Invoke(devicePositionValue);
                isShoulderRollEngaged = true;
            }        
        }
        else
        {
            isShoulderRollEngaged = false;
        }


    }  

}
