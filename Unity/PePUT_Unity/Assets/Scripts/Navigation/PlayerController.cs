using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Valve.VR;

public enum NavigationOption
{
    MouseClick,
    FirstPerson,
}

public enum FirstPersonDirection
{
    None,
    Foreward,
    Backward,
    Left,
    Right
}

public class PlayerController : MonoBehaviour, TCPMessageSubscriber
{
    //Set this to false, if not using 2 trackers    
    private Boolean trackerPosInit = false;

    public Camera cam;
    public NavMeshAgent agent;
    public GameObject tracker;
    public GameObject controllerRight;

    [SerializeField] GeneralSettings generalSettings;
    TCPClientTopic tcpClient;


    public Vector3 destinationV3;

    public bool destinationSet = false;
    public bool moving = false;

    #region calibrate Tracker Angle Offset fields
    /// <summary>
    /// Can be set to true in the Unity Inspector to recalibrate
    /// </summary>
    [SerializeField] public bool reCalibrateTrackerAngleOffset = false;
    [SerializeField] private float angleOffset = 90.0f; 
    public bool calibrated = true;
    public bool calibrationStarted = true;
    #endregion calibrate Tracker Angle Offset fields

    private int frameCounter = 0;

    public int failCounter = 0;

    //Maximum number of fails that are allowed during one movement from peppers start position to its destination
    //A fail is defined as peppers movement call returning "false" which means pepper has encountered an obstacle
    private const int MAXFails = 1;

    /// <summary>
    /// Adjust this value to reduce false tracker movement due to tracking inaccuracies,
    /// the value of "averageOver" is the number of previous tracker positions that are taken into consideration
    /// * E.g. if the value is 10, at Frame 10 the tracker positions 0 - 9 will be averaged over and result in a new
    /// * more accurate robot position Increasing this value to values > 25 can possibly decrease performance or cause
    /// * the motion to lag behind
    /// </summary>
    private int averageOver = 25;
    private readonly Vector2[] lastNPositions = new Vector2[25];
    private float[] lastNRotations = new float[10];

    /// <summary>
    ///Distance to target tolerance in METERS below which the position will count as "destination reached"
    ///This script will keep trying to get pepper to the target until its distance is less than the designated value
    /// </summary>
    private const double DistanceToTargetTolerance = 0.2;

    /// <summary>
    /// distance from center of robot to center of tracker
    /// </summary>
    private const float RobotRadius = 0.1f;

    //Distance the robot will travel to calibrate its forward facing direction
    private const float CalibrationDistance = 0.75f;

    //Variable to hold peppers position before it starts to move, to calibrate for its forward vector
    private Vector2 calibrationStartPosition;

    //angle offset of tracker, to be determined during calibration step
    private float trackerAngleOffset;

    //public so you can change the option in unity editor at runtime
    public NavigationOption navigationOption;
    private bool rotateOnly;
    private FirstPersonDirection firstPersonShooterDirection;
    private Vector3 thirdPersonDirection;
    /// <summary>
    /// Distance Pepper will move when navigated by arrows
    /// </summary>
    public float arrowDistance = 3f; //hier könnte man einen kleineren Wert (zB 0.6f) eintragen, wenn man es genauer steuern können möchte
    /// <summary>
    /// Attention: Theta takes a very small number!
    /// </summary>
    private float arrowAngle = 6f; //hier könnte man einen kleineren Wert (zB 1.8f) eintragen, wenn man es genauer steuern können möchte
    /// <summary>
    /// Will trigger the agentPath to be resetted in the next update. 
    /// Workaround because Unity only allows the ResetPath when its called from the main thread.
    /// </summary>
    private bool resetAgentPathInNextUpdate;


    private void Start()
    {
        tcpClient = generalSettings.client;
    }

