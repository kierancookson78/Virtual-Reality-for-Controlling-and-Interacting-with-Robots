using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;

public class ArmTracking : MonoBehaviour
{
    private InputDevice targetDevice;
    [SerializeField] private GeneralSettings generalSettings;
    private float lastRotationTime = 0f;
    private float rotationCooldown = 1f;

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
        if (Time.time - lastRotationTime >= rotationCooldown)
        {
            targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePositionValue);
            generalSettings.AdjustShoulderPitchRightInit(devicePositionValue.y * Mathf.Rad2Deg);
            generalSettings.AdjustShoulderRollRightInit(devicePositionValue.x * Mathf.Rad2Deg);
            lastRotationTime = Time.time;
        }
    }
}
