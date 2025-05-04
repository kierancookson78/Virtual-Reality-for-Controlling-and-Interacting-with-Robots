using UnityEngine;
using UnityEngine.XR;

public class VRActions : MonoBehaviour
{
    private XRNode controllerNode = XRNode.RightHand;
    private XRNode controllerNodeLeft = XRNode.LeftHand;

    [Header("Spine Bones")]
    public Transform upperSpine;
    public Transform midSpine;
    public Transform lowerSpine;

    [Header("Sitting Angles")]
    public Vector3 upperSpineSitRotation = new Vector3(-30f, 0f, 0f);
    public Vector3 midSpineSitRotation = new Vector3(-20f, 0f, 0f);
    public Vector3 lowerSpineSitRotation = new Vector3(-10f, 0f, 0f);

    [Header("Laying Angles")]
    public Vector3 upperSpineLayRotation = new Vector3(90f, 0f, 0f);
    public Vector3 midSpineLayRotation = new Vector3(90f, 0f, 0f);
    public Vector3 lowerSpineLayRotation = new Vector3(0f, 0f, -90f);

    [Header("Transition Settings")]
    public float transitionDuration = 0.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public static bool isSitting = false;
    public static bool isLaying = false;
    public static bool isCrouching = false;

    private float transitionStartTime;

    private Quaternion upperSpineStartRotation;
    private Quaternion midSpineStartRotation;
    private Quaternion lowerSpineStartRotation;
    private Quaternion upperSpineTargetRotation;
    private Quaternion midSpineTargetRotation;
    private Quaternion lowerSpineTargetRotation;
    private Quaternion upperSpineStartRotationInitial;
    private Quaternion midSpineStartRotationInitial;
    private Quaternion lowerSpineStartRotationInitial;

    private bool primaryButtonHeldPreviousFrame = false;
    private bool primaryButtonLeftHeldPreviousFrame = false;
    private bool secondaryButtonHeldPreviousFrame = false;

    void Start()
    {
        // Store initial rotations
        if (upperSpine != null) upperSpineStartRotationInitial = upperSpine.localRotation;
        if (midSpine != null) midSpineStartRotationInitial = midSpine.localRotation;
        if (lowerSpine != null) lowerSpineStartRotationInitial = lowerSpine.localRotation;

        // Initialize current start rotations for the first transition
        if (upperSpine != null) upperSpineStartRotation = upperSpineStartRotationInitial;
        if (midSpine != null) midSpineStartRotation = midSpineStartRotationInitial;
        if (lowerSpine != null) lowerSpineStartRotation = lowerSpineStartRotationInitial;
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        InputDevice deviceLeft = InputDevices.GetDeviceAtXRNode(controllerNodeLeft);
        bool primaryButtonValue = false;
        bool secondaryButtonValue = false;
        bool primaryButtonValueLeft = false;

        if (!VRNAOMovement.isMoving)
        {
            // Toggle Sit/Stand
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonValue) && !isCrouching)
            {
                if (primaryButtonValue && !primaryButtonHeldPreviousFrame)
                {
                    ToggleSit();
                    StartTransition();
                }
                primaryButtonHeldPreviousFrame = primaryButtonValue;
            }
            else
            {
                primaryButtonHeldPreviousFrame = false;
            }