    bool subscribed = false;
    void Update()
    {
        if (!generalSettings.connected) return;
        if (!subscribed)
        {
            subscribed = true;
            Debug.Log("subscribing...");
            generalSettings.client.SubscribeToMessageReceived(this);
        }

        frameCounter++;

        //resets NavMeshPath and other variables and stops ongoing attempts to send a moveCommand to Pepper.
        //If it't not intentionally trigged by resetAgentPath, this usually means that pepper ran into an obstacle, 
        //got stuck on something on the ground (tape etc.) or the cable is infront of one of her sensors.
        //However sometimes pepper fails for no reason, thats why it sometimes makes sense to try again (2 or 3 times).
        if (failCounter >= MAXFails || resetAgentPathInNextUpdate)
        {
            //dont stop reset Peppers path when option is mouse click because this option uses the navMeshAgent and the path may have more points that should be reached.
            if ((resetAgentPathInNextUpdate == false && navigationOption != NavigationOption.MouseClick) || resetAgentPathInNextUpdate)
            {
                Debug.Log("Reseting Agent Path: " + failCounter + " " + resetAgentPathInNextUpdate);
                //reset all moving-related stuff
                destinationSet = false;
                rotateOnly = false;
                firstPersonShooterDirection = FirstPersonDirection.None;
                agent.ResetPath();
                failCounter = 0;
                resetAgentPathInNextUpdate = false;
                moving = false;
            }
        }

        //this prevents sending new commands to Pepper when stop-button is beeing pushed
        if (IsStopKeyHoldDown())
        {
            Debug.Log("Space key was pressed. Pepper should stop now.");
            StopMovement();
            resetAgentPathInNextUpdate = true;
            return;
        }

        //updating robot representation position in Unity depending on tracker values
        UpdateRobotRepresentationPosition();

        #region Calibration
        if (!TryCalibrate())
        {
            return;
        }
        #endregion Calibration

        #region Move
        if (!TryMove())
        {
            return;
        }
        #endregion Move        
    }

    #region Tracker Angle Offset Calibration
    /// <summary>
    /// If calibration is reqired: Sends MoveCommand to Pepper and calculates <see cref="trackerAngleOffset"/>.
    /// Has to be called multiple times until it returns true.
    /// </summary>
    /// <returns>false if it's to early to calibrate or calibration is not yet finished. Otherwise true</returns>
    public bool TryCalibrate()
    {
        //don't do anything navigation related during the first 100 frames to prevent initialization errors
        if (frameCounter < 100)
        {
            return false;
        }

        //send command to start calibration move if it has not been sent yet
        if (!calibrationStarted || reCalibrateTrackerAngleOffset)
        {
            Debug.Log("Calibrate Start position before calibration: " + calibrationStartPosition);
            Debug.Log("send calibration command");
            StartTrackerAngleOffsetCalibration();
            calibrationStarted = true;
            moving = true;
            reCalibrateTrackerAngleOffset = false;
            calibrated = false;
            Debug.Log("Calibrate Start position: " + calibrationStartPosition);
        }

        //if we are not moving and not calibrated yet => peppers calibration move has ended
        //calibrate rotation and enable nav mesh agent
        if (!calibrated && !moving)
        {
            CalibrateTrackerAngleOffset();
            calibrated = true;
            GetComponent<NavMeshAgent>().enabled = true;
            Debug.Log("NavAgent enabled");
            Debug.Log("Calibrate Start position after calibration: " + calibrationStartPosition);
        }

        return calibrated;
    }

    /// <summary>
    /// Saves start position and lets pepper walk CalibrationDistance meters forward.
    /// </summary>
    private void StartTrackerAngleOffsetCalibration()
    {
        //var calibrateAnnouncement = new TextMessage("speak", "Kalibrierung"); //TODO replace with speak command
        //tcpClientSM.SendMessage(JsonUtility.ToJson(calibrateAnnouncement));
        SpeechContent speech_content = new SpeechContent(SpeechContent.Command.SPEAK, "Kalibrierung");
        TCPContent tcp_speech_content = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_SPEECH, 1, speech_content);

        Debug.Log("Calibrate Start position before assigning new Vector2: " + calibrationStartPosition);
        Debug.Log("transform.position: " + transform.position);
        calibrationStartPosition = new Vector2(transform.position.x, transform.position.z);
        Debug.Log("Calibrate Start position after assigning new Vector2: " + calibrationStartPosition);

