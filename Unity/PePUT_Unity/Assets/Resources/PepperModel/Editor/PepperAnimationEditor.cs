using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEditor.Animations;
using System.IO;
using System.Collections;
using System.Threading;
using System;

public class PepperAnimationEditor : EditorWindow, TCPMessageSubscriber
{
    [MenuItem("Window/Pepper/PepperEditor")]
    public static void ShowExample()
    {
        PepperAnimationEditor wnd = GetWindow<PepperAnimationEditor>();
        wnd.titleContent = new GUIContent("PepperAnimationEditor");
    }

    #region Private Variables
    private float pepperKneePitch = 0.0f;
    private float pepperHipPitch = 0.0f;
    private float pepperHipRoll = 0.0f;
    private float pepperHeadPitch = 0.0f;
    private float pepperHeadYaw = 0.0f;
    //left
    private float pepperShoulderPitchL = 0.0f;
    private float pepperShoulderRollL = 0.0f;
    private float pepperElbowJawL = 0.0f;
    private float pepperElbowRollL = 0.0f;
    private float pepperWristJawL = 0.0f;
    private float pepperHandL = 0.0f;
    //right
    private float pepperShoulderPitchR = 0.0f; //is -90 im base form
    private float pepperShoulderRollR = 0.0f;
    private float pepperElbowJawR = 0.0f;
    private float pepperElbowRollR = 0.0f;
    private float pepperWristJawR = 0.0f;
    private float pepperHandR = 0.0f;
    #endregion

    public struct PepperPose {
        public float pepperKneePitch;
        public float pepperHipPitch;
        public float pepperHipRoll;
        public float pepperHeadPitch;
        public float pepperHeadYaw;
        //left
        public float pepperShoulderPitchL;
        public float pepperShoulderRollL;
        public float pepperElbowYawL;
        public float pepperElbowRollL;
        public float pepperWristJawL;
        public float pepperHandLOpen;
        //right
        public float pepperShoulderPitchR;
        public float pepperShoulderRollR;
        public float pepperElbowYawR;
        public float pepperElbowRollR;
        public float pepperWristJawR;
        public float pepperHandROpen;
    };

    struct simpleCommand
    {
        public string type;
    };

    struct speechCommand
    {
        public string type;
        public string content;
    }

    private TCPClientTopic client;

    float timer = 0.0f;
    float timerThreshhold = 3.0f;
    bool change = false;
    string IP = "132.187.8.179";
    MotorValueParser mvp = new MotorValueParser();
    General general = null;
    MotorControl motorControl = null;
    GameObject pepper = null;
    
    private void Update()
    {
        if (Application.isPlaying && client == null)
        {
            if (GameObject.Find("pepper_prefab") == null)
                return;
            client = GameObject.Find("pepper_prefab").GetComponent<TCPClientTopic>();
        }

        if (timer < 10.0f)
        {
            timer += Time.deltaTime;
        }
        if(change && timer > timerThreshhold)
        {
            //SendAllSliderValues(mvp);
        }
    }

    VisualElement root;
    Vector3 kneeBase = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hipPitchBase = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 headPitchBase = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 head = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 shoulderBase = new Vector3(-6.651f, -8.752001f, 58.725f);
    Vector3 elbowBase = new Vector3(3.57f, -89.731f, 2.578f);
    Vector3 wristBase = new Vector3(-4.713f, 0.026f, -5.419f);
    List<Vector3> leftHandValues = new List<Vector3>();
    List<Vector3> rightHandValues = new List<Vector3>();

    AnimationCurve curve = new AnimationCurve();

    public void OnEnable()
    {
        pepper = GameObject.FindGameObjectWithTag("Pepper");
        Debug.Log(pepper);

        MotorValueParser parser = new MotorValueParser();

        
        if (GetBone("leftHandCollider") == null)
            return;
        #region Pepper Controls
        #region colliders
        BoxCollider leftArmCollider = GetBone("leftHandCollider").GetComponent<BoxCollider>();
        BoxCollider rightArmCollider = GetBone("rightHandCollider").GetComponent<BoxCollider>();
        #endregion

        #region Setup
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/PepperModel/Editor/PepperAnimationEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);

        #region StyleSheet
        // Import Stylesheet
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/PepperModel/Editor/PepperAnimationEditor.uss");
        root.styleSheets.Add(styleSheet);
        #endregion
        #endregion

        #region Options
        bool preventIntersection = true;
        bool autoSend = false;
        #endregion