            if (deviceLeft.isValid && deviceLeft.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonValueLeft))
            {
                if (primaryButtonValueLeft && !primaryButtonLeftHeldPreviousFrame && !isSitting && !isLaying)
                {
                    ToggleCrouch();
                    StartTransition();
                }
                primaryButtonLeftHeldPreviousFrame = primaryButtonValueLeft;
            }
            else
            {
                primaryButtonLeftHeldPreviousFrame = false;
            }

            // Lay Down (only if sitting)
            if (isSitting)
            {
                if (device.isValid && device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonValue))
                {
                    if (secondaryButtonValue && !secondaryButtonHeldPreviousFrame)
                    {
                        LayDown();
                        StartTransition();
                    }
                    secondaryButtonHeldPreviousFrame = secondaryButtonValue;
                }
                else
                {
                    secondaryButtonHeldPreviousFrame = false;
                }
            }
            else if (isLaying) // Transition from Lay to Sit
            {
                if (device.isValid && device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonValue))
                {
                    if (secondaryButtonValue && !secondaryButtonHeldPreviousFrame)
                    {
                        SitUpFromLay();
                        StartTransition();
                    }
                    secondaryButtonHeldPreviousFrame = secondaryButtonValue;
                }
                else
                {
                    secondaryButtonHeldPreviousFrame = false;
                }
            }
            else
            {
                secondaryButtonHeldPreviousFrame = false; // Reset if not sitting or laying
            }

            // Handle the smooth transition every frame if a transition is active
            if (transitionStartTime > 0f)
            {
                PlayAction();
            }
        }
    }

    void ToggleSit()
    {
        isSitting = !isSitting;
        isLaying = false;

        if (upperSpine != null)
        {
            upperSpineStartRotation = upperSpine.localRotation;
            upperSpineTargetRotation = isSitting ? Quaternion.Euler(upperSpineSitRotation) : upperSpineStartRotationInitial;
        }
        if (midSpine != null)
        {
            midSpineStartRotation = midSpine.localRotation;
            midSpineTargetRotation = isSitting ? Quaternion.Euler(midSpineSitRotation) : midSpineStartRotationInitial;
        }
        if (lowerSpine != null)
        {
            lowerSpineStartRotation = lowerSpine.localRotation;
            lowerSpineTargetRotation = isSitting ? Quaternion.Euler(lowerSpineSitRotation) : lowerSpineStartRotationInitial;
        }
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (upperSpine != null)
        {
            upperSpineStartRotation = upperSpine.localRotation;
            upperSpineTargetRotation = isCrouching ? Quaternion.Euler(upperSpineSitRotation) : upperSpineStartRotationInitial;
        }
        if (midSpine != null)
        {
            midSpineStartRotation = midSpine.localRotation;
            midSpineTargetRotation = isCrouching ?  Quaternion.Euler(-90, 0, 0) : midSpineStartRotationInitial;
        }
        if (lowerSpine != null)
        {
            lowerSpineStartRotation = lowerSpine.localRotation;
            lowerSpineTargetRotation = isCrouching ? Quaternion.Euler(180, 0, -180) : lowerSpineStartRotationInitial;
        }
    }

    void LayDown()
    {
        isLaying = true;
        isSitting = false;

        if (upperSpine != null)
        {
            upperSpineStartRotation = upperSpine.localRotation;
            upperSpineTargetRotation = Quaternion.Euler(upperSpineLayRotation);
        }
        if (midSpine != null)
        {
            midSpineStartRotation = midSpine.localRotation;
            midSpineTargetRotation = Quaternion.Euler(midSpineLayRotation);
        }
        if (lowerSpine != null)
        {
            lowerSpineStartRotation = lowerSpine.localRotation;
            lowerSpineTargetRotation = Quaternion.Euler(lowerSpineLayRotation);
        }
    }

    void SitUpFromLay()
    {
        isLaying = false;
        isSitting = true;

        if (upperSpine != null)
        {
            upperSpineStartRotation = upperSpine.localRotation;
            upperSpineTargetRotation = Quaternion.Euler(upperSpineSitRotation);
        }
        if (midSpine != null)
        {
            midSpineStartRotation = midSpine.localRotation;
            midSpineTargetRotation = Quaternion.Euler(midSpineSitRotation);
        }
        if (lowerSpine != null)
        {
            lowerSpineStartRotation = lowerSpine.localRotation;
            lowerSpineTargetRotation = Quaternion.Euler(lowerSpineSitRotation);
        }
    }

    void StartTransition()
    {
        transitionStartTime = Time.time;
    }

    void PlayAction()
    {
        // Handle the smooth transition
        if (transitionStartTime > 0f)
        {
            float timeElapsed = Time.time - transitionStartTime;
            float t = Mathf.Clamp01(timeElapsed / transitionDuration);
            float easedT = transitionCurve.Evaluate(t);

            if (upperSpine != null)
            {
                upperSpine.localRotation = Quaternion.Slerp(upperSpineStartRotation, upperSpineTargetRotation, easedT);
            }
            if (midSpine != null)
            {
                midSpine.localRotation = Quaternion.Slerp(midSpineStartRotation, midSpineTargetRotation, easedT);
            }
            if (lowerSpine != null)
            {
                lowerSpine.localRotation = Quaternion.Slerp(lowerSpineStartRotation, lowerSpineTargetRotation, easedT);
            }

            if (t >= 1f)
            {
                transitionStartTime = 0f; // Transition complete
            }
        }
    }
}
