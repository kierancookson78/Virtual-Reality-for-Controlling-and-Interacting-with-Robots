using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Tablet_Interface : MonoBehaviour, TCPMessageSubscriber
{
    public struct Tablet_Input
    {
        public string input_text;
    }

    [SerializeField] GeneralSettings generalSettings;

    Tablet tablet;
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
        tablet = new Tablet(generalSettings.client);
        tablet.Subscribe(this);
    }

    #region Wifi
    public void enableWifi()
    {
        tablet.EnableWifi();
    }

    public void disableWifi()
    {
        tablet.DisableWifi();
    }

    public void consolePrintWifiStatus()
    {
        tablet.ConsolePrintWifiStatus();
    }

    [SerializeField] InputField configureWifi_networkName;
    [SerializeField] Dropdown configureWifi_security;
    [SerializeField] InputField configureWifi_network_password;
    public void configureWifi()
    {
        string network_name = configureWifi_networkName.text;
        TabletContent.SECURITY security = (TabletContent.SECURITY)configureWifi_security.value;
        string network_password = configureWifi_network_password.text;

        tablet.ConfigureWifi(network_name, security, network_password);
    }

    [SerializeField] InputField connnectWifi_networkName;
    public void connectWifi()
    {
        string network_name = connnectWifi_networkName.text;

        tablet.ConnectWifi(network_name);
    }

    public void disconnectWifi()
    {
        tablet.DisconnectWifi();
    }
    #endregion

    #region Web View
    [SerializeField] InputField showWebView_url;
    public void showWebView()
    {
        string url = showWebView_url.text;
        tablet.ShowWebView(url);
    }

    public void hideWebView()
    {
        tablet.HideWebView();
    }

    [SerializeField] InputField loadApplication_applicationName;
    public void loadApplication()
    {
        string applicationName = loadApplication_applicationName.text;
        tablet.LoadApplication(applicationName);
    }

    public void printInstalledApplications()
    {
        tablet.PrintInstalledApplications();
    }

    [SerializeField] InputField loadUrl_url;
    public void loadUrl()
    {
        string url = loadUrl_url.text;
        tablet.LoadUrl(url);
    }

    public void reloadWebPage()
    {
        bool bypassCache = true;
        tablet.ReloadWebPage(bypassCache);
    }
    #endregion

    #region Video
    [SerializeField] InputField playVideo_url;
    public void playVideo()
    {
        string url = playVideo_url.text;
        tablet.PlayVideo(url);
    }

    public void stopVideo()
    {
        tablet.StopVideo();
    }

    public void pauseVideo()
    {
        tablet.PauseVideo();
    }

    public void resumeVideo()
    {
        tablet.ResumeVideo();
    }
    #endregion

    #region Image
    [SerializeField] InputField showImage_url;
    public void showImage()
    {
        string url = showImage_url.text;
        tablet.ShowImage(url);
    }

    public void hideImage()
    {
        tablet.HideImage();
    }
    #endregion

    #region Input Dialog
    [SerializeField] Dropdown showInputDialog_type;
    [SerializeField] InputField showInputDialog_title;
    [SerializeField] InputField showInputDialog_okText;
    [SerializeField] InputField showInputDialog_cancelText;
    public void showInputDialog()
    {
        TabletContent.TYPE type = (TabletContent.TYPE)showInputDialog_type.value;
        string title = showInputDialog_title.text;
        string ok_text = showInputDialog_okText.text;
        string cancel_text = showInputDialog_cancelText.text;

        tablet.ShowInputDialog(type, title, ok_text, cancel_text);
    }

    public void hideInputDialog()
    {
        tablet.HideInputDialog();
    }
    #endregion

    public void closeTabletBrowser()
    {
        tablet.CloseTabletBrowser();
    }

    public void ReceiveMessage<TabletInfo>(ref TabletInfo tabletInfo)
    {

    }
}
