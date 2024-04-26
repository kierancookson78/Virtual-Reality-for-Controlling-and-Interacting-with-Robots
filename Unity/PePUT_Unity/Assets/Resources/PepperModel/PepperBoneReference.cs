using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperBoneReference : MonoBehaviour
{
    [SerializeField] GameObject spine;
    [SerializeField] GameObject spine_001;
    [SerializeField] GameObject spine_002;
    [SerializeField] GameObject spine_004;
    [SerializeField] GameObject spine_005;

    [SerializeField] GameObject upperarm_L;
    [SerializeField] GameObject forearm_L;
    [SerializeField] GameObject hand_L;

    [SerializeField] GameObject upperarm_R;
    [SerializeField] GameObject forearm_R;
    [SerializeField] GameObject hand_R;

    [SerializeField] GameObject fIndex_01_L;
    [SerializeField] GameObject fIndex_02_L;
    [SerializeField] GameObject fIndex_03_L;

    [SerializeField] GameObject fMiddle_01_L;
    [SerializeField] GameObject fMiddle_02_L;
    [SerializeField] GameObject fMiddle_03_L;

    [SerializeField] GameObject fRing_01_L;
    [SerializeField] GameObject fRing_02_L;
    [SerializeField] GameObject fRing_03_L;

    [SerializeField] GameObject fPinky_01_L;
    [SerializeField] GameObject fPinky_02_L;
    [SerializeField] GameObject fPinky_03_L;

    [SerializeField] GameObject fThumb_01_L;
    [SerializeField] GameObject fThumb_02_L;

    [SerializeField] GameObject fIndex_01_R;
    [SerializeField] GameObject fIndex_02_R;
    [SerializeField] GameObject fIndex_03_R;

    [SerializeField] GameObject fMiddle_01_R;
    [SerializeField] GameObject fMiddle_02_R;
    [SerializeField] GameObject fMiddle_03_R;

    [SerializeField] GameObject fRing_01_R;
    [SerializeField] GameObject fRing_02_R;
    [SerializeField] GameObject fRing_03_R;

    [SerializeField] GameObject fPinky_01_R;
    [SerializeField] GameObject fPinky_02_R;
    [SerializeField] GameObject fPinky_03_R;

    [SerializeField] GameObject fThumb_01_R;
    [SerializeField] GameObject fThumb_02_R;

    [SerializeField] GameObject leftHandCollider;
    [SerializeField] GameObject rightHandCollider;

    public GameObject getBone(string boneName)
    {
        switch (boneName)
        {
            case "spine":
                return spine;
            case "spine.001":
                return spine_001;
            case "spine.002":
                return spine_002;
            case "spine.004":
                return spine_004;
            case "spine.005":
                return spine_005;
            case "upper_arm.L":
                return upperarm_L;
            case "upper_arm.R":
                return upperarm_R;
            case "forearm.L":
                return forearm_L;
            case "forearm.R":
                return forearm_R;
            case "hand.L":
                return hand_L;
            case "hand.R":
                return hand_R;

            case "f_index.01.L":
                return fIndex_01_L;
            case "f_index.02.L":
                return fIndex_02_L;
            case "f_index.03.L":
                return fIndex_03_L;

            case "f_middle.01.L":
                return fMiddle_01_L;
            case "f_middle.02.L":
                return fMiddle_02_L;
            case "f_middle.03.L":
                return fMiddle_03_L;

            case "f_ring.01.L":
                return fRing_01_L;
            case "f_ring.02.L":
                return fRing_02_L;
            case "f_ring.03.L":
                return fRing_03_L;

            case "f_pinky.01.L":
                return fPinky_01_L;
            case "f_pinky.02.L":
                return fPinky_02_L;
            case "f_pinky.03.L":
                return fPinky_03_L;

            case "thumb.01.L":
                return fThumb_01_L;
            case "thumb.02.L":
                return fThumb_02_L;


            case "f_index.01.R":
                return fIndex_01_R;
            case "f_index.02.R":
                return fIndex_02_R;
            case "f_index.03.R":
                return fIndex_03_R;

            case "f_middle.01.R":
                return fMiddle_01_R;
            case "f_middle.02.R":
                return fMiddle_02_R;
            case "f_middle.03.R":
                return fMiddle_03_R;

            case "f_ring.01.R":
                return fRing_01_R;
            case "f_ring.02.R":
                return fRing_02_R;
            case "f_ring.03.R":
                return fRing_03_R;

            case "f_pinky.01.R":
                return fPinky_01_R;
            case "f_pinky.02.R":
                return fPinky_02_R;
            case "f_pinky.03.R":
                return fPinky_03_R;

            case "thumb.01.R":
                return fThumb_01_R;
            case "thumb.02.R":
                return fThumb_02_R;

            case "leftHandCollider":
                return leftHandCollider;
            case "rightHandCollider":
                return rightHandCollider;
        }
        return null;
    }
}
