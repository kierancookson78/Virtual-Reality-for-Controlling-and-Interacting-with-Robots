using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;

public class ArmTracking : MonoBehaviour
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
            //Filtering the list of input devices to only the right controller            
            if (device.name.ToLower().Contains("right") && device.name.ToLower().Contains("controller")){
                targetDevice = device;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {    
        //Getting output data from the devicePostion using Vector3
        //Utilising Boolean flags to ensure the same method is not repeatedly called and prevent overload to the server
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePositionValue);
        if(devicePositionValue.y > 0.0f)
        {
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

        if(devicePositionValue.x > 0.5)
        {
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
