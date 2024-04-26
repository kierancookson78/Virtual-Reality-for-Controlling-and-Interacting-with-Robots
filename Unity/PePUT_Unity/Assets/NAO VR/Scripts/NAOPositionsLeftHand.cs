using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;
public class NAOPositionsLeftHand : MonoBehaviour
{
    public InputDevice targetDevice;

    [Serializable] public class PositionEvent : UnityEvent<Boolean>{}

    [SerializeField] PositionEvent sittingPosition, lyingBackPosition;
    // Start is called before the first frame update
    void Start()
    {

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);


        foreach (var device in devices)
        {
            //Filtering the list of input devices to only the left controller            
            if (device.name.ToLower().Contains("left") && device.name.ToLower().Contains("controller"))
            {
                targetDevice = device;
                Debug.Log(device.name + ": " + device.role);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if(primaryButtonValue)
        {
            sittingPosition.Invoke(primaryButtonValue);
        }

        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
        if(secondaryButtonValue)
        {
            lyingBackPosition.Invoke(secondaryButtonValue);
        }

    }
}
