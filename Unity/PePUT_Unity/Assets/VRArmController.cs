using UnityEngine;
using UnityEngine.XR;

public class VRArmController : MonoBehaviour
{
    public XRNode controllerNode = XRNode.RightHand;
    public Transform upperArmBone;
    public Transform forearmBone;
    public Transform handBone;
    public Transform characterRoot;
    public Quaternion rotationOffset = Quaternion.Euler(90, 0, 0);
    public Quaternion rotationOffsetHand = Quaternion.Euler(-90, -180, 0);

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
            UpdateArmBoneRotations(controllerRotation);
        }
    }

    void UpdateArmBoneRotations(Quaternion targetRotation)
    {
        Quaternion boneTargetRotation = characterRoot.rotation * Quaternion.Euler(0, 180, 0) * targetRotation;

        upperArmBone.rotation = boneTargetRotation * rotationOffset;
        forearmBone.rotation = boneTargetRotation * rotationOffset;
        handBone.rotation = boneTargetRotation * rotationOffsetHand;
    }
}