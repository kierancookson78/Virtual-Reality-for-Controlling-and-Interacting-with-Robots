using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;
public class NAOMovement : MonoBehaviour
{
    private InputDevice targetDevice;

    [Serializable] public class JoystickMovementEvent : UnityEvent<Vector2>{}

    [SerializeField] private JoystickMovementEvent onForwardMovement, onBackwardMovement, onJoystickNeutral, onLeftTurn, onRightTurn, onSideStepLeft, onSideStepRight;
    

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
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);   

        //Forward Movement
        if (primary2DAxisValue.y > 0.5f && primary2DAxisValue.x < 0.5f && primary2DAxisValue.x > -0.5f)
        {
            Debug.Log("Joystick moved forward: " + primary2DAxisValue);
            onForwardMovement.Invoke(primary2DAxisValue);
        }
        
        //Backward Movement
        if (primary2DAxisValue.y < -0.5f && primary2DAxisValue.x < 0.5f && primary2DAxisValue.x > -0.5f)
        {
            Debug.Log("Joystick moved backward: " + primary2DAxisValue);
            onBackwardMovement.Invoke(primary2DAxisValue);
        }
        
        //Side Step Right
        if (primary2DAxisValue.x > 0.5f && primary2DAxisValue.y < 0.5f && primary2DAxisValue.y > -0.5f)
        {
            Debug.Log("Joystick moved right: " + primary2DAxisValue);
            onSideStepRight.Invoke(primary2DAxisValue);
        }
        
        //Side Step Left
        if (primary2DAxisValue.x < -0.5f && primary2DAxisValue.y < 0.5f && primary2DAxisValue.y > -0.5f)
        {
            Debug.Log("Joystick moved left: " + primary2DAxisValue);
            onSideStepLeft.Invoke(primary2DAxisValue);
        }
        
        //Right Turn
        if (primary2DAxisValue.x > 0.5f && primary2DAxisValue.y > 0.5f)
        {
            Debug.Log("Joystick turned right: " + primary2DAxisValue);
            onRightTurn.Invoke(primary2DAxisValue);
        }
        
        //Left Turn
        if (primary2DAxisValue.x < -0.5f && primary2DAxisValue.y > 0.5f)
        {
            Debug.Log("Joystick turned left: " + primary2DAxisValue);
            onLeftTurn.Invoke(primary2DAxisValue);
        }
        
        //Joystick Neutral Position
        if (primary2DAxisValue == Vector2.zero)
        {

            onJoystickNeutral.Invoke(primary2DAxisValue);

        }

    }
}
