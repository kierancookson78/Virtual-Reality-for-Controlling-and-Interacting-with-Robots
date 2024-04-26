using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Animations.Rigging;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    public XRNode input;
    private Vector2 primary2DAxisValue;
    private CharacterController user;
    private XRRig vrrig;
    // Start is called before the first frame update
    void Start()
    {
        user = GetComponent<CharacterController>();
        vrrig = GetComponent<XRRig>();         
    }

    // Update is called once per frame
    void Update()
    {
        //Gets the XR node which is set within Unity
        InputDevice device = InputDevices.GetDeviceAtXRNode(input);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisValue);
        
    }

    private void FixedUpdate()
    {
        //This allows the player to move with the direction they are facing being set to forwards
        Quaternion headYaw = Quaternion.Euler(0, vrrig.cameraGameObject.transform.eulerAngles.y , 0);
        Vector3 direction = headYaw * new Vector3(primary2DAxisValue.x, 0, primary2DAxisValue.y);

        user.Move(direction * Time.fixedDeltaTime * moveSpeed);
    }
}
