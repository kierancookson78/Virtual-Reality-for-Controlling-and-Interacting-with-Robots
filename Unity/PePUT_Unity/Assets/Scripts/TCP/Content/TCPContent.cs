using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TCPContent
{
    public enum PRIORITY
    {
        NONE,
        LOW,
        MID,
        HIGH
    }

    public enum TOPIC
    {
        BASIC,
        TEST_TOPIC,
        INIT,
        ENVIRONMENT,
        USER,
        ROBOT,
        EXTERNAL_TOOLS
    }
    public enum SUBTOPIC
    {
        TEST_SUBTOPIC,
        NONE,

        ENVIRONMENT_LIGHT,
        ENVIRONMENT_FAN,
        ENVIRONMENT_WINDOW,

        USER_MOOD,
        USER_POSITION,

        ROBOT_MOVE,
        ROBOT_MOTOR_CONTROL,
        ROBOT_SPEECH,
        ROBOT_BEHAVIOR_TOOL,
        ROBOT_TABLET,
        ROBOT_MEMORY_DATA,
        ROBOT_AUTONOMY,

        EXTERNAL_TOOLS_WEATHER,
        EXTERNAL_TOOLS_NEWS,
        EXTERNAL_TOOLS_SPEECH_RECOGNITION,
        EXTERNAL_TOOLS_USER_POSITION,
    }

    private Dictionary<TOPIC, string> topicLookup = new Dictionary<TOPIC, string>()
    {
        {TOPIC.BASIC, "basic" },
        {TOPIC.TEST_TOPIC, "testTopic" },
        {TOPIC.INIT, "init" },
        {TOPIC.ENVIRONMENT, "environment" },
        {TOPIC.USER, "user" },
        {TOPIC.ROBOT, "robot" },
        {TOPIC.EXTERNAL_TOOLS, "externalTools" },
    };

    private Dictionary<SUBTOPIC, string> subtopicLookup = new Dictionary<SUBTOPIC, string>()
    {
        {SUBTOPIC.TEST_SUBTOPIC, "testSubtopic" },
        {SUBTOPIC.NONE, "none" },

        {SUBTOPIC.ENVIRONMENT_LIGHT, "light" },
        {SUBTOPIC.ENVIRONMENT_FAN, "fan" },
        {SUBTOPIC.ENVIRONMENT_WINDOW, "window" },

        {SUBTOPIC.USER_MOOD, "mood" },
        {SUBTOPIC.USER_POSITION, "userPosition" },

        {SUBTOPIC.ROBOT_MOVE, "move" },
        {SUBTOPIC.ROBOT_SPEECH, "speech" },
        {SUBTOPIC.ROBOT_MOTOR_CONTROL, "motorControl" },
        {SUBTOPIC.ROBOT_BEHAVIOR_TOOL , "behaviorTool" },
        {SUBTOPIC.ROBOT_TABLET, "tablet" },
        {SUBTOPIC.ROBOT_MEMORY_DATA, "memory_data" },
        {SUBTOPIC.ROBOT_AUTONOMY, "robotAutonomy" },

        {SUBTOPIC.EXTERNAL_TOOLS_WEATHER, "weather" },
        {SUBTOPIC.EXTERNAL_TOOLS_NEWS, "news" },
        {SUBTOPIC.EXTERNAL_TOOLS_SPEECH_RECOGNITION, "speechRecognition" },
        {SUBTOPIC.EXTERNAL_TOOLS_USER_POSITION, "userPosition" },
    };

    public string topic;
    public string subtopic;
    public int priority;
    public Content content;

    public TCPContent(TOPIC topic, SUBTOPIC subtopic, int priority, Content content)
    {
        this.topic = topicLookup[topic];
        this.subtopic = subtopicLookup[subtopic];
        this.priority = priority;
        this.content = content;
    }

    public string toJSONMessage()
    {
        string message;
        message = "{\"topic\":\"" + topic + "\",\"subtopic\":\"" + subtopic + "\",\"priority\":\"" + priority + "\",\"content\":" + content.toJSONString() + "}";
        return message;
    }
}

