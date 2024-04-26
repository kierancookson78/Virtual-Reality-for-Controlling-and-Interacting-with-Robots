using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionContent : Content
{
    public string IP;
    public int port;
    public string autonomy_mode;

    public ConnectionContent(string IP, int port, RobotAutonomyContent.AUTONOMY_MODE autonomy_mode = RobotAutonomyContent.AUTONOMY_MODE.SAFEGUARD)
    {
        this.IP = IP;
        this.port = port;
        this.autonomy_mode = RobotAutonomyContent.autonomyModeLookup[autonomy_mode];
    }
}