        MoveContent content = new MoveContent(MoveContent.MOVE_COMMAND.move_moveTo, CalibrationDistance, 0, 0);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, content);
        if (generalSettings.connected)
        {
            generalSettings.client.SendMessage(tcpContent.toJSONMessage());
        }
    }

    /// <summary>
    /// This function is called, after pepper has moved the calibration distance in a straight line
    /// </summary>
    private void CalibrateTrackerAngleOffset()
    {
        // Get the position of peppers representation right now
        var trackerPosition2d = new Vector2(transform.position.x, transform.position.z);

        //Get the vector from the position where pepper started the calibration, compared to now (this is the real forward facing vector of pepper)
        Vector2 realRobotForward = calibrationStartPosition - trackerPosition2d;

        Debug.Log("real robot fwd: " + realRobotForward + " " + calibrationStartPosition + " " + trackerPosition2d);

        //Get the vector that peppers 3d representation is currently facing (its facing the same direction as the real tracker does)
        Vector2 representationForward = new Vector2(transform.forward.x, transform.forward.z);

        //Calculate the difference between these 2 vectors, to find the error by which the tracker angle needs to be adjusted
        //If the tracker was mounted perfectly facing forward on pepper, this angle would be 0
        var angle = Vector2.SignedAngle(representationForward, realRobotForward);

        //Print debug info
        Debug.Log("Robot fwd: " + realRobotForward.x + ", " + realRobotForward.y + " reprForward: " + representationForward.x + ", " + representationForward.y);
        Debug.Log("Angle offset = " + angle);

        //Adjust for the angle being a Signed Angle, convert it to the amount that needs to be added / subtracted from the detected angle
        float result;
        if (angle < 0)
        {
            result = 180 - (-1 * angle);
        }
        else
        {
            result = -1 * (180 - angle);
        }

        trackerAngleOffset = result;
        Debug.Log("Adjust angle by: " + result);
        // var radAngle = angle * Mathf.Deg2Rad;
    }
    #endregion Tracker Angle Offset Calibration

    #region Move
    /// <summary>
    /// Sends StopMove-Message to Pepper and resets AgentPath.
    /// public because it can be called from a Unity UI button
    /// </summary>
    public void StopMovement()
    {

        MoveContent content = new MoveContent(MoveContent.MOVE_COMMAND.move_stopMove);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, content);
        if (generalSettings.connected)
        {
            generalSettings.client.SendMessage(tcpContent.toJSONMessage());
        }

        resetAgentPathInNextUpdate = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool TryMove()
    {
        #region make movement command
        //check if any target is set, if not, allow setting one.
        if (!destinationSet)
        {
            SetTarget();
        }
        #endregion make movement command

        //we have a target => if we are moving, do nothing
        //Stop Movement if key is not longer pressed down 
        switch (navigationOption)
        {
            case NavigationOption.MouseClick:
                // For this option there is no function for overwriting the target. The user has to press stop button and set a new target manually.
                break;
            case NavigationOption.FirstPerson:
                if (firstPersonShooterDirection != GetFirstPersonDirection() || IsDirectionKeyUp())
                {
                    StopMovement();
                    Debug.Log("First Person. Pepper should stop now.");
                    return false;
                }
                break;
            default:
                Debug.Log($"Navigation option {navigationOption} is not implemented.");
                break;
        }

        //return if no target is set
        if (!destinationSet) return false;

        //we have a target and are not moving => check if we are close enough to the final destination
        //if yes, end the current movement, and allow the setting of a new target
        if (!rotateOnly && navigationOption == NavigationOption.MouseClick && DestinationReached())
        {
            destinationSet = false;
            moving = false;
            return false;
        }

        //we have a target, are currently not moving and are not close enough to the destination
        //=> get next subTarget in the path to our target and send next moveCommand to robot
        if(!moving) SendNextMoveCommand();
        return true;
    }

    private void SetTarget()
    {
        //Debug.Log(navigationOption);
        switch (navigationOption)
        {
            case NavigationOption.MouseClick:
                destinationSet = SetTargetByMouseClick();
                break;
            case NavigationOption.FirstPerson:
                destinationSet = SetTargetFirstPersonShooter();
                break;
            default:
                Debug.Log("Case not implemented: " + navigationOption);
                destinationSet = false;
                break;
        }
        if (!destinationSet) return;

        //calculate and set agent path to destination
        var agentPath = new NavMeshPath();
        bool targetReachable = NavMesh.CalculatePath(agent.transform.position, destinationV3, NavMesh.AllAreas, agentPath);
        agent.SetPath(agentPath);

        Debug.Log("setting a new target succeeded.");
        Debug.Log($"Reachable: {targetReachable}. Target: {destinationV3}. Agent destination: {agent.destination}. Agent path: {agentPath}. Robot position: {agent.transform.position}");
    }
    #endregion Move

    #region Switch Navigation Mode (value is only changed from Unity)
    public void SwitchNavigationModeKlick()
    {
        SwitchNavigationMode(NavigationOption.MouseClick);
    }
    public void SwitchNavigationModeFirstPerson()
    {
        SwitchNavigationMode(NavigationOption.FirstPerson);
    }

    private void SwitchNavigationMode(NavigationOption option)
    {
        Debug.Log($"Navigation option will be switched from {navigationOption} to {option}");
        navigationOption = option;
    }
    #endregion Switch Navigation Mode (value is only changed from Unity)

    /// <summary>
    /// This function moves the 3d representation of pepper in the unity scene, to the position of the real pepper, 
    /// depending on the position of the vive tracker, and the previously determined angle offset. 
    /// It also applies motion smoothing to take care of jittering (frames with wrong tracker positions)
    /// 
    /// Possible improvemnet: Instead of using the average position, sort the last 10 recorded position by the sum of their absolute values
    /// (sum = abs(x) + abs(y) + abs(z)) and then use the median of those values as the ground truth. Outliers should be eliminated that way.
    /// </summary>
    private void UpdateRobotRepresentationPosition()
    {
        //Save current tracker position, to enable averaging / reducing of jittering
        lastNPositions[frameCounter % averageOver] = new Vector2(tracker.transform.position.x, tracker.transform.position.z);

        //calculate average over last n tracker positions and update position, if enough frames have already passed i.e. enough data points are saved
        if (frameCounter > averageOver)
        {
            //MOVE TO AVERAGE OF LAST N TRACKER POSITIONS
            float xSum = 0;
            float zSum = 0;
            for (var i = 0; i < averageOver; i++)
            {
                xSum += lastNPositions[i].x;
                zSum += lastNPositions[i].y;
            }
            transform.position = new Vector3(xSum / averageOver, transform.position.y, zSum / averageOver) + transform.forward * RobotRadius;
        }

        //Rotation smoothing
        lastNRotations[frameCounter % 10] = tracker.transform.eulerAngles.y;
        if (frameCounter > 10)
        {
            float rotationSum = 0;
            for (var i = 0; i < 10; i++)
            {
                rotationSum += lastNRotations[i];
            }
            float rotationAvg = rotationSum / 10;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotationAvg - trackerAngleOffset, transform.eulerAngles.z);
        }
    }

    #region SetTarget
    /// <summary>
    ///
    /// </summary>
    /// <returns>true: a new value has been assigned to <see cref="destinationV3"/>.</returns>
    private bool SetTargetByMouseClick()
    {
        Vector3 clickDestination = GetClickDestination();

        if (clickDestination != Vector3.zero)
        {
            rotateOnly = Input.GetMouseButtonDown(1);
            destinationV3 = clickDestination;
            Debug.Log("Destination: " + destinationV3);
            return true;
        }
        else
        {
            return false;
        }
    }
    private Vector3 GetClickDestination()
    {
        //set destination to position selected with mouse
        //return if mose is not clicked
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //Cast a ray from the camera to the surface hovered by the mouse
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            //return if anything out of bounds is clicked
            if (!Physics.Raycast(ray, out RaycastHit hit)) return Vector3.zero;

            //set target
            Debug.Log("Hit Point: " + hit.point);
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private bool SetTargetFirstPersonShooter()
    {
        firstPersonShooterDirection = GetFirstPersonDirection();

        switch (firstPersonShooterDirection)
        {
            case FirstPersonDirection.None:
                return false;
                break;
            case FirstPersonDirection.Foreward:
                destinationV3 = gameObject.transform.position + Vector3.forward * arrowDistance;
                break;
            case FirstPersonDirection.Backward:
                destinationV3 = gameObject.transform.position + Vector3.back * arrowDistance;
                break;
            case FirstPersonDirection.Left:
                rotateOnly = true;
                break;
            case FirstPersonDirection.Right:
                rotateOnly = true;
                break;
            default:
                return false;
                break;
        }

        return true;
    }
    private FirstPersonDirection GetFirstPersonDirection()
    {
        if (GetIsUpInput())
        {
            Debug.Log("Input Up Was Pressed");
            return FirstPersonDirection.Foreward;
        }
        else if (GetIsLeftInput())
        {
            return FirstPersonDirection.Left;
        }
        else if (GetIsDownInput())
        {
            return FirstPersonDirection.Backward;
        }
        else if (GetIsRightInput())
        {
            return FirstPersonDirection.Right;
        }
        else
        {
            return FirstPersonDirection.None;
        }
    }
    #endregion SetTarget

    #region Get Input
    private bool IsStopKeyHoldDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    private bool IsDirectionKeyUp()
    {
        if (Input.GetKeyUp(KeyCode.W)
            || Input.GetKeyUp(KeyCode.A)
            || Input.GetKeyUp(KeyCode.S)
            || Input.GetKeyUp(KeyCode.D)
            || Input.GetKeyUp(KeyCode.UpArrow)
            || Input.GetKeyUp(KeyCode.DownArrow)
            || Input.GetKeyUp(KeyCode.LeftArrow)
            || Input.GetKeyUp(KeyCode.RightArrow))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool GetIsUpInput()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
    }
    private bool GetIsDownInput()
    {
        return (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));
    }
    private bool GetIsLeftInput()
    {
        return (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));
    }
    private bool GetIsRightInput()
    {
        return (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow));
    }

    #endregion Get Input

    /// <summary>
    /// calculates new agentpath to destination from current position
    /// </summary>
    /// <returns>Vector3 of next subTarget</returns>
    /// <exception cref="Exception"></exception>
    private Vector3 GetNextSubTarget()
    {
        //calculate and set agent path to destination
        var agentPath = new NavMeshPath();
        //calculate path depending on current position and destination
        NavMesh.CalculatePath(agent.transform.position, destinationV3, NavMesh.AllAreas, agentPath);
        agent.SetPath(agentPath);

        if (agent.path.corners.Length == 0)
        {
            //returning original agent position resulting in no movement.
            Debug.LogError("No valid path was found, make sure navigation agent is properly placed inside of a navigation mesh");
            return agent.transform.position;
        }

        //return the first corner in the path, that is not equal to the origin of the path
        var origin = agent.path.corners[0];

        foreach (var pathCorner in agent.path.corners)
        {
            if (pathCorner != origin)
            {
                return pathCorner;
            }
        }
        //returning original agent position resulting in no movement.
        Debug.LogError("Navigation error: Path only consists of origin points");
        return agent.transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if pepper is close enough to its destination, false if the distance still is greater than the distanceToTargetTolerance</returns>
    public bool DestinationReached()
    {
        //Create 2d vectors, disregarding height to calculate distance from agent to destination
        Vector2 agent2d = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 destination2d = new Vector2(agent.destination.x, agent.destination.z);
        var distanceToTarget = Vector2.Distance(agent2d, destination2d);

        if (distanceToTarget < DistanceToTargetTolerance)
        {
            //agent is within tolerable distance to target, stop moving, reset target
            Debug.Log("Destination REACHED (distance to destination: " + distanceToTarget + "m)");
            destinationSet = false;
            moving = false;
            agent.ResetPath();
            return true;
        }
        Debug.Log("Destination NOT reached, distance to destination:" + distanceToTarget);
        return false;
    }

    private void SendNextMoveCommand()
    {
        Debug.Log("Send next move Command...");
        float distance = 0;
        float radAngle = 0;
        if (navigationOption == NavigationOption.FirstPerson)
        {
            switch (firstPersonShooterDirection)
            {
                case FirstPersonDirection.None:
                    Debug.Log("Case 'FirstPersonShooterDirections.None' not handled.");
                    break;
                case FirstPersonDirection.Foreward:
                    distance = arrowDistance;
                    break;
                case FirstPersonDirection.Backward:
                    distance = -arrowDistance / 2;
                    break;
                case FirstPersonDirection.Left:
                    radAngle = arrowAngle;
                    break;
                case FirstPersonDirection.Right:
                    radAngle = -arrowAngle;
                    break;
                default:
                    throw new NotImplementedException($"{firstPersonShooterDirection} is not implemented.");
            }
        }
        else if (rotateOnly) // right klicked only
        {
            //no distance because pepper should only rotate
            distance = 0;

            //get target and agent as Vector2 (X and Z coordinates are now X and Y coordinates)
            Vector2 nextTargetV2 = new Vector2(destinationV3.x, destinationV3.z);
            Vector2 agent2d = new Vector2(agent.transform.position.x, agent.transform.position.z);

            //get angle between vector pointing to target and vector pointing forward from robot
            Vector2 toTarget = nextTargetV2 - agent2d;
            Vector2 forward2d = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.SignedAngle(toTarget, forward2d);
            //convert angle to radians, since peppers move command takes radians as input
            radAngle = (angle + angleOffset) * Mathf.Deg2Rad * -1;

            rotateOnly = false;
            destinationSet = false;
        }
        else
        {
            Vector3 nextTargetV3 = GetNextSubTarget();

            //get target and agent as Vector2 (X and Z coordinates are now X and Y coordinates)
            var nextTargetV2 = new Vector2(nextTargetV3.x, nextTargetV3.z);
            Vector2 agent2d = new Vector2(agent.transform.position.x, agent.transform.position.z);

            //get angle between vector pointing to target and vector pointing forward from robot
            Vector2 toTarget = nextTargetV2 - agent2d;
            Vector2 forward2d = new Vector2(transform.forward.x, transform.forward.z);
            var angle = Vector2.SignedAngle(toTarget, forward2d);
            //convert angle to radians, since peppers move command takes radians as input
            radAngle = (angle + angleOffset) * Mathf.Deg2Rad * -1;

            //get distance to target
            distance = Vector2.Distance(agent2d, nextTargetV2);
        }
        //send command to pepper
        MoveContent content = new MoveContent(MoveContent.MOVE_COMMAND.move_moveTo, distance, 0, radAngle);
        TCPContent tcpContent = new TCPContent(TCPContent.TOPIC.ROBOT, TCPContent.SUBTOPIC.ROBOT_MOVE, 1, content);
        Debug.Log("MOVEcommand: " + content);
        if (generalSettings.connected)
        {
            generalSettings.client.SendMessage(tcpContent.toJSONMessage());
        }

        //Mark that we are currently moving, this blocks the setting of new targets
        moving = true;
    }

    public void ReceiveMessage(string message)
    {
        string[] messageSplit = message.Split('?');
        message = messageSplit[0];

        Debug.Log("Split Message: " + message);

        try
        {
            var msgFromServer = JsonUtility.FromJson<TextMessage>(message);


            if (msgFromServer.type == "motionResult")
            {
                moving = false;
                Debug.Log("MOTION result detected " + msgFromServer.content);
                if (msgFromServer.content == "False")
                {
                    //increase fail counter if pepper failed to execute a movement
                    failCounter++;
                }
                else if (msgFromServer.content == "True")
                {
                    Debug.Log("MotionResult = true");
                    if (navigationOption != NavigationOption.MouseClick)
                    {
                        resetAgentPathInNextUpdate = true;
                    }
                    else
                    {
                        Debug.Log("MouseClick: " + resetAgentPathInNextUpdate + " has to be false");
                    }
                }
            }
            else if (msgFromServer.type == "stopResult")
            {
                resetAgentPathInNextUpdate = true;
                Debug.Log($"STOP result detected {msgFromServer.content}");
            }
        } catch
        {
            return;
        }
    }

    public void ReceiveMessage<T>(ref T message)
    {

    }

    [Serializable]
    private class TextMessage
    {
        public string type;
        public string content;

        public TextMessage(string type, string content)
        {
            this.type = type;
            this.content = content;
        }
    }
}
