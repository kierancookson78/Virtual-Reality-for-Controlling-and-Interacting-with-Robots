import qi
import json

class Tablet:
    tablet_service = None
    connection = None

    def __init__(self, tablet_service, connection):
        self.tablet_service = tablet_service
        self.connection = connection


    def interpretContent(self, message):
        content = message["content"]

        if content["tablet_command"] == "wifi_enableWifi":
            self.enableWifi()
        if content["tablet_command"] == "wifi_disableWifi":
            self.disableWifi()
        if content["tablet_command"] == "wifi_getWifiStatus":
            self.getWifiStatus()
        if content["tablet_command"] == "wifi_configureWifi":
            self.configureWifi(content["security"], content["network_name"], content["network_password"])
        if content["tablet_command"] == "wifi_connectWifi":
            self.connectWifi(content["network_name"])
        if content["tablet_command"] == "wifi_disconnectWifi":
            self.disconnectWifi()

        if content["tablet_command"] == "web_showWebView":
            self.showWebView(content["url"])
        if content["tablet_command"] == "web_hideWebView":
            self.hideWebView()
        if content["tablet_command"] == "web_loadApplication":
            self.loadApplication(content["application_name"])
        if content["tablet_command"] == "web_loadUrl":
            self.loadUrl(content["url"])
        if content["tablet_command"] == "web_reloadPage":
            self.reloadPage(content["bypass_cache"])

        if content["tablet_command"] == "video_playVideo":
            self.playVideo(content["url"])
        if content["tablet_command"] == "video_stopVideo":
            self.stopVideo()
        if content["tablet_command"] == "video_pauseVideo":
            self.pauseVideo()
        if content["tablet_command"] == "video_resumeVideo":
            self.resumeVideo()

        if content["tablet_command"] == "image_showImage":
            self.showImage(content["url"])
        if content["tablet_command"] == "image_hideImage":
            self.hideImage()

        if content["tablet_command"] == "dialog_showInputDialog":
            self.showInputDialog(content["type"], content["title"], content["ok_text"], content["cancel_text"])
        if content["tablet_command"] == "dialog_hideDialog":
            self.hideDialog()

        if content["tablet_command"] == "wake_up":
            self.tablet_service.wakeUp()
        if content["tablet_command"] == "sleep":
            self.tablet_service.goToSleep()

        if content["tablet_command"] == "lowLevel_closeTabletBrowser":
            self.closeTabletBrowser()
            return True
        return False

    def closeTabletBrowser(self):
        try:
            self.tablet_service._stopApk("jp.softbank.tabletbrowser")
        except RuntimeError:
            # A runtime error is expected, as the connection to the TabletService is lost, the moment the apk is stopped
            tablet_service = None
            pass

    def tabletNotificationConfirmation(self, ip, port):
        try:
            connection_url = "tcp://" + ip + ":" + str(port)
            app = qi.Application(["TabletModule", "--qi-url=" + connection_url])
            app.start()

            session = app.session
            behavior_mng_service = session.service("ALBehaviorManager")
            behavior_mng_service.runBehavior("notificationproj2-5d7801/behavior_1")
            self.tablet_service = session.service("ALTabletService")

            # Don't forget to disconnect the signal at the end
            signalID = 0

            # function called when the signal onTouchDown is triggered
            def callback(x, y):
                print "Notification has been confirmed"
                print "Touch coordinate are x: ", x, " y: ", y
                self.tablet_service.hideWebview()
                self.tablet_service.onTouchDown.disconnect(signalID)
                app.stop()

            # attach the callback function to onJSEvent signal
            signalID = self.tablet_service.onTouchDown.connect(callback)
            app.run()
        except Exception, e:
            print "Error was: ", e

    #region Wifi
    def enableWifi(self):
        self.tablet_service.enableWifi()

    def disableWifi(self):
        self.tablet_service.disableWifi()

    def getWifiStatus(self):
        print self.tablet_service.getWifiStatus()

    def configureWifi(self, security, network_name, network_password):
        self.tablet_service.configureWifi(security, network_name, network_password)

    def connectWifi(self, network_name):
        self.tablet_service.connectWifi(network_name)

    def disconnectWifi(self):
        self.tablet_service.disconnectWifi()
    #endregion

    #region Web View
    def showWebView(self, url):
        if url == "":
            self.tablet_service.showWebview()
        else:
            self.tablet_service.showWebview(url)

    def hideWebView(self):
        self.tablet_service.hideWebview()

    def loadApplication(self, application_name):
        self.tablet_service.loadApplication(application_name)

    def loadUrl(self, url):
        self.tablet_service.loadUrl(url)

    def reloadPage(self, bypassCache): # set to true in order to reload the current page by bypassing the local web cache
        self.tablet_service.reloadPage(bypassCache)
    #endregion

    #region Video Player
    def playVideo(self, url):
        self.tablet_service.playVideo(url)

    def stopVideo(self):
        self.tablet_service.stopVideo()

    def pauseVideo(self):
        self.tablet_service.pauseVideo()

    def resumeVideo(self):
        self.tablet_service.resumeVideo()
    #endregion

    #region Image
    def showImage(self, url):
        self.tablet_service.showImage(url)

    def hideImage(self):
        self.tablet_service.hideImage()
    #endregion

    #region Dialog
    def showInputDialog(self, type, title, ok_text, cancel_text):
        touchID = 0
        def touchdown(x,y):
            print str(x) + " " + str(y)
            if(x > 2000 and y > 500):
                self.tablet_service.onTouchDown.disconnect(touchID)

        #touchID = tablet_service.onTouchDown.connect(touchdown)
        touchID = self.tablet_service.onTouchMove.connect(touchdown)
        #drawing sites
        #https://www.tinyimage.com

        self.tablet_service.showInputDialog(type, title, ok_text, cancel_text)
        self.tablet_service.onInputText

        signalID = 0
        def callback(validation, input_text):
            x = ""
            if(validation == 1):
                print input_text
                x = {"input_text": input_text}
            if(validation == 0):
                print "Input canceled"
                x = {"input_text": "error::canceled"}
            y = json.dumps(x)
            self.connection.sendall(y.encode())

        signalID = self.tablet_service.onInputText.connect(callback)

    def hideDialog(self):
        self.tablet_service.hideDialog()
    #endregion
