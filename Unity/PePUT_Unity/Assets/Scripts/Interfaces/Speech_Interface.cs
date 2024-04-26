using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using System.Globalization;
using System.Threading;

public class Speech_Interface : MonoBehaviour, TCPMessageSubscriber
{
    public struct SpeechInfo
    {
        public string speech_status;
        public string current_sentence;
    }

    [SerializeField] GeneralSettings generalSettings;

    [SerializeField] InputField sayText;
    [SerializeField] InputField readFromFile;

    Speech speech;
    private void Start()
    {
        Thread waitForClientThread = new Thread(() => waitForClient());
        waitForClientThread.Start();
    }

    private void waitForClient()
    {
        while(generalSettings.client == null)
        {
            continue;
        }
        speech = new Speech(generalSettings.client);
        speech.Subscribe(this);
    }

    #region Speech Settings
    [SerializeField] Dropdown language;
    [SerializeField] Dropdown voice;
    [SerializeField] Slider volume;
    [SerializeField] Slider speed;
    [SerializeField] Slider pitch;

    public void OnLanguageChanged()
    {
        speech.SetLanguage((SpeechContent.LANGUAGE)language.value);
    }

    public void OnVoiceChanged()
    {
        speech.SetVoice((SpeechContent.VOICE)voice.value);
    }

    public void OnVolumeChanged()
    {
        Debug.Log(volume.value);
        speech.SetVolume(volume.value);
    }

    public void OnSpeedChanged()
    {
        speech.SetSpeed(speed.value);
    }

    public void OnPitchChanged()
    {
        speech.SetPitch(pitch.value);
    }
    #endregion

    #region Speech
    #region Sample Buttons
    public void SaySampleText_1()
    {
        switch ((SpeechContent.LANGUAGE)language.value)
        {
            case SpeechContent.LANGUAGE.GERMAN:
                SendSpeechCommand("Hallo mein name ist Pepper! Willkommen bei PePUT", TCPContent.PRIORITY.LOW);
                break;
            case SpeechContent.LANGUAGE.ENGLISH:
                SendSpeechCommand("Hello my name is Pepper! welcome to PePUT", TCPContent.PRIORITY.LOW);
                break;
            case SpeechContent.LANGUAGE.CHINESE:
                SendSpeechCommand("Ni hao PePUT", TCPContent.PRIORITY.LOW);
                break;
        }
    }
    public void SaySampleText_2()
    {
        switch ((SpeechContent.LANGUAGE)language.value)
        {
            case SpeechContent.LANGUAGE.GERMAN:
                SendSpeechCommand("PePUT steht fuer Pepper Paifon Unity Toolkitt", TCPContent.PRIORITY.MID);
                break;
            case SpeechContent.LANGUAGE.ENGLISH:
                SendSpeechCommand("PePUT stands for Pepper Python Unity Toolkit", TCPContent.PRIORITY.MID);
                break;
            case SpeechContent.LANGUAGE.CHINESE:
                SendSpeechCommand("PePUT - Pepper Python Unity Toolkit", TCPContent.PRIORITY.MID);
                break;
        }
    }
    public void SaySampleText_3()
    {
        switch ((SpeechContent.LANGUAGE)language.value)
        {
            case SpeechContent.LANGUAGE.GERMAN:
                SendSpeechCommand("PePUT macht es moeglich Pepper ganz einfach ueber Unity zu steuern", TCPContent.PRIORITY.HIGH);
                break;
            case SpeechContent.LANGUAGE.ENGLISH:
                SendSpeechCommand("PePUT makes it possible to easily control Pepper over Unity", TCPContent.PRIORITY.HIGH);
                break;
            case SpeechContent.LANGUAGE.CHINESE:
                SendSpeechCommand("Tongguo Unity kongzhi Pepper", TCPContent.PRIORITY.HIGH);
                break;
        }
    }

    public void StopSpeech()
    {
        speech.StopSpeech();
    }
    #endregion
    #region Sample Buttons
    public void SayText_InputField()
    {
        SendSpeechCommand(sayText.text, TCPContent.PRIORITY.LOW);
    }

    public void SayText_TextFile()
    {
        speech.SayTextFromTextFile(readFromFile.text);
    }
    #endregion

    private void SendSpeechCommand(string text, TCPContent.PRIORITY priority = TCPContent.PRIORITY.LOW)
    {
        speech.StartSpeech(text, priority);
    }

    public void ReceiveMessage<SpeechInfo>(ref SpeechInfo speechInfo)
    {

    }
    #endregion
}