        root.Q<ObjectField>("ObjectField").objectType = typeof(GameObject);
        root.Q<ObjectField>("ObjectField").RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
        {
            pepper = (GameObject)root.Q<ObjectField>("ObjectField").value;
            Debug.Log(pepper);
        });

        if (pepper != null) 
        { 
            root.Q<ObjectField>("ObjectField").value = pepper; 
        }

        root.Q<ObjectField>("GeneralSettings").objectType = typeof(GameObject);
        root.Q<ObjectField>("GeneralSettings").RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
        {       
            GameObject generalSettings = (GameObject)root.Q<ObjectField>("GeneralSettings").value;
            GeneralSettings settings = generalSettings.GetComponent<GeneralSettings>();
            client = settings.client;
            Debug.Log(client);
        });

        #region Knee Pitch
        root.Q<Slider>("KneePitch").lowValue = -29.5f;
        root.Q<Slider>("KneePitch").highValue = 29.5f;
        root.Q<Slider>("KneePitch").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SyncFieldToSlider(root, "KneePitchField", "KneePitch");
            RotateBoneX(GetBone("spine.001"), kneeBase.x + root.Q<Slider>("KneePitch").value);
            pepperKneePitch = root.Q<Slider>("KneePitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("KneePitchReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneX(GetBone("spine.001"), kneeBase.x);
            root.Q<Slider>("KneePitch").value = 0.0f;
            root.Q<FloatField>("KneePitchField").value = 0.0f;
            pepperKneePitch = root.Q<Slider>("KneePitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("KneePitchField").RegisterCallback<FocusOutEvent>(evt =>
        {
            root.Q<FloatField>("KneePitchField").value = Clamp(root.Q<FloatField>("KneePitchField").value, -29.5f, 29.5f);
            SyncSliderToField(root, "KneePitch", "KneePitchField");
            RotateBoneX(GetBone("spine.001"), kneeBase.x + root.Q<FloatField>("KneePitchField").value);
            pepperKneePitch = root.Q<FloatField>("KneePitchField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region Hip
        #region Hip Pitch
        root.Q<Slider>("HipPitch").lowValue = -59.5f;
        root.Q<Slider>("HipPitch").highValue = 59.5f;
        root.Q<Slider>("HipPitch").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SyncFieldToSlider(root, "HipPitchField", "HipPitch");
            RotateBoneX(GetBone("spine.002"), hipPitchBase.x + root.Q<Slider>("HipPitch").value);
            pepperHipPitch = root.Q<Slider>("HipPitch").value;
            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HipPitchReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneX(GetBone("spine.002"), hipPitchBase.x);
            root.Q<Slider>("HipPitch").value = 0.0f;
            root.Q<FloatField>("HipPitchField").value = 0.0f;
            pepperHipPitch = root.Q<Slider>("HipPitch").value;

            if (autoSend) {change = true; SendAllSliderValues(parser);}
        });

        root.Q<FloatField>("HipPitchField").RegisterCallback<FocusOutEvent>(evt =>
        {
            root.Q<FloatField>("HipPitchField").value = Clamp(root.Q<FloatField>("HipPitchField").value, -59.5f, 59.5f);
            SyncSliderToField(root, "HipPitch", "HipPitchField");
            RotateBoneX(GetBone("spine.002"), hipPitchBase.x + root.Q<FloatField>("HipPitchField").value);
            pepperHipPitch = root.Q<FloatField>("HipPitchField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region Hip Roll
        root.Q<Slider>("HipRoll").lowValue = -29.5f;
        root.Q<Slider>("HipRoll").highValue = 29.5f;
        root.Q<Slider>("HipRoll").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SyncFieldToSlider(root, "HipRollField", "HipRoll");
            RotateBoneZ(GetBone("spine.002"), hipPitchBase.z + root.Q<Slider>("HipRoll").value);
            pepperHipRoll = root.Q<Slider>("HipRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HipRollReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneZ(GetBone("spine.002"), hipPitchBase.z);
            root.Q<Slider>("HipRoll").value = 0.0f;
            root.Q<FloatField>("HipRollField").value = 0.0f;
            pepperHipRoll = root.Q<Slider>("HipRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("HipRollField").RegisterCallback<FocusOutEvent>(evt =>
        {
            root.Q<FloatField>("HipRollField").value = Clamp(root.Q<FloatField>("HipRollField").value, -29.5f, 29.5f);
            SyncSliderToField(root, "HipRoll", "HipRollField");
            RotateBoneZ(GetBone("spine.002"), hipPitchBase.z + root.Q<FloatField>("HipRollField").value);
            pepperHipRoll = root.Q<FloatField>("HipRollField").value;

            if (autoSend) {change = true; SendAllSliderValues(parser);}
        });
        #endregion
        #endregion

        #region Head
        //var headLocal = GetBone("spine.005").transform.localEulerAngles;
        //var headLocalX = headLocal.x;
        //var headLocalY = headLocal.y;
        #region Head Pitch
        root.Q<Slider>("HeadPitch").lowValue = -40.5f;
        root.Q<Slider>("HeadPitch").highValue = 25.5f;
        root.Q<Slider>("HeadPitch").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            //limit rotation
            float headPitch = headPitchLimited(root.Q<Slider>("HeadJaw").value, root.Q<Slider>("HeadPitch").value);
            root.Q<Slider>("HeadPitch").value = headPitch;

            //sync field to slider
            SyncFieldToSlider(root, "HeadPitchField", "HeadPitch");

            //set bone value
            head.x = headPitchBase.x + root.Q<Slider>("HeadPitch").value;
            GetBone("spine.005").transform.localEulerAngles = head;

            pepperHeadPitch = root.Q<Slider>("HeadPitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HeadPitchReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            head.x = headPitchBase.x;
            GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
            root.Q<Slider>("HeadPitch").value = 0.0f;
            root.Q<FloatField>("HeadPitchField").value = 0.0f;
            pepperHeadPitch = root.Q<Slider>("HeadPitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("HeadPitchField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //limit max Input
            float headPitch = Clamp(root.Q<FloatField>("HeadPitchField").value, -40.5f, 25.5f);
            headPitch = headPitchLimited(root.Q<Slider>("HeadJaw").value, headPitch);

            //sync value
            root.Q<FloatField>("HeadPitchField").value = headPitch;
            SyncSliderToField(root, "HeadPitch", "HeadPitchField");

            //set bone value
            head.x = headPitchBase.x + root.Q<FloatField>("HeadPitchField").value;
            GetBone("spine.005").transform.localEulerAngles = head;

            pepperHeadPitch = root.Q<FloatField>("HeadPitchField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region Head Jaw
        root.Q<Slider>("HeadJaw").lowValue = -119.5f;
        root.Q<Slider>("HeadJaw").highValue = 119.5f;
        root.Q<Slider>("HeadJaw").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //limit rotation
            float headYaw = headYawLimited(root.Q<Slider>("HeadPitch").value, root.Q<Slider>("HeadJaw").value);
            root.Q<Slider>("HeadJaw").value = headYaw;

            //sync field to slider
            SyncFieldToSlider(root, "HeadJawField", "HeadJaw");

            //set bone value
            head.y = headPitchBase.y + root.Q<Slider>("HeadJaw").value;
            GetBone("spine.005").transform.localEulerAngles = head;

            pepperHeadYaw = root.Q<Slider>("HeadJaw").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HeadJawReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            head.y = 0.0f;
            GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
            root.Q<Slider>("HeadJaw").value = 0.0f;
            root.Q<FloatField>("HeadJawField").value = 0.0f;
            pepperHeadYaw = root.Q<Slider>("HeadJaw").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("HeadJawField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //limit max Input
            float headYaw = Clamp(root.Q<FloatField>("HeadJawField").value, -119.5f, 119.5f);
            headYaw = headYawLimited(root.Q<Slider>("HeadPitch").value, headYaw);

            //sync value
            root.Q<FloatField>("HeadJawField").value = headYaw;
            SyncSliderToField(root, "HeadJaw", "HeadJawField");

            //set bone value
            head.y = headPitchBase.y + root.Q<FloatField>("HeadJawField").value;
            GetBone("spine.005").transform.localEulerAngles = head;

            pepperHeadYaw = root.Q<FloatField>("HeadJawField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #endregion

        #region Shoulder
        Vector3 shoulderBaseEulerLocal = new Vector3(353.3f, 351.2f, 58.7f);
        Vector3 shoulder = shoulderBaseEulerLocal;

        #region ShoulderL
        var shoulderLGlobal = GetBone("upper_arm.L").transform.localEulerAngles;
        var shoulderLGlobalX = shoulderLGlobal.x;
        var shoulderLGlobalZ = shoulderLGlobal.z;
        float oldShoulderPitchValueL = 0.0f;
        #region ShoulderL Pitch
        root.Q<Slider>("ShoulderLPitch").lowValue = -119.5f;
        root.Q<Slider>("ShoulderLPitch").highValue = 119.5f;
        root.Q<Slider>("ShoulderLPitch").value = 0.0f;

        float anglePrevious = 0.0f;
        root.Q<Slider>("ShoulderLPitch").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.L").transform.eulerAngles;

            float angleDelta = root.Q<Slider>("ShoulderLPitch").value - anglePrevious;
            anglePrevious = root.Q<Slider>("ShoulderLPitch").value;
            GetBone("upper_arm.L").transform.Rotate(GetBone("spine").transform.right, angleDelta, Space.World);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.L").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ShoulderLPitch").value = oldShoulderPitchValueL;
            }
            else
            {
                oldShoulderPitchValueL = root.Q<Slider>("ShoulderLPitch").value;
            }

            //sync 
            SyncFieldToSlider(root, "ShoulderLPitchField", "ShoulderLPitch");

            pepperShoulderPitchL = root.Q<Slider>("ShoulderLPitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ShoulderLPitchField").value = 0.0f;
        root.Q<FloatField>("ShoulderLPitchField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ShoulderLPitchField").value = Clamp(root.Q<FloatField>("ShoulderLPitchField").value, -119.5f, 119.5f);

            //get field value
            shoulderLGlobal.x = shoulderLGlobalX + root.Q<FloatField>("ShoulderLPitchField").value;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.L").transform.eulerAngles;

            //set new bone value
            GetBone("upper_arm.L").transform.eulerAngles.Set(shoulderLGlobal.x + 90f, shoulderLGlobal.y, shoulderLGlobal.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.L").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ShoulderLPitchField").value = oldShoulderPitchValueL;
            }
            else
            {
                oldShoulderPitchValueL = root.Q<FloatField>("ShoulderLPitchField").value;
            }

            //sync
            SyncSliderToField(root, "ShoulderLPitch", "ShoulderLPitchField");

            pepperShoulderPitchL = root.Q<FloatField>("ShoulderLPitchField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region ShoulderL Roll
        float oldShoulderRollValue = 0.0f;
        float anglePrevious_shoulderRollL = 0.0f;
        root.Q<Slider>("ShoulderLRoll").lowValue = -0.5f;
        root.Q<Slider>("ShoulderLRoll").highValue = 89.5f;
        root.Q<Slider>("ShoulderLRoll").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            shoulderLGlobal.z = shoulderLGlobalZ + root.Q<Slider>("ShoulderLRoll").value;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.L").transform.eulerAngles;

            //set new bone value
            //GetBone("upper_arm.L").transform.eulerAngles = shoulderLGlobal;

            float angleDelta = root.Q<Slider>("ShoulderLRoll").value - anglePrevious_shoulderRollL;
            anglePrevious_shoulderRollL = root.Q<Slider>("ShoulderLRoll").value;
            GetBone("upper_arm.L").transform.Rotate(0.0f, 0.0f, -angleDelta, Space.Self);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.L").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ShoulderLRoll").value = oldShoulderRollValue;
            }
            else
            {
                oldShoulderRollValue = root.Q<Slider>("ShoulderLRoll").value;
            }

            //sync
            SyncFieldToSlider(root, "ShoulderLRollField", "ShoulderLRoll");

            pepperShoulderRollL = root.Q<Slider>("ShoulderLRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ShoulderLRollField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ShoulderLRollField").value = Clamp(root.Q<FloatField>("ShoulderLRollField").value, -0.5f, 89.5f);

            //get field value
            shoulderLGlobal.z = shoulderLGlobalZ + root.Q<FloatField>("ShoulderLRollField").value;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.L").transform.eulerAngles;

            //set new bone value
            GetBone("upper_arm.L").transform.eulerAngles.Set(shoulderLGlobal.x, shoulderLGlobal.y, shoulderLGlobal.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.L").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ShoulderLRollField").value = oldShoulderRollValue;
            }
            else
            {
                oldShoulderRollValue = root.Q<FloatField>("ShoulderLRollField").value;
            }

            //sync
            SyncSliderToField(root, "ShoulderLRoll", "ShoulderLRollField");

            pepperShoulderRollL = root.Q<FloatField>("ShoulderLRollField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        root.Q<Button>("ShoulderLReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            ResetLeftShoulder(autoSend, parser);
        });

        #endregion

        #region ShoulderR
        var shoulderRGlobal = GetBone("upper_arm.R").transform.eulerAngles;
        var shoulderRGlobalX = shoulderRGlobal.x;
        var shoulderRGlobalZ = shoulderRGlobal.z;
        float oldShoulderPitchValueR = 0.0f;

        #region ShoulderR Pitch
        float anglePrevious_shoulderRPitch = 0.0f;
        root.Q<Slider>("ShoulderRPitch").lowValue = -119.5f;
        root.Q<Slider>("ShoulderRPitch").highValue = 119.5f;
        root.Q<Slider>("ShoulderRPitch").value = 90.0f;
        root.Q<Slider>("ShoulderRPitch").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            shoulderRGlobal.x = shoulderRGlobalX + root.Q<Slider>("ShoulderRPitch").value + 90.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.R").transform.eulerAngles;

            //set new bone value
            float angleDelta = root.Q<Slider>("ShoulderRPitch").value - anglePrevious_shoulderRPitch;
            anglePrevious_shoulderRPitch = root.Q<Slider>("ShoulderRPitch").value;
            GetBone("upper_arm.R").transform.Rotate(GetBone("spine").transform.right, angleDelta, Space.World);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.R").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ShoulderRPitch").value = oldShoulderPitchValueR;
            }
            else
            {
                oldShoulderPitchValueR = root.Q<Slider>("ShoulderRPitch").value;
            }

            //sync 
            SyncFieldToSlider(root, "ShoulderRPitchField", "ShoulderRPitch");

            pepperShoulderPitchR = root.Q<Slider>("ShoulderRPitch").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ShoulderRPitchField").value = 0.0f;
        root.Q<FloatField>("ShoulderRPitchField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ShoulderRPitchField").value = Clamp(root.Q<FloatField>("ShoulderRPitchField").value, -119.5f, 119.5f);

            //get field value
            shoulderRGlobal.x = shoulderRGlobalX + root.Q<FloatField>("ShoulderRPitchField").value + 90.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.R").transform.eulerAngles;

            //set new bone value
            GetBone("upper_arm.R").transform.eulerAngles.Set(shoulderRGlobal.x + 90f, shoulderRGlobal.y, shoulderRGlobal.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.R").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ShoulderRPitchField").value = oldShoulderPitchValueR;
            }
            else
            {
                oldShoulderPitchValueR = root.Q<FloatField>("ShoulderRPitchField").value;
            }

            //sync
            SyncSliderToField(root, "ShoulderRPitch", "ShoulderRPitchField");

            pepperShoulderPitchR = root.Q<FloatField>("ShoulderRPitchField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region ShoulderR Roll
        float anglePrevious_shoulderRRoll = 0.0f;
        float oldShoulderRollValueR = 0.0f;
        root.Q<Slider>("ShoulderRRoll").lowValue = -0.5f;
        root.Q<Slider>("ShoulderRRoll").highValue = 89.5f;
        root.Q<Slider>("ShoulderRRoll").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            shoulderRGlobal.z = shoulderRGlobalZ - root.Q<Slider>("ShoulderRRoll").value;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.R").transform.eulerAngles;

            //set new bone value
            float angleDelta = root.Q<Slider>("ShoulderRRoll").value - anglePrevious_shoulderRRoll;
            anglePrevious_shoulderRRoll = root.Q<Slider>("ShoulderRRoll").value;
            GetBone("upper_arm.R").transform.Rotate(0.0f, 0.0f, angleDelta, Space.Self);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.R").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ShoulderRRoll").value = oldShoulderRollValueR;
            }
            else
            {
                oldShoulderRollValueR = root.Q<Slider>("ShoulderRRoll").value;
            }

            //sync
            SyncFieldToSlider(root, "ShoulderRRollField", "ShoulderRRoll");

            pepperShoulderRollR = root.Q<Slider>("ShoulderRRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ShoulderRRollField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ShoulderRRollField").value = Clamp(root.Q<FloatField>("ShoulderRRollField").value, -0.5f, 89.5f);

            //get field value
            shoulderRGlobal.z = shoulderRGlobalZ - root.Q<FloatField>("ShoulderRRollField").value;

            //save old bone value
            Vector3 oldAngles = GetBone("upper_arm.R").transform.eulerAngles;

            //set new bone value
            GetBone("upper_arm.R").transform.eulerAngles.Set(shoulderRGlobal.x, shoulderRGlobal.y, shoulderRGlobal.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("upper_arm.R").transform.eulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ShoulderRRollField").value = oldShoulderRollValueR;
            }
            else
            {
                oldShoulderRollValueR = root.Q<FloatField>("ShoulderRRollField").value;
            }

            //sync
            SyncSliderToField(root, "ShoulderRRoll", "ShoulderRRollField");

            pepperShoulderRollR = root.Q<FloatField>("ShoulderRRollField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        root.Q<Button>("ShoulderRReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            ResetRightShoulder(autoSend, parser);
        });
        #endregion
        #endregion

        #region Elbow
        #region ElbowL
        float elbowBaseX = 0.0f;
        float elbowBaseY = -90.0f;
        float elbowBaseZ = 350.2f;

        var elbowL = elbowBase;//new Vector3(elbowBaseX, elbowBaseY, elbowBaseZ);
        var elbowLY = elbowL.y;
        var elbowLX = elbowL.x;
        #region Elbow YAW
        float oldElbowLYawValue = 0.0f;
        root.Q<Slider>("ElbowLYaw").lowValue = -119.5f;
        root.Q<Slider>("ElbowLYaw").highValue = 119.5f;
        root.Q<Slider>("ElbowLYaw").value = 0.0f;
        float anglePrevious_ElbowYaw = 0.0f;
        root.Q<Slider>("ElbowLYaw").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            elbowL.y = elbowBaseY - root.Q<Slider>("ElbowLYaw").value - 0.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.L").transform.localEulerAngles;
             
            //set new bone value
            //GetBone("forearm.L").transform.localEulerAngles = elbowL;

            float angleDelta = root.Q<Slider>("ElbowLYaw").value - anglePrevious_ElbowYaw;
            anglePrevious_ElbowYaw = root.Q<Slider>("ElbowLYaw").value;
            GetBone("forearm.L").transform.Rotate(GetBone("upper_arm.L").transform.up, -angleDelta, Space.World);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("forearm.L").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ElbowLYaw").value = oldElbowLYawValue;
            }
            else
            {
                oldElbowLYawValue = root.Q<Slider>("ElbowLYaw").value;
            }

            //sync
            SyncFieldToSlider(root, "ElbowLYawField", "ElbowLYaw");
            pepperElbowJawL = root.Q<Slider>("ElbowLYaw").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ElbowLYawField").value = 0.0f;
        root.Q<FloatField>("ElbowLYawField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ElbowLYawField").value = Clamp(root.Q<FloatField>("ElbowLYawField").value, -119.5f, 119.5f);

            //get field value
            elbowL.y = elbowLY - root.Q<FloatField>("ElbowLYawField").value - 0.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.L").transform.localEulerAngles;

            //set new bone value
            GetBone("forearm.L").transform.localEulerAngles.Set(elbowL.x, elbowL.y, elbowL.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("forearm.L").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ElbowLYawField").value = oldElbowLYawValue;
            }
            else
            {
                oldElbowLYawValue = root.Q<FloatField>("ElbowLYawField").value;
            }

            //sync
            SyncSliderToField(root, "ElbowLYaw", "ElbowLYawField");
            pepperElbowJawL = root.Q<FloatField>("ElbowLYawField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #region Elbow Roll
        float oldElbowLRollValue = 0.0f;
        root.Q<Slider>("ElbowLRoll").lowValue = -89.5f;
        root.Q<Slider>("ElbowLRoll").highValue = 0.5f;

        float anglePrevious_ElbowRoll = 0.0f;
        root.Q<Slider>("ElbowLRoll").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            elbowL.x = elbowBaseX + root.Q<Slider>("ElbowLRoll").value;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.L").transform.localEulerAngles;

            //set new bone value
            //GetBone("forearm.L").transform.localEulerAngles = elbowL;


            float angleDelta = root.Q<Slider>("ElbowLRoll").value - anglePrevious_ElbowRoll;
            anglePrevious_ElbowRoll = root.Q<Slider>("ElbowLRoll").value;
            GetBone("forearm.L").transform.Rotate(-angleDelta, 0.0f, 0.0f, Space.Self);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("forearm.L").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ElbowLRoll").value = oldElbowLRollValue;
            }
            else
            {
                oldElbowLRollValue = root.Q<Slider>("ElbowLRoll").value;
            }

            //sync
            SyncFieldToSlider(root, "ElbowLRollField", "ElbowLRoll");
            pepperElbowRollL = root.Q<Slider>("ElbowLRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ElbowLRollField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ElbowLRollField").value = Clamp(root.Q<FloatField>("ElbowLRollField").value, -89.5f, 0.5f);

            //get field value
            elbowL.x = elbowLX + root.Q<FloatField>("ElbowLRollField").value;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.L").transform.localEulerAngles;

            //set new bone value
            GetBone("forearm.L").transform.localEulerAngles.Set(elbowL.x, elbowL.y, elbowL.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(leftArmCollider) && preventIntersection)
            {
                GetBone("forearm.L").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ElbowLRollField").value = oldElbowLRollValue;
            }
            else
            {
                oldElbowLRollValue = root.Q<FloatField>("ElbowLRollField").value;
            }

            //sync
            SyncSliderToField(root, "ElbowLRoll", "ElbowLRollField");
            pepperElbowRollL = root.Q<FloatField>("ElbowLRollField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        root.Q<Button>("ElbowLReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneEulerXYZ(GetBone("forearm.L"), elbowBase.x, elbowBase.y, elbowBase.z);
            root.Q<Slider>("ElbowLYaw").value = 0.0f;
            root.Q<FloatField>("ElbowLYawField").value = 0.0f;
            root.Q<Slider>("ElbowLRoll").value = 0.0f;
            root.Q<FloatField>("ElbowLRollField").value = 0.0f;
            elbowL.x = elbowLX;
            elbowL.y = elbowLY;
            pepperElbowJawL = root.Q<Slider>("ElbowLYaw").value;
            pepperElbowRollL = root.Q<Slider>("ElbowLRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #region ElbowR
        var elbowR = new Vector3(elbowBaseX, elbowBaseY, -elbowBaseZ);
        var elbowRY = elbowR.y;
        var elbowRX = elbowR.x;
        #region Elbow YAW
        float oldElbowRYawValue = 0.0f;
        float anglePrevious_ElbowYawR = 0.0f;
        root.Q<Slider>("ElbowRYaw").lowValue = -119.5f;
        root.Q<Slider>("ElbowRYaw").highValue = 119.5f;
        root.Q<Slider>("ElbowRYaw").value = 0.0f;
        root.Q<Slider>("ElbowRYaw").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            elbowR.y = elbowRY - root.Q<Slider>("ElbowRYaw").value - 180.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.R").transform.localEulerAngles;

            //set new bone value
            float angleDelta = root.Q<Slider>("ElbowRYaw").value - anglePrevious_ElbowYawR;
            anglePrevious_ElbowYawR = root.Q<Slider>("ElbowRYaw").value;
            GetBone("forearm.R").transform.Rotate(GetBone("upper_arm.R").transform.up, angleDelta, Space.World);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("forearm.R").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ElbowRYaw").value = oldElbowRYawValue;
            }
            else
            {
                oldElbowRYawValue = root.Q<Slider>("ElbowRYaw").value;
            }

            //sync
            SyncFieldToSlider(root, "ElbowRYawField", "ElbowRYaw");
            pepperElbowJawR = root.Q<Slider>("ElbowRYaw").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ElbowRYawField").value = 0.0f;
        root.Q<FloatField>("ElbowRYawField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ElbowRYawField").value = Clamp(root.Q<FloatField>("ElbowRYawField").value, -119.5f, 119.5f);

            //get field value
            elbowR.y = elbowRY - root.Q<FloatField>("ElbowRYawField").value - 180.0f;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.R").transform.localEulerAngles;

            //set new bone value
            GetBone("forearm.R").transform.localEulerAngles.Set(elbowR.x, elbowR.y, elbowR.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("forearm.R").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ElbowRYawField").value = oldElbowRYawValue;
            }
            else
            {
                oldElbowRYawValue = root.Q<FloatField>("ElbowRYawField").value;
            }

            //sync
            SyncSliderToField(root, "ElbowRYaw", "ElbowRYawField");
            pepperElbowJawR = root.Q<FloatField>("ElbowRYawField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #region Elbow Roll
        float oldElbowRRollValue = 0.0f;
        float anglePrevious_ElbowRollR = 0.0f;
        root.Q<Slider>("ElbowRRoll").lowValue = -89.5f;
        root.Q<Slider>("ElbowRRoll").highValue = 0.5f;
        root.Q<Slider>("ElbowRRoll").RegisterCallback<ChangeEvent<float>>(evt =>
        {

            //get slider value
            elbowR.x = elbowBaseX + root.Q<Slider>("ElbowRRoll").value;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.R").transform.localEulerAngles;

            //set new bone value
            float angleDelta = root.Q<Slider>("ElbowRRoll").value - anglePrevious_ElbowRollR;
            anglePrevious_ElbowRollR = root.Q<Slider>("ElbowRRoll").value;
            GetBone("forearm.R").transform.Rotate(-angleDelta, 0.0f, 0.0f, Space.Self);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("forearm.R").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<Slider>("ElbowRRoll").value = oldElbowRRollValue;
            }
            else
            {
                oldElbowRRollValue = root.Q<Slider>("ElbowRRoll").value;
            }

            //sync
            SyncFieldToSlider(root, "ElbowRRollField", "ElbowRRoll");
            pepperElbowRollR = root.Q<Slider>("ElbowRRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<FloatField>("ElbowRRollField").RegisterCallback<FocusOutEvent>(evt =>
        {
            //clamp input
            root.Q<FloatField>("ElbowRRollField").value = Clamp(root.Q<FloatField>("ElbowRRollField").value, -89.5f, 0.5f);

            //get field value
            elbowR.x = elbowRX + root.Q<FloatField>("ElbowRRollField").value;

            //save old bone value
            Vector3 oldAngles = GetBone("forearm.R").transform.localEulerAngles;

            //set new bone value
            GetBone("forearm.R").transform.localEulerAngles.Set(elbowR.x, elbowR.y, elbowR.z);

            //if arm now intersects the body, reset value
            if (CheckIntersection(rightArmCollider) && preventIntersection)
            {
                GetBone("forearm.R").transform.localEulerAngles.Set(oldAngles.x, oldAngles.y, oldAngles.z);
                root.Q<FloatField>("ElbowRRollField").value = oldElbowRRollValue;
            }
            else
            {
                oldElbowRRollValue = root.Q<FloatField>("ElbowRRollField").value;
            }

            //sync
            SyncSliderToField(root, "ElbowRRoll", "ElbowRRollField");
            pepperElbowRollR = root.Q<FloatField>("ElbowRRollField").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        root.Q<Button>("ElbowRReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneEulerXYZ(GetBone("forearm.R"), elbowBase.x, -elbowBase.y, -elbowBase.z);
            root.Q<Slider>("ElbowRYaw").value = 0.0f;
            root.Q<FloatField>("ElbowRYawField").value = 0.0f;
            root.Q<Slider>("ElbowRRoll").value = 0.0f;
            root.Q<FloatField>("ElbowRRollField").value = 0.0f;
            elbowR.x = elbowRX;
            elbowR.y = elbowRY;
            pepperElbowJawR = root.Q<Slider>("ElbowRYaw").value;
            pepperElbowRollR = root.Q<Slider>("ElbowRRoll").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #endregion

        #region Wrist Jaw
        float wristBaseX = 0.0f;
        float wristBaseY = 0.0f;
        float wristBaseZ = 180.0f;

        #region Wrist Jaw L
        var wristYawL = GetBone("hand.L").transform.localEulerAngles;
        var wristYawLY = wristYawL.y;
        root.Q<Slider>("WristLYaw").lowValue = -104.5f;
        root.Q<Slider>("WristLYaw").highValue = 104.5f;
        root.Q<Slider>("WristLYaw").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SyncFieldToSlider(root, "WristLYawField", "WristLYaw");
            wristYawL.x = GetBone("hand.L").transform.localEulerAngles.x;
            wristYawL.z = GetBone("hand.L").transform.localEulerAngles.z;
            wristYawL.y = wristYawLY + root.Q<Slider>("WristLYaw").value;
            GetBone("hand.L").transform.localRotation = Quaternion.Euler(wristYawL);
            pepperWristJawL = root.Q<Slider>("WristLYaw").value;
        });

        root.Q<FloatField>("WristLYawField").RegisterCallback<FocusOutEvent>(evt =>
        {
            root.Q<FloatField>("WristLYawField").value = Clamp(root.Q<FloatField>("WristLYawField").value, -104.5f, 104.5f);
            SyncSliderToField(root, "WristLYaw", "WristLYawField");
            wristYawL.x = GetBone("hand.L").transform.localEulerAngles.x;
            wristYawL.z = GetBone("hand.L").transform.localEulerAngles.z;
            wristYawL.y = wristYawLY + root.Q<FloatField>("WristLYawField").value;
            GetBone("hand.L").transform.localRotation = Quaternion.Euler(wristYawL);
            pepperWristJawL = root.Q<FloatField>("WristLYawField").value;
        });

        root.Q<Button>("WristLYawReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneEulerXYZ(GetBone("hand.L"), wristBase.x, wristBase.y, wristBase.z);
            root.Q<Slider>("WristLYaw").value = 0.0f;
            root.Q<FloatField>("WristLYawField").value = 0.0f;
            pepperWristJawL = root.Q<Slider>("WristLYaw").value;
        });
        #endregion

        #region Wrist Jaw R
        var wristYawR = GetBone("hand.R").transform.localEulerAngles;
        var wristYawRY = wristYawR.y;
        root.Q<Slider>("WristRYaw").lowValue = -104.5f;
        root.Q<Slider>("WristRYaw").highValue = 104.5f;
        root.Q<Slider>("WristRYaw").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SyncFieldToSlider(root, "WristRYawField", "WristRYaw");
            wristYawR.x = GetBone("hand.R").transform.localEulerAngles.x;
            wristYawR.z = GetBone("hand.R").transform.localEulerAngles.z;
            wristYawR.y = wristYawRY + root.Q<Slider>("WristRYaw").value;
            GetBone("hand.R").transform.localRotation = Quaternion.Euler(wristYawR);
            pepperWristJawR = root.Q<Slider>("WristRYaw").value;
        });

        root.Q<FloatField>("WristRYawField").RegisterCallback<FocusOutEvent>(evt =>
        {
            root.Q<FloatField>("WristRYawField").value = Clamp(root.Q<FloatField>("WristRYawField").value, -104.5f, 104.5f);
            SyncSliderToField(root, "WristRYaw", "WristRYawField");
            wristYawR.x = GetBone("hand.R").transform.localEulerAngles.x;
            wristYawR.z = GetBone("hand.R").transform.localEulerAngles.z;
            wristYawL.y = wristYawLY + root.Q<FloatField>("WristRYawField").value;
            GetBone("hand.R").transform.localRotation = Quaternion.Euler(wristYawR);
            pepperWristJawR = root.Q<FloatField>("WristRYawField").value;
        });

        root.Q<Button>("WristRYawReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            RotateBoneEulerXYZ(GetBone("hand.R"), wristBase.x, wristBase.y, wristBase.z);
            root.Q<Slider>("WristRYaw").value = 0.0f;
            root.Q<FloatField>("WristRYawField").value = 0.0f;
            pepperWristJawR = root.Q<Slider>("WristRYaw").value;
        });
        #endregion
        #endregion

        #region Hand 
        #region Hand L
        var fIndexL1 = GetBone("f_index.01.L").transform.localEulerAngles;
        var fIndexL2 = GetBone("f_index.02.L").transform.localEulerAngles;
        var fIndexL3 = GetBone("f_index.03.L").transform.localEulerAngles;

        var fMiddleL1 = GetBone("f_middle.01.L").transform.localEulerAngles;
        var fMiddleL2 = GetBone("f_middle.02.L").transform.localEulerAngles;
        var fMiddleL3 = GetBone("f_middle.03.L").transform.localEulerAngles;

        var fRingL1 = GetBone("f_ring.01.L").transform.localEulerAngles;
        var fRingL2 = GetBone("f_ring.02.L").transform.localEulerAngles;
        var fRingL3 = GetBone("f_ring.03.L").transform.localEulerAngles;

        var fPinkyL1 = GetBone("f_pinky.01.L").transform.localEulerAngles;
        var fPinkyL2 = GetBone("f_pinky.02.L").transform.localEulerAngles;
        var fPinkyL3 = GetBone("f_pinky.03.L").transform.localEulerAngles;

        var fThumbL1 = GetBone("thumb.01.L").transform.localEulerAngles;
        var fThumbL2 = GetBone("thumb.02.L").transform.localEulerAngles;

        leftHandValues.Add(fIndexL1);
        leftHandValues.Add(fIndexL2);
        leftHandValues.Add(fIndexL3);

        leftHandValues.Add(fMiddleL1);
        leftHandValues.Add(fMiddleL2);
        leftHandValues.Add(fMiddleL3);

        leftHandValues.Add(fRingL1);
        leftHandValues.Add(fRingL2);
        leftHandValues.Add(fRingL3);

        leftHandValues.Add(fPinkyL1);
        leftHandValues.Add(fPinkyL2);
        leftHandValues.Add(fPinkyL3);

        leftHandValues.Add(fThumbL1);
        leftHandValues.Add(fThumbL2);

        root.Q<Slider>("HandL").lowValue = 0.0f;
        root.Q<Slider>("HandL").highValue = 35.0f;
        root.Q<Slider>("HandL").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            float fIndexL1X = fIndexL1.x + root.Q<Slider>("HandL").value;
            float fIndexL2X = fIndexL2.x + root.Q<Slider>("HandL").value;
            float fIndexL3X = fIndexL3.x + root.Q<Slider>("HandL").value;

            float fMiddleL1X = fMiddleL1.x + root.Q<Slider>("HandL").value;
            float fMiddleL2X = fMiddleL2.x + root.Q<Slider>("HandL").value;
            float fMiddleL3X = fMiddleL3.x + root.Q<Slider>("HandL").value;

            float fRingL1X = fRingL1.x + root.Q<Slider>("HandL").value;
            float fRingL2X = fRingL2.x + root.Q<Slider>("HandL").value;
            float fRingL3X = fRingL3.x + root.Q<Slider>("HandL").value;

            float fPinkyL1X = fPinkyL1.x + root.Q<Slider>("HandL").value;
            float fPinkyL2X = fPinkyL2.x + root.Q<Slider>("HandL").value;
            float fPinkyL3X = fPinkyL3.x + root.Q<Slider>("HandL").value;

            float fThumbL1X = fThumbL1.x + root.Q<Slider>("HandL").value;
            float fThumbL2X = fThumbL2.x + root.Q<Slider>("HandL").value;

            SetBoneLocalEulerAngles(GetBone("f_index.01.L"), new Vector3(fIndexL1X, fIndexL1.y, fIndexL1.z));
            SetBoneLocalEulerAngles(GetBone("f_index.02.L"), new Vector3(fIndexL2X, fIndexL2.y, fIndexL2.z));
            SetBoneLocalEulerAngles(GetBone("f_index.03.L"), new Vector3(fIndexL3X, fIndexL3.y, fIndexL3.z));

            SetBoneLocalEulerAngles(GetBone("f_middle.01.L"), new Vector3(fMiddleL1X, fMiddleL1.y, fMiddleL1.z));
            SetBoneLocalEulerAngles(GetBone("f_middle.02.L"), new Vector3(fMiddleL2X, fMiddleL2.y, fMiddleL2.z));
            SetBoneLocalEulerAngles(GetBone("f_middle.03.L"), new Vector3(fMiddleL3X, fMiddleL3.y, fMiddleL3.z));

            SetBoneLocalEulerAngles(GetBone("f_ring.01.L"), new Vector3(fRingL1X, fRingL1.y, fRingL1.z));
            SetBoneLocalEulerAngles(GetBone("f_ring.02.L"), new Vector3(fRingL2X, fRingL2.y, fRingL2.z));
            SetBoneLocalEulerAngles(GetBone("f_ring.03.L"), new Vector3(fRingL3X, fRingL3.y, fRingL3.z));

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.L"), new Vector3(fPinkyL1X, fPinkyL1.y, fPinkyL1.z));
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.L"), new Vector3(fPinkyL2X, fPinkyL2.y, fPinkyL2.z));
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.L"), new Vector3(fPinkyL3X, fPinkyL3.y, fPinkyL3.z));

            SetBoneLocalEulerAngles(GetBone("thumb.01.L"), new Vector3(fThumbL1X, fThumbL1.y, fThumbL1.z));
            SetBoneLocalEulerAngles(GetBone("thumb.02.L"), new Vector3(fThumbL2X, fThumbL2.y, fThumbL2.z));

            pepperHandL = root.Q<Slider>("HandL").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HandLReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            SetBoneLocalEulerAngles(GetBone("f_index.01.L"), fIndexL1);
            SetBoneLocalEulerAngles(GetBone("f_index.02.L"), fIndexL2);
            SetBoneLocalEulerAngles(GetBone("f_index.03.L"), fIndexL3);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.L"), fMiddleL1);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.L"), fMiddleL2);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.L"), fMiddleL3);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.L"), fRingL1);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.L"), fRingL2);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.L"), fRingL3);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.L"), fPinkyL1);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.L"), fPinkyL2);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.L"), fPinkyL3);

            SetBoneLocalEulerAngles(GetBone("thumb.01.L"), fThumbL1);
            SetBoneLocalEulerAngles(GetBone("thumb.02.L"), fThumbL2);

            pepperHandL = 0.5f;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion

        #region Hand R
        var fIndexR1 = GetBone("f_index.01.R").transform.localEulerAngles;
        var fIndexR2 = GetBone("f_index.02.R").transform.localEulerAngles;
        var fIndexR3 = GetBone("f_index.03.R").transform.localEulerAngles;

        var fMiddleR1 = GetBone("f_middle.01.R").transform.localEulerAngles;
        var fMiddleR2 = GetBone("f_middle.02.R").transform.localEulerAngles;
        var fMiddleR3 = GetBone("f_middle.03.R").transform.localEulerAngles;

        var fRingR1 = GetBone("f_ring.01.R").transform.localEulerAngles;
        var fRingR2 = GetBone("f_ring.02.R").transform.localEulerAngles;
        var fRingR3 = GetBone("f_ring.03.R").transform.localEulerAngles;

        var fPinkyR1 = GetBone("f_pinky.01.R").transform.localEulerAngles;
        var fPinkyR2 = GetBone("f_pinky.02.R").transform.localEulerAngles;
        var fPinkyR3 = GetBone("f_pinky.03.R").transform.localEulerAngles;

        var fThumbR1 = GetBone("thumb.01.R").transform.localEulerAngles;
        var fThumbR2 = GetBone("thumb.02.R").transform.localEulerAngles;

        rightHandValues.Add(fIndexR1);
        rightHandValues.Add(fIndexR2);
        rightHandValues.Add(fIndexR3);

        rightHandValues.Add(fMiddleR1);
        rightHandValues.Add(fMiddleR2);
        rightHandValues.Add(fMiddleR3);

        rightHandValues.Add(fRingR1);
        rightHandValues.Add(fRingR2);
        rightHandValues.Add(fRingR3);

        rightHandValues.Add(fPinkyR1);
        rightHandValues.Add(fPinkyR2);
        rightHandValues.Add(fPinkyR3);

        rightHandValues.Add(fThumbR1);
        rightHandValues.Add(fThumbR2);

        root.Q<Slider>("HandR").lowValue = 0.0f;
        root.Q<Slider>("HandR").highValue = 35.0f;
        root.Q<Slider>("HandR").RegisterCallback<ChangeEvent<float>>(evt =>
        {
            //TODO use rightHandValues list

            float fIndexR1X = fIndexR1.x + root.Q<Slider>("HandR").value;
            float fIndexR2X = fIndexR2.x + root.Q<Slider>("HandR").value;
            float fIndexR3X = fIndexR3.x + root.Q<Slider>("HandR").value;

            float fMiddleR1X = fMiddleR1.x + root.Q<Slider>("HandR").value;
            float fMiddleR2X = fMiddleR2.x + root.Q<Slider>("HandR").value;
            float fMiddleR3X = fMiddleR3.x + root.Q<Slider>("HandR").value;

            float fRingR1X = fRingR1.x + root.Q<Slider>("HandR").value;
            float fRingR2X = fRingR2.x + root.Q<Slider>("HandR").value;
            float fRingR3X = fRingR3.x + root.Q<Slider>("HandR").value;

            float fPinkyR1X = fPinkyR1.x + root.Q<Slider>("HandR").value;
            float fPinkyR2X = fPinkyR2.x + root.Q<Slider>("HandR").value;
            float fPinkyR3X = fPinkyR3.x + root.Q<Slider>("HandR").value;

            float fThumbR1X = fThumbR1.x + root.Q<Slider>("HandR").value;
            float fThumbR2X = fThumbR2.x + root.Q<Slider>("HandR").value;

            SetBoneLocalEulerAngles(GetBone("f_index.01.R"), new Vector3(fIndexR1X, fIndexR1.y, fIndexR1.z));
            SetBoneLocalEulerAngles(GetBone("f_index.02.R"), new Vector3(fIndexR2X, fIndexR2.y, fIndexR2.z));
            SetBoneLocalEulerAngles(GetBone("f_index.03.R"), new Vector3(fIndexR3X, fIndexR3.y, fIndexR3.z));

            SetBoneLocalEulerAngles(GetBone("f_middle.01.R"), new Vector3(fMiddleR1X, fMiddleR1.y, fMiddleR1.z));
            SetBoneLocalEulerAngles(GetBone("f_middle.02.R"), new Vector3(fMiddleR2X, fMiddleR2.y, fMiddleR2.z));
            SetBoneLocalEulerAngles(GetBone("f_middle.03.R"), new Vector3(fMiddleR3X, fMiddleR3.y, fMiddleR3.z));

            SetBoneLocalEulerAngles(GetBone("f_ring.01.R"), new Vector3(fRingR1X, fRingR1.y, fRingR1.z));
            SetBoneLocalEulerAngles(GetBone("f_ring.02.R"), new Vector3(fRingR2X, fRingR2.y, fRingR2.z));
            SetBoneLocalEulerAngles(GetBone("f_ring.03.R"), new Vector3(fRingR3X, fRingR3.y, fRingR3.z));

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.R"), new Vector3(fPinkyR1X, fPinkyR1.y, fPinkyR1.z));
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.R"), new Vector3(fPinkyR2X, fPinkyR2.y, fPinkyR2.z));
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.R"), new Vector3(fPinkyR3X, fPinkyR3.y, fPinkyR3.z));

            SetBoneLocalEulerAngles(GetBone("thumb.01.R"), new Vector3(fThumbR1X, fThumbR1.y, fThumbR1.z));
            SetBoneLocalEulerAngles(GetBone("thumb.02.R"), new Vector3(fThumbR2X, fThumbR2.y, fThumbR2.z));

            pepperHandR = root.Q<Slider>("HandR").value;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });

        root.Q<Button>("HandRReset").RegisterCallback<MouseUpEvent>(evt =>
        {
            SetBoneLocalEulerAngles(GetBone("f_index.01.R"), fIndexR1);
            SetBoneLocalEulerAngles(GetBone("f_index.02.R"), fIndexR2);
            SetBoneLocalEulerAngles(GetBone("f_index.03.R"), fIndexR3);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.R"), fMiddleR1);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.R"), fMiddleR2);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.R"), fMiddleR3);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.R"), fRingR1);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.R"), fRingR2);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.R"), fRingR3);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.R"), fPinkyR1);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.R"), fPinkyR2);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.R"), fPinkyR3);

            SetBoneLocalEulerAngles(GetBone("thumb.01.R"), fThumbR1);
            SetBoneLocalEulerAngles(GetBone("thumb.02.R"), fThumbR2);

            pepperHandR = 0.5f;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        });
        #endregion
        #endregion

        #region Reset
        root.Q<Button>("ResetAll").RegisterCallback<MouseUpEvent>(evt =>
        {
            ResetAll();
        });

        void ResetAll()
        {
            RotateBoneX(GetBone("spine.001"), kneeBase.x);
            root.Q<Slider>("KneePitch").value = 0.0f;
            root.Q<FloatField>("KneePitchField").value = 0.0f;

            RotateBoneX(GetBone("spine.002"), headPitchBase.x);
            root.Q<Slider>("HipPitch").value = 0.0f;
            root.Q<FloatField>("HipPitchField").value = 0.0f;

            RotateBoneZ(GetBone("spine.002"), headPitchBase.z);
            root.Q<Slider>("HipRoll").value = 0.0f;
            root.Q<FloatField>("HipRollField").value = 0.0f;

            head.x = headPitchBase.x;
            GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
            root.Q<Slider>("HeadPitch").value = 0.0f;
            root.Q<FloatField>("HeadPitchField").value = 0.0f;

            head.y = 0.0f;
            GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
            root.Q<Slider>("HeadJaw").value = 0.0f;
            root.Q<FloatField>("HeadJawField").value = 0.0f;

            ResetLeftShoulder(autoSend, parser);
            ResetRightShoulder(autoSend, parser);

            RotateBoneEulerXYZ(GetBone("forearm.L"), elbowBaseX, elbowBaseY, elbowBaseZ);
            root.Q<Slider>("ElbowLYaw").value = 0.0f;
            root.Q<FloatField>("ElbowLYawField").value = 0.0f;
            root.Q<Slider>("ElbowLRoll").value = 0.0f;
            root.Q<FloatField>("ElbowLRollField").value = 0.0f;
            elbowL.x = elbowLX;
            elbowL.y = elbowLY;

            RotateBoneEulerXYZ(GetBone("forearm.R"), elbowBaseX, -elbowBaseY, -elbowBaseZ);
            root.Q<Slider>("ElbowRYaw").value = 0.0f;
            root.Q<FloatField>("ElbowRYawField").value = 0.0f;
            root.Q<Slider>("ElbowRRoll").value = 0.0f;
            root.Q<FloatField>("ElbowRRollField").value = 0.0f;
            elbowR.x = elbowRX;
            elbowR.y = elbowRY;

            RotateBoneEulerXYZ(GetBone("hand.L"), wristBaseX, wristBaseY, 0.0f);
            root.Q<Slider>("WristLYaw").value = 0.0f;
            root.Q<FloatField>("WristLYawField").value = 0.0f;
            pepperWristJawL = root.Q<Slider>("WristLYaw").value;

            RotateBoneEulerXYZ(GetBone("hand.R"), wristBaseX, wristBaseY, 0.0f);
            root.Q<Slider>("WristRYaw").value = 0.0f;
            root.Q<FloatField>("WristRYawField").value = 0.0f;
            pepperWristJawR = root.Q<Slider>("WristRYaw").value;

            SetBoneLocalEulerAngles(GetBone("f_index.01.L"), fIndexL1);
            SetBoneLocalEulerAngles(GetBone("f_index.02.L"), fIndexL2);
            SetBoneLocalEulerAngles(GetBone("f_index.03.L"), fIndexL3);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.L"), fMiddleL1);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.L"), fMiddleL2);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.L"), fMiddleL3);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.L"), fRingL1);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.L"), fRingL2);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.L"), fRingL3);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.L"), fPinkyL1);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.L"), fPinkyL2);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.L"), fPinkyL3);

            SetBoneLocalEulerAngles(GetBone("thumb.01.L"), fThumbL1);
            SetBoneLocalEulerAngles(GetBone("thumb.02.L"), fThumbL2);

            SetBoneLocalEulerAngles(GetBone("f_index.01.R"), fIndexR1);
            SetBoneLocalEulerAngles(GetBone("f_index.02.R"), fIndexR2);
            SetBoneLocalEulerAngles(GetBone("f_index.03.R"), fIndexR3);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.R"), fMiddleR1);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.R"), fMiddleR2);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.R"), fMiddleR3);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.R"), fRingR1);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.R"), fRingR2);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.R"), fRingR3);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.R"), fPinkyR1);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.R"), fPinkyR2);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.R"), fPinkyR3);

            SetBoneLocalEulerAngles(GetBone("thumb.01.R"), fThumbR1);
            SetBoneLocalEulerAngles(GetBone("thumb.02.R"), fThumbR2);

            pepperKneePitch = 0.0f;
            pepperHipPitch = 0.0f;
            pepperHipRoll = 0.0f;
            pepperHeadPitch = 0.0f;
            pepperHeadYaw = 0.0f;
            //left
            pepperShoulderPitchL = 0.0f;
            pepperShoulderRollL = 0.0f;
            pepperElbowJawL = 0.0f;
            pepperElbowRollL = 0.0f;
            pepperWristJawL = 0.0f;
            pepperHandL = 0.5f;
            //right
            pepperShoulderPitchR = 0.0f;
            pepperShoulderRollR = 0.0f;
            pepperElbowJawR = 0.0f;
            pepperElbowRollR = 0.0f;
            pepperWristJawR = 0.0f;
            pepperHandR = 0.5f;

            if (autoSend) { change = true; SendAllSliderValues(parser); }
        }
        #endregion

        #region Options
        root.Q<Toggle>("Option_PreventIntersection").value = preventIntersection;
        root.Q<Toggle>("Option_PreventIntersection").RegisterCallback<MouseUpEvent>(evt =>
        {
            preventIntersection = root.Q<Toggle>("Option_PreventIntersection").value;
        });

        root.Q<Toggle>("Option_AutoSend").value = autoSend;
        root.Q<Toggle>("Option_AutoSend").RegisterCallback<MouseUpEvent>(evt =>
        {
            autoSend = root.Q<Toggle>("Option_AutoSend").value;
        });
        #endregion
        #endregion

        #region Animation
        int currentFrame = 1;
        string filename = "PepperTest";
        root.Q<TextField>("AnimationName").RegisterCallback<ChangeEvent<string>>(evt => {
            filename = root.Q<TextField>("AnimationName").value;
        });

        root.Q<Button>("CreateAnimation").RegisterCallback<MouseUpEvent>(evt =>
        {
            AnimationClip clip = new AnimationClip();
            string path = "PepperBones/spine/spine.001";

            Keyframe keyframe = new Keyframe(0.0f, 100f);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(keyframe);

            //knee
            clip.SetCurve(path, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //hip
            path += "/spine.002";
            clip.SetCurve(path, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(path, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(path, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //head
            path += "/spine.003";
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, GetBone("spine.005").transform.localRotation.x, 1, GetBone("spine.005").transform.localRotation.x));
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, GetBone("spine.005").transform.localRotation.y, 1, GetBone("spine.005").transform.localRotation.y));
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, GetBone("spine.005").transform.localRotation.z, 1, GetBone("spine.005").transform.localRotation.z));

            //upper arm l
            string pathl = path + "/shoulder.L/upper_arm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, GetBone("upper_arm.L").transform.localRotation.x, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, GetBone("upper_arm.L").transform.localRotation.y, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, GetBone("upper_arm.L").transform.localRotation.z, 100, 0));

            //forearm l
            pathl += "/forearm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //hand l
            pathl += "/hand.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //fingers l
            string tmpPathl = pathl;
            for(int i = 1; i < 4; i++)
            {
                pathl += $"/f_index.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_middle.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_pinky.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_ring.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 3; i++)
            {
                pathl += $"/thumb.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }

            //upper arm r
            string pathr = path + "/shoulder.R/upper_arm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //forearm r
            pathr += "/forearm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //hand r
            pathr += "/hand.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));

            //fingers r
            string tmpPathr = pathr;
            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_index.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_middle.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_pinky.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_ring.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 3; i++)
            {
                pathr += $"/thumb.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, 0, 100, 0));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, 100, 0));
            }

            AssetDatabase.CreateAsset(clip, $"Assets/Resources/PepperModel/Animation/{filename}.anim");

            Animator animator = GameObject.Find("pepper_prefab").GetComponent<Animator>();
            var runtimeController = animator.runtimeAnimatorController;
            var controller = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(runtimeController));

            controller.AddMotion(clip);
        });

        root.Q<Button>("CurrentPoseToKeyframe").RegisterCallback<MouseUpEvent>(evt =>
        {
            AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath($"Assets/Resources/PepperModel/Animation/{filename}.anim", typeof(AnimationClip));

            //AssetDatabase.CreateAsset(curve, $"Assets/Resources/PepperModel/Animation/{filename}.curve");

            string path = "PepperBones/spine/spine.001";

            curve.AddKey(currentFrame, GetBone("spine.001").transform.localRotation.x);

            clip.SetCurve(path, typeof(Transform), "localRotation.x", curve);

            //hip
            path += "/spine.002";
            clip.SetCurve(path, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(path, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(path, typeof(Transform), "localRotation.z", curve);

            //head
            path += "/spine.003";
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(path + "/spine.004/spine.005", typeof(Transform), "localRotation.z", curve);

            //upper arm l
            string pathl = path + "/shoulder.L/upper_arm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);

            //forearm l
            pathl += "/forearm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);

            //hand l
            pathl += "/hand.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);

            //fingers l
            string tmpPathl = pathl;
            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_index.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_middle.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_pinky.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_ring.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);
            }
            pathl = tmpPathl;

            for (int i = 1; i < 3; i++)
            {
                pathl += $"/thumb.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", curve);
            }

            //upper arm r
            string pathr = path + "/shoulder.R/upper_arm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);

            //forearm r
            pathr += "/forearm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);

            //hand r
            pathr += "/hand.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);

            //fingers r
            string tmpPathr = pathr;
            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_index.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_middle.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_pinky.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_ring.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);
            }
            pathr = tmpPathr;

            for (int i = 1; i < 3; i++)
            {
                pathr += $"/thumb.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", curve);
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", curve);
            }

            Animator animator = GameObject.Find("pepper_prefab").GetComponent<Animator>();
            var runtimeController = animator.runtimeAnimatorController;
            var controller = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(runtimeController));

            controller.AddMotion(clip);

            currentFrame++;
            root.Q<IntegerField>("CurrentKeyframe").value = currentFrame;
        });

        root.Q<IntegerField>("CurrentKeyframe").RegisterCallback<ChangeEvent<int>>(evt =>
        {
            currentFrame = root.Q<IntegerField>("CurrentKeyframe").value;
        });
        #endregion

        #region Pepper Connection
        root.Q<TextField>("IP").RegisterCallback<ChangeEvent<string>>(evt =>
        {
            IP = root.Q<TextField>("IP").value;
        });

        root.Q<Button>("Connect").RegisterCallback<MouseUpEvent>(evt =>
        {
            /*
            client = new TCPClientTopic();
            Queue<string> messageQueue = new Queue<string>();
            client.ConnectToTcpServer();

            speechCommand sc;
            sc.type = "speak";
            sc.content = "Verbunden mit Unity";
            client.SendMessage(JsonUtility.ToJson(sc));*/
            bool IDERun = root.Q<Toggle>("IDE").value;
            client = new TCPClientTopic(IP);
            general = new General(client);
            if (!IDERun)
            {
                general.StartScriptRunner(IP);
            }
            else
            {
                general.StartIDE(IP);
            }
            motorControl = new MotorControl(client);
            client.SubscribeToMessageReceived(this);
        });

        root.Q<Button>("Disconnect").RegisterCallback<MouseUpEvent>(evt =>
        {
            client.SendMessage("dc");
        });

        root.Q<Button>("Sleep").RegisterCallback<MouseUpEvent>(evt =>
        {
            motorControl.Sleep();
        });

        root.Q<Button>("WakeUp").RegisterCallback<MouseUpEvent>(evt =>
        {
            motorControl.WakeUp();
        });

        root.Q<Button>("SendPose").RegisterCallback<MouseUpEvent>(evt =>
        {
            SendAllSliderValues(parser);
        });

        root.Q<Button>("ReceivePose").RegisterCallback<MouseUpEvent>(evt =>
        {
            ResetAll();

            MotorContent motor_content = new MotorContent(MotorContent.MOTOR_COMMAND.RECEIVE);
            TCPContent tcp_content = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, motor_content);
            client.SendMessage(tcp_content.toJSONMessage());

            WaitAndCheckLastMessage();
        });

        #endregion

        //Save Peppers current pose to textfile
        root.Q<Button>("PoseToMotorValues").RegisterCallback<MouseUpEvent>(evt =>
        {
            currentModelToPose();
        });

        //Load Pepper pose from textfile
        root.Q<Button>("PoseValue").RegisterCallback<MouseUpEvent>(evt =>
        {
            PepperPose test = currentPoseToMotorValues();

            Debug.Log("Knee Pitch: " + test.pepperKneePitch);
            Debug.Log("Hip Pitch: " + test.pepperHipPitch); //TODO
            Debug.Log("Hip Roll: " + test.pepperHipRoll);
            Debug.Log("Head Pitch: " + test.pepperHeadPitch);
            Debug.Log("Head Jaw: " + test.pepperHeadYaw);

            //left
            Debug.Log("Shoulder Pitch: " + test.pepperShoulderPitchL);
            Debug.Log("Shoulder Roll: " + test.pepperShoulderRollL);
            Debug.Log("Elbow Jaw: " + test.pepperElbowYawL);
            Debug.Log("Elbow Roll: " + test.pepperElbowRollL);
            Debug.Log("Wrist Jaw: " + test.pepperWristJawL);

            /*
            root.Q<Slider>("KneePitch").value = test.pepperKneePitch;
            root.Q<Slider>("HipPitch").value = test.pepperHipPitch;
            root.Q<Slider>("HipRoll").value = test.pepperHipRoll;
            root.Q<Slider>("HeadPitch").value = test.pepperHeadPitch;
            root.Q<Slider>("HeadJaw").value = test.pepperHeadYaw;

            root.Q<Slider>("ElbowLYaw").value = test.pepperElbowYawL;
            root.Q<Slider>("ElbowLRoll").value = test.pepperElbowRollL;
            root.Q<Slider>("ElbowRYaw").value = test.pepperElbowYawR;
            root.Q<Slider>("ElbowRRoll").value = test.pepperElbowRollR;
            root.Q<Slider>("WristLYaw").value = test.pepperWristJawL;
            root.Q<Slider>("WristRYaw").value = test.pepperWristJawR;
            */
            applyPepperPose(test, root, kneeBase.x, hipPitchBase.x, head, shoulderBase, elbowBase, elbowL, elbowR, wristBase, new List<Vector3>(), new List<Vector3>());
        });
}
    

    #region Bone operations
    GameObject GetBone(string boneName)
    {
        if(pepper != null)
        {
            return pepper.GetComponent<PepperBoneReference>().getBone(boneName);
        }
        else
        {
            Debug.Log("NO PEPPER PREFAB ASSIGNED");
            Debug.Log("Trying to find Pepper prefab");
            pepper = GameObject.FindGameObjectWithTag("Pepper");

            if(pepper == null)
            {
                Debug.Log("NO PEPPER PREFAB FOUND IN SCENE");
                return null;
            }
            else
            {
                Debug.Log("Pepper prefab found");
                return pepper.GetComponent<PepperBoneReference>().getBone(boneName);
            }
        }
        //return GameObject.Find(boneName);
    }

    void RotateBoneX(GameObject bone, float x)
    {
        bone.transform.localRotation =
            new Quaternion(
                (x / 2.0f) * Mathf.Deg2Rad,
                bone.transform.localRotation.y,
                bone.transform.localRotation.z,
                bone.transform.localRotation.w);
    }

    void SetBoneEulerAngles(GameObject bone, Vector3 angles)
    {
        bone.transform.eulerAngles.Set(angles.x, angles.y, angles.z);
    }

    void SetBoneLocalEulerAngles(GameObject bone, Vector3 angles)
    {
        bone.transform.localEulerAngles = angles;
    }

    void RotateBoneZ(GameObject bone, float z)
    {
        bone.transform.localRotation =
            new Quaternion(
                bone.transform.localRotation.x,
                bone.transform.localRotation.y,
                (z /2.0f) * Mathf.Deg2Rad,
                bone.transform.localRotation.w);
    }

    void RotateBoneEulerXYZ(GameObject bone, float x, float y, float z)
    {
        Debug.Log(bone.name + " " + x + " " + y + " " + z);
        bone.transform.localEulerAngles.Set(x, y, z);

        Debug.Log(bone.transform.localRotation);
        Debug.Log(bone.transform.localEulerAngles);
    }
    #endregion

    #region limiter
    //Head pitch limiter
    #region Head
    float headPitchLimited(float headYaw, float headPitch)
    {
        if (headYaw < -91.4f)
        {
            return Clamp(headPitch, -35.0f, 13.0f);
        }
        if (headYaw < -61.6f)
        {
            return Clamp(headPitch, -35.1f, 13.5f);
        }
        if (headYaw < -33.33f)
        {
            return Clamp(headPitch, -35.2f, 20.9f);
        }
        if ((headYaw >= -33.33f) && (headYaw <= 33.33f))
        {
            return Clamp(headPitch, -40.5f, 25.5f);
        }
        if(headYaw < 61.6f)
        {
            return Clamp(headPitch, -35.2f, 20.9f);
        }
        if (headYaw < 91.4f)
        {
            return Clamp(headPitch, -35.1f, 13.5f);
        }
        return Clamp(headPitch, -35.0f, 13.5f);
    }

    float headYawLimited(float headPitch, float headYaw)
    {
        if (headPitch >= -35.0f && headPitch <= 13.5f)
        {
            return Clamp(headYaw, -119.5f, 119.5f);
        }
        if (headPitch >= -35.1f && headPitch <= 13.5f)
        {
            return Clamp(headYaw, -91.4f, 91.4f);
        }
        if (headPitch >= -35.2f && headPitch <= 20.9f)
        {
            return Clamp(headYaw, -61.6f, 61.6f);
        }
        return Clamp(headYaw, -33.33f, 33.33f);
    }
    #endregion
    #endregion

    #region Utility
    bool CheckIntersection(BoxCollider armCollider)
    {
        /*
        Vector3 halfExtends = new Vector3(armCollider.bounds.extents.x / 4.0f, armCollider.bounds.extents.y / 4.0f, armCollider.bounds.extents.z / 4.0f);
        Collider[] tmpColliders = Physics.OverlapBox(armCollider.transform.position, halfExtends, armCollider.transform.rotation);
        if (tmpColliders.Length > 1)
        {
            return true;
        } */
        
        return false;
    }

    void SyncSliderToField(VisualElement root, string sliderName, string fieldName)
    {
        root.Q<Slider>(sliderName).value = root.Q<FloatField>(fieldName).value;
    }

    void SyncFieldToSlider(VisualElement root, string fieldName, string sliderName)
    {
        root.Q<FloatField>(fieldName).value = root.Q<Slider>(sliderName).value;
    }

    float Clamp(float input, float min, float max)
    {
        if (input < min) input = min;
        if (input > max) input = max;

        return input;
    }
    #endregion

    #region TCP
    void SendAllSliderValues(MotorValueParser parser)
    {
        if (checkTimer())
        {
            MotorContent motor_content = new MotorContent(
                MotorContent.MOTOR_COMMAND.SET,

                -pepperKneePitch,
                -pepperHipPitch,
                pepperHipRoll,
                pepperHeadPitch,
                -pepperHeadYaw,

                pepperShoulderPitchL,
                pepperShoulderRollL,
                pepperShoulderPitchR,
                -pepperShoulderRollR,

                pepperElbowJawL,
                pepperElbowRollL,
                -pepperElbowJawR,
                -pepperElbowRollR,

                -pepperWristJawL,
                -pepperWristJawR,

                pepperHandL,
                pepperHandR
                );
            TCPContent tcp_content = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOTOR_CONTROL, 1, motor_content);
            client.SendMessage(tcp_content.toJSONMessage());

            //string message = parser.AllValuesToMessage(currentSliderValuesToPepperPose());
            //Debug.Log(message);
            //client.SendMessage(message);
            change = false;
        }
    }

    PepperPose currentSliderValuesToPepperPose()
    {
        PepperPose pepperPose;

        pepperPose.pepperKneePitch = -pepperKneePitch;
        pepperPose.pepperHipPitch = -pepperHipPitch;
        pepperPose.pepperHipRoll = pepperHipRoll;
        pepperPose.pepperHeadPitch = pepperHeadPitch;
        pepperPose.pepperHeadYaw = -pepperHeadYaw;
        //left
        pepperPose.pepperShoulderPitchL = pepperShoulderPitchL;
        pepperPose.pepperShoulderRollL = pepperShoulderRollL;
        pepperPose.pepperElbowYawL = pepperElbowJawL;
        pepperPose.pepperElbowRollL = pepperElbowRollL;
        pepperPose.pepperWristJawL = -pepperWristJawL;

        pepperPose.pepperHandLOpen = pepperHandL;

        //right
        pepperPose.pepperShoulderPitchR = pepperShoulderPitchR;
        pepperPose.pepperShoulderRollR = -pepperShoulderRollR;
        pepperPose.pepperElbowYawR = -pepperElbowJawR;
        pepperPose.pepperElbowRollR = -pepperElbowRollR;
        pepperPose.pepperWristJawR = -pepperWristJawR;

        pepperPose.pepperHandROpen = pepperHandR;

        return pepperPose;
    }

    bool checkTimer()
    {
        if(timer > timerThreshhold)
        {
            timer = 0.0f;
            return true;
        }
        return false;
    }
    #endregion

    void currentModelToPose()
    {
        var knee = GetBone("spine.001").transform.localEulerAngles;
        var hip = GetBone("spine.002").transform.localEulerAngles;
        var head = GetBone("spine.005").transform.localEulerAngles;

        var shoulderL = GetBone("upper_arm.L").transform.localEulerAngles;
        var elbowL = GetBone("forearm.L").transform.localEulerAngles;
        var wristL = GetBone("hand.L").transform.localEulerAngles;
        //var handL = GetBone("upper_arm.L").transform.eulerAngles;

        var shoulderR = GetBone("upper_arm.R").transform.localEulerAngles;
        var elbowR = GetBone("forearm.R").transform.localEulerAngles;
        var wristR = GetBone("hand.R").transform.localEulerAngles;
        //var handR = GetBone("upper_arm.L").transform.eulerAngles;


        string data = $"{knee.x}/{knee.y}/{knee.z}_{hip.x}/{hip.y}/{hip.z}_{head.x}/{head.y}/{head.z}_{shoulderL.x}/{shoulderL.y}/{shoulderL.z}_{elbowL.x}/{elbowL.y}/{elbowL.z}_{wristL.x}/{wristL.y}/{wristL.z}_{shoulderR.x}/{shoulderR.y}/{shoulderR.z}_{elbowR.x}/{elbowR.y}/{elbowR.z}_{wristR.x}/{wristR.y}/{wristR.z}";

        FileReaderWriter.WriteToFile(filename, data, true);

        Debug.Log("Write to file: " + filename);
    }

    PepperPose currentPoseToMotorValues()
    {
        Debug.Log("Read from file: " + filename);

        string data = FileReaderWriter.ReadFromFile(filename);

        string[] split = data.Split('_');

        PepperPose pepperPose;

        string[] knee = split[0].Split('/');
        pepperPose.pepperKneePitch = Mathf.Round(float.Parse(knee[0]) - GetBone("spine.001").transform.localEulerAngles.x);

        string[] hip = split[1].Split('/');
        pepperPose.pepperHipPitch = Mathf.Round(float.Parse(hip[0]) - GetBone("spine.001").transform.localEulerAngles.x);
        pepperPose.pepperHipRoll = Mathf.Round(float.Parse(hip[2]) - GetBone("spine.001").transform.localEulerAngles.z);

        string[] head = split[2].Split('/');
        pepperPose.pepperHeadPitch = Mathf.Round(float.Parse(head[0]) - GetBone("spine.005").transform.localEulerAngles.x);
        pepperPose.pepperHeadYaw = Mathf.Round(float.Parse(head[1]) - GetBone("spine.005").transform.localEulerAngles.y);

        //left
        string[] shoulderL = split[3].Split('/');
        pepperPose.pepperShoulderPitchL = Mathf.Round(float.Parse(shoulderL[0]) - GetBone("upper_arm.L").transform.localEulerAngles.x);
        pepperPose.pepperShoulderRollL = Mathf.Round(float.Parse(shoulderL[2]) - GetBone("upper_arm.L").transform.localEulerAngles.z);

        string[] elbowL = split[4].Split('/');
        pepperPose.pepperElbowYawL = Mathf.Round(float.Parse(elbowL[1]) - GetBone("forearm.L").transform.localEulerAngles.y);
        pepperPose.pepperElbowRollL = Mathf.Round(float.Parse(elbowL[0]) - GetBone("forearm.L").transform.localEulerAngles.x);

        string[] wristL = split[5].Split('/');
        pepperPose.pepperWristJawL = Mathf.Round(float.Parse(wristL[1]) - GetBone("hand.L").transform.localEulerAngles.y);

        //right
        string[] shoulderR = split[6].Split('/');
        pepperPose.pepperShoulderPitchR = Mathf.Round(float.Parse(shoulderR[0]) - GetBone("upper_arm.R").transform.localEulerAngles.x);
        pepperPose.pepperShoulderRollR = Mathf.Round(float.Parse(shoulderR[2]) - GetBone("upper_arm.R").transform.localEulerAngles.z);

        string[] elbowR = split[7].Split('/');
        pepperPose.pepperElbowYawR = Mathf.Round(float.Parse(elbowR[1]) - GetBone("forearm.R").transform.localEulerAngles.y);
        pepperPose.pepperElbowRollR = Mathf.Round(float.Parse(elbowR[0]) - GetBone("forearm.R").transform.localEulerAngles.x);

        string[] wristR = split[8].Split('/');
        pepperPose.pepperWristJawR = Mathf.Round(float.Parse(wristR[1]) - GetBone("hand.R").transform.localEulerAngles.y);

        pepperPose.pepperHandLOpen = 0.5f;
        pepperPose.pepperHandROpen = 0.5f;

        return pepperPose;
    }

    void applyPepperPose(PepperPose pepperPose, 
        VisualElement root, 
        float kneeBase, 
        float hipPitchBase, 
        Vector3 head, 
        Vector3 shoulderBase, 
        Vector3 elbowBase, 
        Vector3 elbowL, 
        Vector3 elbowR, 
        Vector3 wristBase,
        List<Vector3> HandL,
        List<Vector3> HandR)
    {
        RotateBoneX(GetBone("spine.001"), kneeBase + pepperPose.pepperKneePitch);
        root.Q<Slider>("KneePitch").value = pepperPose.pepperKneePitch;
        root.Q<FloatField>("KneePitchField").value = pepperPose.pepperKneePitch;

        RotateBoneX(GetBone("spine.002"), hipPitchBase + root.Q<Slider>("HipPitch").value);
        root.Q<Slider>("HipPitch").value = pepperPose.pepperHipPitch;
        root.Q<FloatField>("HipPitchField").value = pepperPose.pepperHipPitch;

        RotateBoneZ(GetBone("spine.002"), pepperPose.pepperHipRoll);
        root.Q<Slider>("HipRoll").value = pepperPose.pepperHipRoll;
        root.Q<FloatField>("HipRollField").value = pepperPose.pepperHipRoll;

        head.x = pepperPose.pepperHeadPitch;
        GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
        root.Q<Slider>("HeadPitch").value = pepperPose.pepperHeadPitch;
        root.Q<FloatField>("HeadPitchField").value = pepperPose.pepperHeadPitch;

        head.y = pepperPose.pepperHeadYaw;
        GetBone("spine.005").transform.localEulerAngles.Set(head.x, head.y, head.z);
        root.Q<Slider>("HeadJaw").value = pepperPose.pepperHeadYaw;
        root.Q<FloatField>("HeadJawField").value = pepperPose.pepperHeadYaw;

        GetBone("upper_arm.L").transform.Rotate(GetBone("spine").transform.right, pepperPose.pepperShoulderPitchL, Space.World);
        GetBone("upper_arm.L").transform.Rotate(0.0f, 0.0f, -pepperPose.pepperShoulderRollL, Space.Self);
        root.Q<Slider>("ShoulderLPitch").value = pepperPose.pepperShoulderPitchL;
        root.Q<FloatField>("ShoulderLPitchField").value = pepperPose.pepperShoulderPitchL;
        root.Q<Slider>("ShoulderLRoll").value = 0.0f + pepperPose.pepperShoulderRollL;
        root.Q<FloatField>("ShoulderLRollField").value = 0.0f + pepperPose.pepperShoulderRollL;

        GetBone("upper_arm.R").transform.Rotate(GetBone("spine").transform.right, pepperPose.pepperShoulderPitchR, Space.World);
        GetBone("upper_arm.R").transform.Rotate(0.0f, 0.0f, -pepperPose.pepperShoulderRollR, Space.Self);
        root.Q<Slider>("ShoulderRPitch").value = pepperPose.pepperShoulderPitchR;
        root.Q<FloatField>("ShoulderRPitchField").value = pepperPose.pepperShoulderPitchR;
        root.Q<Slider>("ShoulderRRoll").value = 0.0f + pepperPose.pepperShoulderRollR;
        root.Q<FloatField>("ShoulderRRollField").value = 0.0f + pepperPose.pepperShoulderRollR;

        GetBone("forearm.L").transform.Rotate(GetBone("upper_arm.L").transform.up, -root.Q<Slider>("ElbowLYaw").value, Space.World);
        GetBone("forearm.L").transform.Rotate(-root.Q<Slider>("ElbowLRoll").value, 0.0f, 0.0f, Space.Self);
        root.Q<Slider>("ElbowLYaw").value = pepperPose.pepperElbowYawL;
        root.Q<FloatField>("ElbowLYawField").value = pepperPose.pepperElbowYawL;
        root.Q<Slider>("ElbowLRoll").value = pepperPose.pepperElbowRollL;
        root.Q<FloatField>("ElbowLRollField").value = pepperPose.pepperElbowRollL;
        elbowL.x = pepperPose.pepperElbowRollL;
        elbowL.y = pepperPose.pepperElbowYawL;

        GetBone("forearm.R").transform.Rotate(GetBone("upper_arm.R").transform.up, -root.Q<Slider>("ElbowRYaw").value, Space.World);
        GetBone("forearm.R").transform.Rotate(-root.Q<Slider>("ElbowRRoll").value, 0.0f, 0.0f, Space.Self);
        root.Q<Slider>("ElbowRYaw").value = pepperPose.pepperElbowYawR;
        root.Q<FloatField>("ElbowRYawField").value = pepperPose.pepperElbowYawR;
        root.Q<Slider>("ElbowRRoll").value = pepperPose.pepperElbowRollR;
        root.Q<FloatField>("ElbowRRollField").value = pepperPose.pepperElbowRollR;
        elbowR.x = pepperPose.pepperElbowRollR;
        elbowR.y = pepperPose.pepperElbowYawR;

        RotateBoneEulerXYZ(GetBone("hand.L"), wristBase.x, wristBase.y + pepperPose.pepperWristJawL, 0.0f);
        root.Q<Slider>("WristLYaw").value = pepperPose.pepperWristJawL;
        root.Q<FloatField>("WristLYawField").value = pepperPose.pepperWristJawL;

        RotateBoneEulerXYZ(GetBone("hand.R"), wristBase.x, wristBase.y + pepperPose.pepperWristJawR, 0.0f);
        root.Q<Slider>("WristRYaw").value = pepperPose.pepperWristJawR;
        root.Q<FloatField>("WristRYawField").value = pepperPose.pepperWristJawR;

        float handLOffset = 0.5f;

        List<Vector3> handLValues = new List<Vector3>();
        foreach (Vector3 finger in HandL)
        {
            handLValues.Add(new Vector3(finger.x + handLOffset, finger.y, finger.z));
        }

        if (handLValues.Count > 0)
        {
            SetBoneLocalEulerAngles(GetBone("f_index.01.L"), handLValues[0]);
            SetBoneLocalEulerAngles(GetBone("f_index.02.L"), handLValues[1]);
            SetBoneLocalEulerAngles(GetBone("f_index.03.L"), handLValues[2]);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.L"), handLValues[3]);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.L"), handLValues[4]);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.L"), handLValues[5]);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.L"), handLValues[6]);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.L"), handLValues[7]);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.L"), handLValues[8]);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.L"), handLValues[9]);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.L"), handLValues[10]);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.L"), handLValues[11]);

            SetBoneLocalEulerAngles(GetBone("thumb.01.L"), handLValues[12]);
            SetBoneLocalEulerAngles(GetBone("thumb.02.L"), handLValues[13]);
        }

        float handROffset = 0.5f;

        List<Vector3> handRValues = new List<Vector3>();
        foreach (Vector3 finger in HandR)
        {
            handRValues.Add(new Vector3(finger.x + handROffset, finger.y, finger.z));
        }

        if(handRValues.Count > 0)
        {
            SetBoneLocalEulerAngles(GetBone("f_index.01.R"), handRValues[0]);
            SetBoneLocalEulerAngles(GetBone("f_index.02.R"), handRValues[1]);
            SetBoneLocalEulerAngles(GetBone("f_index.03.R"), handRValues[2]);

            SetBoneLocalEulerAngles(GetBone("f_middle.01.R"), handRValues[3]);
            SetBoneLocalEulerAngles(GetBone("f_middle.02.R"), handRValues[4]);
            SetBoneLocalEulerAngles(GetBone("f_middle.03.R"), handRValues[5]);

            SetBoneLocalEulerAngles(GetBone("f_ring.01.R"), handRValues[6]);
            SetBoneLocalEulerAngles(GetBone("f_ring.02.R"), handRValues[7]);
            SetBoneLocalEulerAngles(GetBone("f_ring.03.R"), handRValues[8]);

            SetBoneLocalEulerAngles(GetBone("f_pinky.01.R"), handRValues[9]);
            SetBoneLocalEulerAngles(GetBone("f_pinky.02.R"), handRValues[10]);
            SetBoneLocalEulerAngles(GetBone("f_pinky.03.R"), handRValues[11]);

            SetBoneLocalEulerAngles(GetBone("thumb.01.R"), handRValues[12]);
            SetBoneLocalEulerAngles(GetBone("thumb.02.R"), handRValues[13]);
        }

        pepperKneePitch = pepperPose.pepperKneePitch;
        pepperHipPitch = pepperPose.pepperHipPitch;
        pepperHipRoll = pepperPose.pepperHipRoll;
        pepperHeadPitch = pepperPose.pepperHeadPitch;
        pepperHeadYaw = pepperPose.pepperHeadYaw;
        //left
        pepperShoulderPitchL = pepperPose.pepperShoulderPitchL;
        pepperShoulderRollL = pepperPose.pepperShoulderRollL;
        pepperElbowJawL = pepperPose.pepperElbowYawL;
        pepperElbowRollL = pepperPose.pepperElbowRollL;
        pepperWristJawL = pepperPose.pepperWristJawL;
        pepperHandL = 0.5f;
        //right
        pepperShoulderPitchR = pepperPose.pepperShoulderPitchR;
        pepperShoulderRollR = pepperPose.pepperShoulderRollR;
        pepperElbowJawR = pepperPose.pepperElbowYawR;
        pepperElbowRollR = pepperPose.pepperElbowRollR;
        pepperWristJawR = pepperPose.pepperWristJawR;
        pepperHandR = 0.5f;
    }

    private void SaveStringToFile(string path, string data)
    {
        if (!File.Exists(path))
        {
            File.Create(path);
        }

        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(data);
        writer.Close();
    }

    private string LoadStringFromFile(string path)
    {
        string data;
        StreamReader reader = new StreamReader(path);
        data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    void wait()
    {
        Debug.Log("waiting");
        Thread.Sleep(5000);
        Debug.Log("waited");
        ConnectionContent connectionContent = new ConnectionContent(IP, 9559);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.INIT, TCPContent.SUBTOPIC.NONE, 1, connectionContent);
        client.SendMessage(tcpContent.toJSONMessage());
        Thread.Sleep(5000);
        client.SubscribeToMessageReceived(this);
    }

    int secondsBetweenKeyframes = 50;
    int maxKeyframes = 500;
    string filename = "test_file";

    private void CreateKeyFrames()
    {
        AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath($"Assets/Resources/PepperModel/Animation/{filename}.anim", typeof(AnimationClip));

        Thread keyFrameThread = new Thread(() => CreateKeyFrame(secondsBetweenKeyframes, maxKeyframes, clip));
        keyFrameThread.Start();
    }

    void CreateKeyFrame(int secondsBetweenKeyframes, int maxKeyframes, AnimationClip clip)
    {
        for (var j = 0; j < maxKeyframes; j++)
        {
            string path = "PepperRig/PepperBones/spine/spine.001";
            //knee
            clip.SetCurve(path, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("spine.001").transform.localRotation.x)));

            //hip
            path += "/spine.002";
            clip.SetCurve(path, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("spine.002").transform.localRotation.x)));
            clip.SetCurve(path, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("spine.002").transform.localRotation.y)));
            clip.SetCurve(path, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("spine.002").transform.localRotation.z)));

            //head
            path += "/spine.003";
            clip.SetCurve(path + "/spine.005", typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("spine.005").transform.localRotation.x)));
            clip.SetCurve(path + "/spine.005", typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("spine.005").transform.localRotation.y)));
            clip.SetCurve(path + "/spine.005", typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("spine.005").transform.localRotation.z)));

            //upper arm l
            string pathl = path + "/shoulder.L/upper_arm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.L").transform.localRotation.x)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.L").transform.localRotation.y)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.L").transform.localRotation.z)));

            //forearm l
            pathl += "/forearm.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("forearm.L").transform.localRotation.x)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("forearm.L").transform.localRotation.y)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("forearm.L").transform.localRotation.z)));

            //hand l
            pathl += "/hand.L";
            clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("hand.L").transform.localRotation.x)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("hand.L").transform.localRotation.y)));
            clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("hand.L").transform.localRotation.z)));

            //fingers l
            string tmpPathl = pathl;
            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_index.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.L").transform.localRotation.x)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.L").transform.localRotation.y)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.L").transform.localRotation.z)));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_middle.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.L").transform.localRotation.x)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.L").transform.localRotation.y)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.L").transform.localRotation.z)));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_pinky.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.L").transform.localRotation.x)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.L").transform.localRotation.y)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.L").transform.localRotation.z)));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 4; i++)
            {
                pathl += $"/f_ring.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.L").transform.localRotation.x)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.L").transform.localRotation.y)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.L").transform.localRotation.z)));
            }
            pathl = tmpPathl;

            for (int i = 1; i < 3; i++)
            {
                pathl += $"/thumb.0{i}.L";
                clip.SetCurve(pathl, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.L").transform.localRotation.x)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.L").transform.localRotation.y)));
                clip.SetCurve(pathl, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.L").transform.localRotation.z)));
            }

            //upper arm r
            string pathr = path + "/shoulder.R/upper_arm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.R").transform.localRotation.x)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.R").transform.localRotation.y)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("upper_arm.R").transform.localRotation.z)));

            //forearm r
            pathr += "/forearm.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("forearm.R").transform.localRotation.x)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("forearm.R").transform.localRotation.y)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("forearm.R").transform.localRotation.z)));

            //hand r
            pathr += "/hand.R";
            clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone("hand.R").transform.localRotation.x)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone("hand.R").transform.localRotation.y)));
            clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone("hand.R").transform.localRotation.z)));

            //fingers r
            string tmpPathr = pathr;
            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_index.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.R").transform.localRotation.x)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.R").transform.localRotation.y)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_index.0{i}.R").transform.localRotation.z)));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_middle.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.R").transform.localRotation.x)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.R").transform.localRotation.y)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_middle.0{i}.R").transform.localRotation.z)));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_pinky.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.R").transform.localRotation.x)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.R").transform.localRotation.y)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_pinky.0{i}.R").transform.localRotation.z)));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 4; i++)
            {
                pathr += $"/f_ring.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.R").transform.localRotation.x)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.R").transform.localRotation.y)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"f_ring.0{i}.R").transform.localRotation.z)));
            }
            pathr = tmpPathr;

            for (int i = 1; i < 3; i++)
            {
                pathr += $"/thumb.0{i}.R";
                clip.SetCurve(pathr, typeof(Transform), "localRotation.x", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.R").transform.localRotation.x)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.y", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.R").transform.localRotation.y)));
                clip.SetCurve(pathr, typeof(Transform), "localRotation.z", new AnimationCurve(new Keyframe(j, GetBone($"thumb.0{i}.R").transform.localRotation.z)));
            }

            Thread.Sleep(secondsBetweenKeyframes);
        }
    }

    void ResetLeftShoulder(bool autoSend, MotorValueParser parser)
    {
        GetBone("upper_arm.L").transform.localEulerAngles.Set(shoulderBase.x, shoulderBase.y, shoulderBase.z); //Use 'Set' instead of '=' 
        root.Q<Slider>("ShoulderLPitch").value = 0.0f;
        root.Q<FloatField>("ShoulderLPitchField").value = 0.0f;
        root.Q<Slider>("ShoulderLRoll").value = 0.0f;
        root.Q<FloatField>("ShoulderLRollField").value = 0.0f;

        pepperShoulderPitchL = root.Q<Slider>("ShoulderLPitch").value;
        pepperShoulderRollL = root.Q<Slider>("ShoulderLRoll").value;

        if (autoSend) { change = true; SendAllSliderValues(parser); }
    }

    void ResetRightShoulder(bool autoSend, MotorValueParser parser)
    {
        GetBone("upper_arm.R").transform.localEulerAngles.Set(shoulderBase.x, -shoulderBase.y, -shoulderBase.z);
        root.Q<Slider>("ShoulderRPitch").value = 0.0f;
        root.Q<FloatField>("ShoulderRPitchField").value = 0.0f;
        root.Q<Slider>("ShoulderRRoll").value = 0.0f;
        root.Q<FloatField>("ShoulderRRollField").value = 0.0f;

        pepperShoulderPitchR = root.Q<Slider>("ShoulderRPitch").value;
        pepperShoulderRollR = root.Q<Slider>("ShoulderRRoll").value;

        if (autoSend) { change = true; SendAllSliderValues(parser); }
    }

    string lastMessage = "";
    private void ReceivePoseSleep()
    {
        Thread.Sleep(1000);
    }

    public void ReceiveMessage(string message)
    {
        lastMessage = message;
    }

    void WaitAndCheckLastMessage()
    {

        if (lastMessage.Contains("\"type\" : \"getmv\""))
        {
            try
            {
                JsonUtility.FromJson<PepperPose>(client.lastMessage);
            }
            catch (Exception e)
            {
                Debug.Log("Not JSON");
                Debug.Log(e);
                return;
            }

            PepperPose p = JsonUtility.FromJson<PepperPose>(client.lastMessage);
            Debug.Log("PAE recveived: " + p.pepperKneePitch);

            applyPepperPose(p, root, kneeBase.x, hipPitchBase.x, head,
                new Vector3(shoulderBase.x, shoulderBase.y, shoulderBase.z),
                new Vector3(elbowBase.x, elbowBase.y, elbowBase.z),
                elbowBase, elbowBase,
                new Vector3(wristBase.x, wristBase.y, wristBase.z),
                leftHandValues, rightHandValues);

            lastMessage = "";
        }
    }

    public void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();
        lastMessage = message;
        ReceiveMessage(lastMessage);
    }
}