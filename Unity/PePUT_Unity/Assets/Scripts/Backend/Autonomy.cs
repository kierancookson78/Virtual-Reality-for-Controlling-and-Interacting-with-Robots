using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autonomy : MessageClient
{
    public Autonomy(TCPClientTopic client) : base(client)
    {
    }

    public void SetAutonomousMode(RobotAutonomyContent.AUTONOMY_MODE autonomyMode)
    {
        RobotAutonomyContent robotAutonomyContent = new RobotAutonomyContent(autonomyMode);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_AUTONOMY, 1, robotAutonomyContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    #region Autonomous Blinking
    public void AutonomousBlinking_SetEnabled(bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.AUTONOMOUS_BLINKING, RobotAutonomyContent.Subcommand.set_enabled, enable.ToString()));
    }

    public void AutonomousBlinking_IsEnabled()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.AUTONOMOUS_BLINKING, RobotAutonomyContent.Subcommand.is_enabled));
    }
    #endregion

    #region Background Movement
    public void BackgroundMovement_SetEnabled(bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BACKGROUND_MOVEMENT, RobotAutonomyContent.Subcommand.set_enabled, enable.ToString()));
    }

    public void BackgroundMovement_IsEnabled()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BACKGROUND_MOVEMENT, RobotAutonomyContent.Subcommand.is_enabled));
    }

    public void BackgroundMovement_IsRunning()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BACKGROUND_MOVEMENT, RobotAutonomyContent.Subcommand.is_running));
    }
    #endregion

    #region Basic Awareness
    public void BasicAwareness_SetEnabled(bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.set_enabled, enable.ToString()));
    }

    public void BasicAwareness_IsEnabled()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.is_enabled));
    }

    public void BasicAwareness_IsRunning()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.is_running));
    }

    public void BasicAwareness_PauseAwareness()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.pause_awareness));
    }

    public void BasicAwareness_ResumeAwareness()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.resume_awareness));
    }

    public void BasicAwareness_IsAwarenessPaused()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.is_awareness_paused));
    }

    public void BasicAwareness_IsStimulusDetectionEnabled(RobotAutonomyContent.StimTypes stimType)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.is_stimulus_detection_enabled, stimType.ToString()));
    }

    public void BasicAwareness_SetStimulusDetectionEnabled(RobotAutonomyContent.StimTypes stimType, bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.set_stimulus_detection_enabled, stimType.ToString(), enable.ToString()));
    }

    public void BasicAwareness_SetEngagementMode(RobotAutonomyContent.EngagementModes engagementMode)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.set_engagement_mode, engagementMode.ToString()));
    }

    public void BasicAwareness_GetEngagementMode()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.get_engagement_mode));
    }

    public void BasicAwareness_EngagePerson(int personID)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.engage_person, personID.ToString()));
    }

    public void BasicAwareness_TriggerStimulus(string position_frame_world)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.trigger_stimulus, position_frame_world));
    }

    public void BasicAwareness_SetTrackingMode(RobotAutonomyContent.TrackingModes trackingMode)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.set_tracking_mode, trackingMode.ToString()));
    }

    public void BasicAwareness_GetTrackingMode()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.get_tracking_mode));
    }

    public void BasicAwareness_SetParameter(RobotAutonomyContent.ParamNames paramName, float paramValue)
    {
        paramValue = Mathf.Clamp(paramValue, 0.0001f, 1.0f);

        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.set_parameter, paramName.ToString(), paramValue.ToString()));
    }

    public void BasicAwareness_GetParameter(RobotAutonomyContent.ParamNames paramName)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.get_parameter, paramName.ToString()));
    }


    public void BasicAwareness_resetAllParameters()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.BASIC_AWARENESS, RobotAutonomyContent.Subcommand.reset_all_parameters));
    }
    #endregion

    #region Listening Movement
    public void ListeningMovement_SetEnabled(bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.LISTENING_MOVEMENT, RobotAutonomyContent.Subcommand.set_enabled, enable.ToString()));
    }

    public void ListeningMovement_IsEnabled()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.LISTENING_MOVEMENT, RobotAutonomyContent.Subcommand.is_enabled));
    }

    public void ListeningMovement_IsRunning()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.LISTENING_MOVEMENT, RobotAutonomyContent.Subcommand.is_running));
    }
    #endregion

    #region Speaking Movement
    public void SpeakingMovement_SetEnabled(bool enable)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.set_enabled, enable.ToString()));
    }

    public void SpeakingMovement_IsEnabled()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.is_enabled));
    }

    public void SpeakingMovement_IsRunning()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.is_running));
    }

    public void SpeakingMovement_SetMode(RobotAutonomyContent.SpeakingMovementModes mode)
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.set_mode, mode.ToString()));
    }

    public void SpeakingMovement_GetMode()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.get_mode));
    }

    public void SpeakingMovement_AddTagsToWords(string tag, string[] words) //example: tag -> "hello" : ["hey", "yo", "testword"] <- words
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.add_tags_to_words, tag, words.ToString()));
    }

    public void SpeakingMovement_ResetTagsToWords()
    {
        SendAutonomyContent(new RobotAutonomyContent(RobotAutonomyContent.Command.SPEAKING_MOVEMENT, RobotAutonomyContent.Subcommand.reset_tags_to_words));
    }
    #endregion

    private void SendAutonomyContent(RobotAutonomyContent robotAutonomyContent)
    {
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_AUTONOMY, 1, robotAutonomyContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public override void ReceiveMessage<T>(ref T message)
    {

    }

    protected override void NotifySubscribers<T>(ref T message)
    {

    }
}
