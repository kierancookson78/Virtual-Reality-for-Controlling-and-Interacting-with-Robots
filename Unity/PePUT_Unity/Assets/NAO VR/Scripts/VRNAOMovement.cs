using UnityEngine;
using UnityEngine.XR;

public class VRNAOMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotationSpeed = 2f;
    public static bool isMoving = false;
    public XRNode input;
    public Transform vrRigRoot;
    public float followDistance = 2f;

    private Vector2 primary2DAxisValue;
    private CharacterController user;
    private Quaternion targetRotation;

    void Start()
    {
        user = GetComponent<CharacterController>();

        if (vrRigRoot == null)
        {
            Debug.LogError("VR Rig Root Transform is not assigned in the Inspector!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (!VRActions.isSitting && !VRActions.isLaying && !VRActions.isCrouching)
        {

            InputDevice device = InputDevices.GetDeviceAtXRNode(input);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisValue);

            if (vrRigRoot != null)
            {
                UpdateVRRigPosition();
            }

            Vector3 moveDirection = new Vector3(primary2DAxisValue.x, 0, primary2DAxisValue.y).normalized;
            if (moveDirection.magnitude > 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            if (moveDirection.magnitude > 0.1f) // Avoid rotation when not moving significantly
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg - 180;
                Rotate(targetAngle);
            }

            user.Move(moveDirection * Time.fixedDeltaTime * moveSpeed);
        }
    }

    void Rotate(float angle)
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    void UpdateVRRigPosition()
    {
        // Calculate the desired position behind the character
        Vector3 offset = -transform.forward * followDistance;
        Vector3 targetRigPosition = transform.position + offset;

        // Maintain the VR rig's Y position
        targetRigPosition.y = vrRigRoot.position.y;

        vrRigRoot.position = targetRigPosition;

        // Make the VR rig look at the character
        vrRigRoot.LookAt(transform);
    }
}
