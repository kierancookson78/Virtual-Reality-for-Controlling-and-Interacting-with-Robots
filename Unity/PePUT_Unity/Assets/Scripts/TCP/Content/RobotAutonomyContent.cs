using System.Collections.Generic;

public class RobotAutonomyContent : Content
{
    public enum AUTONOMY_MODE
    {
        SOLITARY,
        INTERACTIVE,
        SAFEGUARD,
        DISABLED
    }

    public static Dictionary<AUTONOMY_MODE, string> autonomyModeLookup = new Dictionary<AUTONOMY_MODE, string>()
    {
        {AUTONOMY_MODE.SOLITARY , "solitary"},
        {AUTONOMY_MODE.INTERACTIVE, "interactive" },
        {AUTONOMY_MODE.SAFEGUARD, "safeguard" },
        {AUTONOMY_MODE.DISABLED, "disabled" }
    };

    public enum Command
    {
        SET_AUTONOMY,
        AUTONOMOUS_BLINKING,
        BACKGROUND_MOVEMENT,
        BASIC_AWARENESS,
        LISTENING_MOVEMENT,
        SPEAKING_MOVEMENT,
    }

    public enum Subcommand
    {
        //general
        set_enabled,
        is_enabled,
        is_running,

        //basic awareness
        pause_awareness,
        resume_awareness,
        is_awareness_paused,
        is_stimulus_detection_enabled,
        set_stimulus_detection_enabled,
        set_engagement_mode,
        get_engagement_mode,
        engage_person,
        trigger_stimulus,
        set_tracking_mode,
        get_tracking_mode,
        set_parameter,
        get_parameter,
        reset_all_parameters,

        //speaking movement
        set_mode,
        get_mode,
        add_tags_to_words,
        reset_tags_to_words,
    }

    public enum StimTypes
    {
        People,
        Touch,
        TabletTouch,
        Sound,
        Movement,
        NavigationMotion
    }

    public enum TrackingModes
    {
        Head,
        BodyRotation,
        WholeBody,
        MoveContextually
    }

    public enum EngagementModes
    {
        Unengaged,
        FullyEngaged,
        SemiEngaged
    }

    public enum ParamNames
    {
        LookStimulusSpeed,
        LookBackSpeed
    }

    public enum SpeakingMovementModes
    {
        random,
        contextual
    }

    public string command;
    public string subcommand;
    public string value;
    public string secondary_value;

    public RobotAutonomyContent(AUTONOMY_MODE autonomy_mode)
    {
        value = autonomyModeLookup[autonomy_mode];
        command = Command.SET_AUTONOMY.ToString();

        secondary_value = "";
        subcommand = "";
    }

    public RobotAutonomyContent(Command command, Subcommand subcommand)
    {
        this.command = command.ToString();
        this.subcommand = subcommand.ToString();
        value = "";
        secondary_value = "";
    }

    public RobotAutonomyContent(Command command, Subcommand subcommand, string value)
    {
        this.command = command.ToString();
        this.subcommand = subcommand.ToString();
        this.value = value;
        secondary_value = "";
    }

    public RobotAutonomyContent(Command command, Subcommand subcommand, string value, string secondary_value)
    {
        this.command = command.ToString();
        this.subcommand = subcommand.ToString();
        this.value = value;
        this.secondary_value = secondary_value;
    }
}
