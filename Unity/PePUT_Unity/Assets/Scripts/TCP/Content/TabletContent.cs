using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TabletContent : Content
{
    public enum TABLET_COMMAND
    {
        wifi_enableWifi,
        wifi_disableWifi,
        wifi_getWifiStatus,
        wifi_configureWifi,
        wifi_connectWifi,
        wifi_disconnectWifi,

        web_showWebView,
        web_hideWebView,
        web_loadApplication,
        web_loadUrl,
        web_reloadPage,

        video_playVideo,
        video_stopVideo,
        video_pauseVideo,
        video_resumeVideo,

        image_showImage,
        image_hideImage,

        dialog_showInputDialog,
        dialog_hideDialog,

        lowLevel_closeTabletBrowser,

        wake_up,
        sleep
    }

    public enum SECURITY
    {
        wep,
        wpa,
        open,
    }

    public enum TYPE
    {
        text, 
        password,
        email,
        url,
        number
    }

    public string tablet_command;
    public string security;
    public string network_name;
    public string network_password;
    public string url;
    public string application_name;
    public bool bypass_cache;
    public string type;
    public string title;
    public string ok_text;
    public string cancel_text;

    public TabletContent(TABLET_COMMAND tablet_command)
    {
        this.tablet_command = tablet_command.ToString();
    }

    public TabletContent(TABLET_COMMAND tablet_command, string network_name, SECURITY security = SECURITY.open, string network_password = "") //for configureWifi, connectWifi
    {
        this.tablet_command = tablet_command.ToString();
        this.network_name = network_name;
        this.security = security.ToString();
        this.network_password = network_password;
    }

    public TabletContent(TABLET_COMMAND tablet_command, string url_applicationName)
    {
        this.tablet_command = tablet_command.ToString();
        if (tablet_command == TABLET_COMMAND.web_loadApplication)
        {
            application_name = url_applicationName;
        }
        else
        {
            url = url_applicationName;
        }
    }

    public TabletContent(TABLET_COMMAND tablet_command, bool bypass_cache) //for web_reloadPage
    {
        this.tablet_command = tablet_command.ToString();
        this.bypass_cache = bypass_cache;
    }

    public TabletContent(TABLET_COMMAND tablet_command, TYPE type, string title, string ok_text, string cancel_text)
    {
        this.tablet_command = tablet_command.ToString();
        this.type = type.ToString();
        this.title = title;
        this.ok_text = ok_text;
        this.cancel_text = cancel_text;
    }

}
