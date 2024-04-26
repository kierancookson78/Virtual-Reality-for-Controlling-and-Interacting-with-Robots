using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorContent : Content
{
    public enum BEHAVIOR_COMMAND
    {
        HELP,
        START,
        START_PREVIOUS,
        STOP,
        STOP_ALL,
        LIST,
        RUNNING,
        GET_BEHAVIORS,
        GET_DEFAULT_BEHAVIORS,
        ADD_DEFAULT_BEHAVIOR,
        LAUNCH_AND_STOP_BEHAVIOR,
    }

    private Dictionary<BEHAVIOR_COMMAND, string> behaviorCommandLookup = new Dictionary<BEHAVIOR_COMMAND, string>()
    {
        {BEHAVIOR_COMMAND.HELP, "help" },
        {BEHAVIOR_COMMAND.START, "start" },
        {BEHAVIOR_COMMAND.START_PREVIOUS, "startPrevious" },
        {BEHAVIOR_COMMAND.STOP, "stop" },
        {BEHAVIOR_COMMAND.STOP_ALL, "stopall" },
        {BEHAVIOR_COMMAND.LIST, "list" },
        {BEHAVIOR_COMMAND.RUNNING, "running" },
        {BEHAVIOR_COMMAND.GET_BEHAVIORS, "getBehavior" },
        {BEHAVIOR_COMMAND.GET_DEFAULT_BEHAVIORS, "getDefaultBehaviors" },
        {BEHAVIOR_COMMAND.ADD_DEFAULT_BEHAVIOR, "addDefaultBehavior" },
        {BEHAVIOR_COMMAND.LAUNCH_AND_STOP_BEHAVIOR, "launchAndStopBehavior" }
    };
    public string behavior_command;
    public string behaviorName;
    public bool async;

    public BehaviorContent(BEHAVIOR_COMMAND behavior_command, string behaviorName = "", bool async = false)
    {
        this.behavior_command = behaviorCommandLookup[behavior_command];
        this.behaviorName = behaviorName;
        this.async = async;
    }
}
