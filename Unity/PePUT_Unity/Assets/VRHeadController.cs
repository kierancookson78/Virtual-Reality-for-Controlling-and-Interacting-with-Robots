using UnityEngine;
using UnityEngine.XR;

public class VRHeadController : MonoBehaviour
{
    public XRNode controllerNode = XRNode.Head;
    public Transform lowerNeckBone;
    public Transform upperNeckBone;
    public Transform characterRoot; // Reference to the root of your character
    public Quaternion rotationOffsetHead = Quaternion.Euler(0, -180, 0);

    private InputDevice device;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(controllerNode);
    }

    void Update()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(controllerNode);
            if (!device.isValid) return;
        }
        Quaternion controllerRotation;

        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotation))
        {
            UpdateHeadRotation(controllerRotation);
        }
    }

    void UpdateHeadRotation(Quaternion targetRotationWorld)
    {
        // Get the character's current world rotation
        Quaternion characterWorldRotation = characterRoot.rotation;

        // Calculate the headset's rotation relative to the character's rotation
        Quaternion headRotationRelative = characterWorldRotation * targetRotationWorld;

        Vector3 euler = headRotationRelative.eulerAngles;

        // Invert the x and z axes rotation to account for offset
        euler.x = -euler.x;
        euler.z = -euler.z;

        // Create a new rotation with the inverted pitch
        Quaternion correctedRotation = Quaternion.Euler(euler);

        upperNeckBone.rotation = correctedRotation * rotationOffsetHead;
    }
}
