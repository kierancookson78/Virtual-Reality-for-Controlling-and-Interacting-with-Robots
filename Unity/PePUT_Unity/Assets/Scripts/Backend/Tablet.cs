using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet : MessageClient
{
    public struct TabletInput
    {
        public string inputText;
    }
    TabletInput currentInput;

    public Tablet(TCPClientTopic client) : base(client)
    {
    }

    #region Wifi
    public void EnableWifi()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_enableWifi);
        SendTabletContent(tabletContent);
    }

    public void DisableWifi()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_disableWifi);
        SendTabletContent(tabletContent);
    }

    public void ConsolePrintWifiStatus()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_getWifiStatus);
        SendTabletContent(tabletContent);
    }

    public void ConfigureWifi(string networkName, TabletContent.SECURITY security, string networkPassword)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_configureWifi, networkName, security, networkPassword);
        SendTabletContent(tabletContent);
    }

    public void ConnectWifi(string networkName)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_connectWifi, networkName);
        SendTabletContent(tabletContent);
    }

    public void DisconnectWifi()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.wifi_disconnectWifi);
        SendTabletContent(tabletContent);
    }
    #endregion

    #region Web View
    public void ShowWebView(string url)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.web_showWebView, url);
        SendTabletContent(tabletContent);
    }

    public void HideWebView()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.web_hideWebView);
        SendTabletContent(tabletContent);
    }

    public void LoadApplication(string applicationName)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.web_loadApplication, applicationName);
        SendTabletContent(tabletContent);
    }

    public void PrintInstalledApplications()
    {
        if (client == null)
        {
            Debug.LogError("CLIENT NOT INITIALIZED");
            return;
        }
        BehaviorContent behaviorContent = new BehaviorContent(BehaviorContent.BEHAVIOR_COMMAND.LIST);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_BEHAVIOR_TOOL, 1, behaviorContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public void LoadUrl(string url)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.web_loadUrl, url);
        SendTabletContent(tabletContent);
    }

    public void ReloadWebPage(bool bypassCache = true)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.web_reloadPage, bypassCache);
        SendTabletContent(tabletContent);
    }
    #endregion

    #region Video
    public void PlayVideo(string url)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.video_playVideo, url);
        SendTabletContent(tabletContent);
    }

    public void StopVideo()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.video_stopVideo);
        SendTabletContent(tabletContent);
    }

    public void PauseVideo()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.video_pauseVideo);
        SendTabletContent(tabletContent);
    }

    public void ResumeVideo()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.video_resumeVideo);
        SendTabletContent(tabletContent);
    }
    #endregion

    #region Image
    public void ShowImage(string url)
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.image_showImage, url);
        SendTabletContent(tabletContent);
    }

    public void HideImage()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.image_hideImage);
        SendTabletContent(tabletContent);
    }
    #endregion

    #region Input Dialog
    public void ShowInputDialog(TabletContent.TYPE type, string title, string okText = "ok", string cancelText = "cancel")
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.dialog_showInputDialog, type, title, okText, cancelText);
        SendTabletContent(tabletContent);
    }

    public void HideInputDialog()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.dialog_hideDialog);
        SendTabletContent(tabletContent);
    }
    #endregion

    public void CloseTabletBrowser()
    {
        TabletContent tabletContent = new TabletContent(TabletContent.TABLET_COMMAND.lowLevel_closeTabletBrowser);
        SendTabletContent(tabletContent);
    }

    private void SendTabletContent(TabletContent tabletContent)
    {
        if (client == null)
        {
            Debug.LogError("CLIENT NOT INITIALIZED");
            return;
        }
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_TABLET, 1, tabletContent);
        client.SendMessage(tcpContent.toJSONMessage());
    }

    public override void ReceiveMessage<T>(ref T message_T)
    {
        string message = message_T.ToString();
        Debug.Log(message);

        try
        {
            JsonUtility.FromJson<TabletInput>(message);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        TabletInput input = JsonUtility.FromJson<TabletInput>(message);
        if (input.inputText == "error::canceled")
        {
            TabletInputCanceled();
            return;
        }
        Debug.Log(input.inputText);
        currentInput = input;

        NotifySubscribers(ref input);
    }

    private void TabletInputCanceled()
    {
        Debug.Log("Input was canceled");
    }

    protected override void NotifySubscribers<TabletInput>(ref TabletInput tabletInfo)
    {
        foreach(TCPMessageSubscriber subscriber in subscribers)
        {
            subscriber.ReceiveMessage(ref tabletInfo);
        }
    }
}
