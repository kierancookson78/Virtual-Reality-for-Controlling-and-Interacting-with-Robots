using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float interactionDistance = 100f;
    public XRNode controllerNode = XRNode.LeftHand;
    public Transform playerTransform;
    public AudioClip openingSound;
    public AudioClip closingSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private InputDevice controller;
    private bool triggerPressed = false;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + openAngle);
    }

    void Update()
    {
        if (!VRActions.isSitting && !VRActions.isLaying && !VRActions.isCrouching)
        {
            controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            float triggerValue;

            if (controller == null || playerTransform == null) return;

            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance <= interactionDistance)
            {
                if (controller.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
                {
                    if (triggerValue >= 0.99f)
                    {
                        if (!triggerPressed)
                        {
                            triggerPressed = true;
                            Debug.Log("Door Open");
                            if (!isOpen)
                            {
                                transform.localRotation = openRotation;
                                isOpen = true;
                                AudioSource.PlayClipAtPoint(openingSound, transform.localPosition);
                            }
                            else
                            {
                                transform.localRotation = closedRotation;
                                isOpen = false;
                                AudioSource.PlayClipAtPoint(closingSound, transform.localPosition);
                            }
                        }

                    }
                    else
                    {
                        triggerPressed = false;
                    }
                }
            }
            else
            {
                triggerPressed = false;
            }
        }
    }
}
