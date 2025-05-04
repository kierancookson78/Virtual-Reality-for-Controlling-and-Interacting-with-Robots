using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterHose : MonoBehaviour
{
    public XRNode controllerNode = XRNode.RightHand;
    public Transform headBone;
    public GameObject waterParticleSystemPrefab;
    public float waterSpeed = 10f;
    public float maxDistance = 20f;
    public int waterDamage = 3;

    private GameObject currentWaterStream;
    private ParticleSystem waterParticles;
    private bool isSpraying = false;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        bool triggerValue;

        if (!VRActions.isSitting && !VRActions.isLaying && !VRActions.isCrouching)
        {
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue))
            {
                if (triggerValue && !isSpraying)
                {
                    StartSpraying();
                }
                else if (triggerValue && isSpraying)
                {
                    UpdateWaterStream();
                }
                else if (!triggerValue && isSpraying)
                {
                    StopSpraying();
                }
            }
            else
            {
                if (isSpraying)
                {
                    StopSpraying();
                }
            }
        }
    }

    void StartSpraying()
    {
        isSpraying = true;
        currentWaterStream = Instantiate(waterParticleSystemPrefab, headBone.position, headBone.rotation);
        waterParticles = currentWaterStream.GetComponent<ParticleSystem>();

        if (waterParticles == null)
        {
            Debug.LogError("Water particle system prefab must have a ParticleSystem component.");
            return;
        }

        var mainModule = waterParticles.main;
        mainModule.startSpeed = waterSpeed;
    }

    void UpdateWaterStream()
    {
        if (currentWaterStream == null) return;

        currentWaterStream.transform.position = headBone.position;
        currentWaterStream.transform.rotation = headBone.rotation;

        Vector3 sprayDirection = headBone.forward;

        RaycastHit hit;
        if (Physics.Raycast(headBone.position, sprayDirection, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Fire"))
            {
                Debug.Log("Fire is hit");
                FireBehaviour fire = hit.collider.GetComponent<FireBehaviour>();
                if (fire != null)
                {
                    fire.TakeDamage(waterDamage);
                }
                else
                {
                    Debug.LogWarning("Fire object has 'Fire' tag, but no FireBehavior script.");
                }

            }
        }

        var mainModule = waterParticles.main;
        mainModule.startSpeed = waterSpeed;

        currentWaterStream.transform.rotation = Quaternion.LookRotation(sprayDirection);
    }

    void StopSpraying()
    {
        isSpraying = false;
        if (currentWaterStream != null)
        {
            Destroy(currentWaterStream);
        }
    }
}
