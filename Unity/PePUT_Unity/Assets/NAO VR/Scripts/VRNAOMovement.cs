using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.U2D;
public class VRNAOMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    public XRNode input;
    private Vector2 primary2DAxisValue;
    private CharacterController user;
    
    // Start is called before the first frame update
    void Start()
    {
        user = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(input);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out  primary2DAxisValue);

    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(primary2DAxisValue.x, 0, primary2DAxisValue.y);

        user.Move(direction * Time.fixedDeltaTime * moveSpeed);
    }
}
