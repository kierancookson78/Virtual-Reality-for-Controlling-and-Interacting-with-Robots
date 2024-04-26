using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Animation_Interface : MonoBehaviour, TCPMessageSubscriber
{
    [SerializeField] GeneralSettings generalSettings;

    [SerializeField] InputField sayText;
    [SerializeField] InputField readFromFile;

    #region Animation Settings
    [SerializeField] Slider speed;
    [SerializeField] Dropdown category_dropdown;
    [SerializeField] Dropdown animation_dropdown;

    BehaviorTool behaviorTool;
    MotorControl motorControl;
    private void Start()
    {
        Thread waitForClientThread = new Thread(() => waitForClient());
        waitForClientThread.Start();
    }

    private void waitForClient()
    {
        while (generalSettings.client == null)
        {
            continue;
        }
        behaviorTool = new BehaviorTool(generalSettings.client);
        behaviorTool.Subscribe(this);

        motorControl = new MotorControl(generalSettings.client);
        motorControl.Subscribe(this);
    }
    #endregion

    #region Animation
    #region Sample Buttons
    public void SampleAnimation_1() //Hey
    {
        behaviorTool.SampleBehavior_1();

    }
    public void SampleAnimation_2() //Happy
    {
        behaviorTool.SampleBehavior_2();
    }
    public void SampleAnimation_3() //Embarrassed
    {
        behaviorTool.SampleBehavior_3();
    }

    #endregion
    public void GetBehaviorList()
    {
        behaviorTool.GetBehaviorList();
    }

    bool shouldFill = false;
    public void ReceiveMessage<BehaviorInfo>(ref BehaviorInfo behaviorInfo)
    {
        shouldFill = true;
    }

    private void Update()
    {
        if (shouldFill)
        {
            FillAnimationList();
            shouldFill = false;
        }
    }

    public void FillAnimationList()
    {
        switch (category_dropdown.options[category_dropdown.value].text)
        {
            case "BodyTalk":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.bodyTalkInfo.values);
                break;
            case "Emotions":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.emotionsInfo.values);
                break;
            case "Gestures":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.gesturesInfo.values);
                break;
            case "Reactions":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.reactionsInfo.values);
                break;
            case "Waiting":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.waitingInfo.values);
                break;
            case "Misc":
                animation_dropdown.ClearOptions();
                animation_dropdown.AddOptions(behaviorTool.miscInfo.values);
                break;
        }
    }

    private string GetAnimationNameFromDropdownValue()
    {
        if (category_dropdown.options[category_dropdown.value].text == "Misc")
        {
            string animation_name = animation_dropdown.options[animation_dropdown.value].text;
            return animation_name;
        }
        else
        {
            string animation_name = animation_dropdown.options[animation_dropdown.value].text;
            string full_name = "animations/Stand/" + animation_name;

            return full_name;
        }
    }

    public void PlaySelectedAnimation()
    {
        behaviorTool.PlayBehavior(GetAnimationNameFromDropdownValue());
    }

    public void StopAllBehaviors()
    {
        behaviorTool.StopAllBehaviors();
    }

    public void StopSelectedBehavior()
    {
        behaviorTool.StopBehavior(GetAnimationNameFromDropdownValue());
    }

    public void StandInit()
    {
        motorControl.StandInit();
    }
    #endregion
}
