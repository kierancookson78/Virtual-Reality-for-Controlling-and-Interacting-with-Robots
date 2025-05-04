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

    [SerializeField] private GeneralSettings generalSettings;
    private float lastRotationTime = 0f;
    private float rotationCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        headset = GetComponent<XRRig>();


        foreach (var device in devices)
        {

            if (device.name.ToLower().Contains("") && device.name.ToLower().Contains(""))
            {
                targetDevice = device;
                Debug.Log(device.name + ": " + device.role);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out UnityEngine.Quaternion deviceRotationValue);
        if (Time.time - lastRotationTime >= rotationCooldown)
        {
            generalSettings.AdjustHeadPitchInit(deviceRotationValue.z * Mathf.Rad2Deg);
            generalSettings.AdjustHeadYawInit(deviceRotationValue.y * Mathf.Rad2Deg);
            lastRotationTime = Time.time;
        }
    }
}
