using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpeechContent : Content
{
    public enum LANGUAGE { 
        GERMAN,
        ENGLISH,
        CHINESE
    }

    public enum VOICE
    {
        ANNA,
        NAOENU,
        NAOMNC
    }

    private Dictionary<LANGUAGE, string> languageLookup = new Dictionary<LANGUAGE, string>()
    {
        {LANGUAGE.GERMAN, "German" },
        {LANGUAGE.ENGLISH, "English" },
        {LANGUAGE.CHINESE, "Chinese" },
    };

    private Dictionary<VOICE, string> voiceLookup = new Dictionary<VOICE, string>()
    {
        {VOICE.ANNA, "anna" },
        {VOICE.NAOENU, "naoenu" },
        {VOICE.NAOMNC, "naomnc" },
    };

    public enum Command
    {
        SPEAK,
        VOLUME,
        LANGUAGE,
        VOICE,
        SPEED,
        PITCH,
        STOP,
        SKIP_QUEUE,
        CLEAR_QUEUE,
        SEND_ALL,
    }

    public string command;
    public string value;

    public SpeechContent(Command command, string value)
    {
        this.command = command.ToString();
        this.value = value;
    }

    public SpeechContent(Command command, LANGUAGE language)
    {
        this.command = command.ToString();
        this.value = languageLookup[language];
    }

    public SpeechContent(Command command, VOICE voice)
    {
        this.command = command.ToString();
        this.value = voiceLookup[voice];
    }

    public SpeechContent(Command command)
    {
        this.command = command.ToString();
    }

    public SpeechContent(Command command, string text, float volume, LANGUAGE language, VOICE voice, float pitch, float speed)
    {
        this.command = command.ToString();
        this.value = "{\"text\":\"" + text + "\",\"volume\":\"" + volume + "\",\"language\":\"" + language + "\",\"voice\":" + voice.ToString() + "\",\"pitch\":\"" + pitch + "\",\"speed\":\"" + speed + "}";
    }
}
