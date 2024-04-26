import almath
import time

class MotorControl():
    @staticmethod
    def interpret(motion_service, message, connection, posture_service):
        content = message["content"]
        command = content["command"]

        if command == "SET":
            MotorControl.setMotors(motion_service, message)
        elif command == "RECEIVE":
            MotorControl.getMotorValues(motion_service, connection)
        elif command == "SLEEP":
            MotorControl.sleep(motion_service)
        elif command == "WAKEUP":
            MotorControl.wakeUp(motion_service)
        elif command == "STAND_INIT":
            MotorControl.standInit(posture_service)

        #Positions implemented
        elif command == "CROUCH_INIT":
            MotorControl.crouchInit(posture_service)
        elif command == "SIT_RELAX_INIT":
            MotorControl.sitRelaxInit(posture_service)
        elif command == "LYING_BACK_INIT":
            MotorControl.lyingBackInit(posture_service)

        #Right Arm implemented
        elif command == "SHOULDER_PITCH_RIGHT_INIT":
            MotorControl.setShoulderPitchRightInit(motion_service, message)
        elif command == "SHOULDER_ROLL_RIGHT_INIT":
            MotorControl.setShoulderRollRightInit(motion_service, message)

        #Left Arm implemented
        elif command == "SHOULDER_PITCH_LEFT_INIT":
            MotorControl.setShoulderPitchLeftInit(motion_service, message)

        #Head Movement implemented
        elif command == "HEAD_PITCH_INIT":
            MotorControl.setHeadPitchInit(motion_service, message)
        elif command == "HEAD_YAW_INIT":
            MotorControl.setHeadYawInit(motion_service, message)

    #setMotors using the list of motors for the NAO robot
    @staticmethod
    def setMotors(motion_service, message):
        content = message["content"]

        motors = ["heady", "headp",
                  "spl", "srl", "eyl", "erl", "wyl", "hl",
                  "spr", "srr", "eyr", "err", "wyr", "hr",
                  "hipypl", "hipypr",
                  "hiprl", "hippl", "kpl", "apl", "arl",
                  "hiprr", "hippr", "kpr", "apr", "arr"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], float(axis["s"]))

            # Simple content for the HeadYaw joint at 10% max speed
            names.append(axis["ma"])
            angles.append(float(axis["mv"]) * almath.TO_RAD)
            fractionMaxSpeed = float(axis["sp"])

        # Hand Control
        angles[15] = float(content["hl"]["mv"])
        angles[16] = float(content["hr"]["mv"])

        motion_service.setAngles(names, angles, fractionMaxSpeed)

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)

    @staticmethod
    def getMotorValues(motion_service, connection):

        motors = ["HeadYaw", "HeadPitch",
                  "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHand",
                  "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWritstYaw", "RHand",
                  "LHipYawPitch", "RHipYawPitch",
                  "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "LAnkleRoll",
                  "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "RAnkleRoll"]


        useSensors = False
        commandAngles = []

        for motor in motors:
            commandAngles.append(float(motion_service.getAngles(motor, useSensors)[0]) * almath.TO_DEG)
        message = "{\"type\" : \"getmv\", \"NAOHeadYaw\":" + str(
            commandAngles[0]) + ", \"NAOHeadPitch\":" + str(
            commandAngles[1]) + ", \"NAOShoulderPitchL\":" + str(
            commandAngles[2]) + ", \"NAOShoulderRollL\":" + str(
            commandAngles[3]) + ", \"NAOElbowYawL\":" + str(
            commandAngles[4]) + ", \"NAOElbowRollL\":" + str(
            commandAngles[5]) + ", \"NAOWristYawL\":" + str(
            commandAngles[6]) + ", \"NAOHandL\":" + str(
            commandAngles[7]) + ", \"NAOShoulderPitchR\":" + str(
            commandAngles[8]) + ", \"NAOShoulderRollR\":" + str(
            commandAngles[9]) + ", \"NAOElbowYawR\":" + str(
            commandAngles[10]) + ", \"NAOElbowRollR\":" + str(
            commandAngles[11]) + ", \"NAOWristYawR\":" + str(
            commandAngles[12]) + ", \"NAOHandR\":" + str(
            commandAngles[13]) + ", \"NAOHipYawPitchL\":" + str(
            commandAngles[14]) + ", \"NAOHipYawPitchR\":" + str(
            commandAngles[15]) + ", \"NAOHipRollL\":" + str(
            commandAngles[16]) + ", \"NAOHipPitchL\":" + str(
            commandAngles[17]) + ", \"NAOKneePitchL\":" + str(
            commandAngles[18]) + ", \"NAOAnklePitchL\":" + str(
            commandAngles[19]) + ", \"NAOAnkleRollL\":" + str(
            commandAngles[20]) + ", \"NAOHipRollR\":" + str(
            commandAngles[21]) + ", \"NAOHipPitchR\":" + str(
            commandAngles[22]) + ", \"NAOKneePitchR\":" + str(
            commandAngles[23]) + ", \"NAOAnklePitchR\":" + str(
            commandAngles[24]) + ", \"NAOAnkleRollR\":" + str(
            commandAngles[25]) + "}"

        connection.sendall(message.encode())

    @staticmethod
    def sleep(motion_service):
        motion_service.rest()

    @staticmethod
    def wakeUp(motion_service):
        motion_service.wakeUp()

    #LED control / ALLedsProxy
    @staticmethod
    def LED_earLedsSetAngle(LED_service, content):
        LED_service.earLedsSetAngle(float(content["degrees"]), float(content["duration"]), bool(content["leaveOnAtEnd"]))

    @staticmethod
    def fade(LED_service, content):
        LED_service.fade(content["group_name"], float(content["intensity"]), float(content["duration"]))

    @staticmethod
    def fadeListRGB(LED_service, content):
        LED_service.fade(content["group_name"], content["rgbList"], content["timeList"])

    #TODO check if overload of fadeRGB is required
    @staticmethod
    def fadeRGB(LED_service, content):
        LED_service.fadeRGB(content["group_name"], content["color"], float(content["duration"]))

    @staticmethod
    def getIntensity(LED_service, connection, content):
        intensity = LED_service.getIntensity(content["group_name"])
        connection.sendAll(str(intensity).encode())

    @staticmethod
    def listGroup(LED_service, connection, content):
        groupList = LED_service.listGroup(content["group_name"])
        connection.sendAll(str(groupList).encode())

    @staticmethod
    def off(LED_service, content):
        LED_service.off(content["group_name"])

    @staticmethod
    def on(LED_service, content):
        LED_service.on(content["group_name"])

    @staticmethod
    def randomEyes(LED_service, content):
        LED_service.randomEyes(float(content["duration"]))

    @staticmethod
    def rasta(LED_service, content):
        LED_service.rasta(float(content["duration"]))

    @staticmethod
    def reset(LED_service, content):
        LED_service.reset(content["group_name"])

    @staticmethod
    def rotateEyes(LED_service, content):
        LED_service.rotateEyes(content["rgb_hex"], float(content["rotation_time"]), float(content["duration"]))

    @staticmethod
    def setIntensity(LED_service, content):
        LED_service.setIntensity(content["group_name"], float(content["intensity"]))

    @staticmethod
    def standInit(posture_service):
        posture_service.goToPosture("StandInit", 0.5)

    #New Functions implemented for NAO
    #Function that takes the posture_service as a parameter and sets NAO to the crouch position
    @staticmethod
    def crouchInit(posture_service):
        posture_service.goToPosture("Crouch", 0.5)

    #Function that takes the posture_service as a parameter and sets NAO to the sitting position
    @staticmethod
    def sitRelaxInit(posture_service):
        posture_service.goToPosture("SitRelax", 0.5)

    #Function that takes the posture_service as a parameter and sets NAO to the lying position
    @staticmethod
    def lyingBackInit(posture_service):
        posture_service.goToPosture("LyingBack", 0.5)

    #Function that takes the motion_service and message as parameters sets the NAOs right shoulder pitch to the
    #specified angle
    @staticmethod
    def setShoulderPitchRightInit(motion_service, message):
        content = message["content"]

        motors = ["spr"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            stiffness_value = axis["s"]

            if stiffness_value:
                motion_service.setStiffnesses(axis["sa"], float(stiffness_value))
            else:
                print("Stiffness value is empty for axis:", axisNames)

            names.append(axis["ma"])
            if axis["mv"]:
                try:
                    motor_value = float(axis["mv"])
                    angles.append(motor_value * almath.TO_RAD)
                except ValueError as e:
                    print("Error converting motor value to float:", e)
            else:
                print("Empty motor value encountered for axis:", axisNames)

            if axis["sp"]:
                try:
                    fractionMaxSpeed = float(axis["sp"])
                except ValueError as e:
                    print("Error converting speed value to float:", e)
            else:
                print("Empty speed value encountered for axis:", axisNames)

        names = [name for name in names if name]

        if names:
            motion_service.setAngles("RShoulderPitch", angles, fractionMaxSpeed)
        else:
            print("No valid joint names provided. Unable to set angles.")

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)

    #Function that takes the motion_service and message as parameters sets the NAOs left shoulder pitch to the
    #specified angle
    @staticmethod
    def setShoulderPitchLeftInit(motion_service, message):
        content = message["content"]

        motors = ["spl"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            stiffness_value = axis["s"]

            if stiffness_value:
                motion_service.setStiffnesses(axis["sa"], float(stiffness_value))
            else:
                print("Stiffness value is empty for axis:", axisNames)

            names.append(axis["ma"])
            if axis["mv"]:
                try:
                    motor_value = float(axis["mv"])
                    angles.append(motor_value * almath.TO_RAD)
                except ValueError as e:
                    print("Error converting motor value to float:", e)
            else:
                print("Empty motor value encountered for axis:", axisNames)

            if axis["sp"]:
                try:
                    fractionMaxSpeed = float(axis["sp"])
                except ValueError as e:
                    print("Error converting speed value to float:", e)
            else:
                print("Empty speed value encountered for axis:", axisNames)

        names = [name for name in names if name]

        if names:
            motion_service.setAngles("LShoulderPitch", angles, fractionMaxSpeed)
        else:
            print("No valid joint names provided. Unable to set angles.")

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)

    #Function that takes the motion_service and message as parameters sets the NAOs right shoulder roll to the
    #specified angle
    @staticmethod
    def setShoulderRollRightInit(motion_service, message):
        content = message["content"]

        motors = ["srr"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            stiffness_value = axis["s"]

            if stiffness_value:
                motion_service.setStiffnesses(axis["sa"], float(stiffness_value))
            else:
                print("Stiffness value is empty for axis:", axisNames)

            names.append(axis["ma"])
            if axis["mv"]:
                try:
                    motor_value = float(axis["mv"])
                    angles.append(motor_value * almath.TO_RAD)
                except ValueError as e:
                    print("Error converting motor value to float:", e)
            else:
                print("Empty motor value encountered for axis:", axisNames)

            if axis["sp"]:
                try:
                    fractionMaxSpeed = float(axis["sp"])
                except ValueError as e:
                    print("Error converting speed value to float:", e)
            else:
                print("Empty speed value encountered for axis:", axisNames)

        names = [name for name in names if name]

        if names:
            motion_service.setAngles("RShoulderRoll", angles, fractionMaxSpeed)
        else:
            print("No valid joint names provided. Unable to set angles.")

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)

    #Function that takes the motion_service and message as parameters sets the NAOs head pitch to the
    #specified angle
    @staticmethod
    def setHeadPitchInit(motion_service, message):
        content = message["content"]

        motors = ["headp"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            stiffness_value = axis["s"]

            if stiffness_value:
                motion_service.setStiffnesses(axis["sa"], float(stiffness_value))
            else:
                print("Stiffness value is empty for axis:", axisNames)

            names.append(axis["ma"])
            if axis["mv"]:
                try:
                    motor_value = float(axis["mv"])
                    angles.append(motor_value * almath.TO_RAD)
                except ValueError as e:
                    print("Error converting motor value to float:", e)
            else:
                print("Empty motor value encountered for axis:", axisNames)

            if axis["sp"]:
                try:
                    fractionMaxSpeed = float(axis["sp"])
                except ValueError as e:
                    print("Error converting speed value to float:", e)
            else:
                print("Empty speed value encountered for axis:", axisNames)

        names = [name for name in names if name]

        if names:
            motion_service.setAngles("HeadPitch", angles, fractionMaxSpeed)
        else:
            print("No valid joint names provided. Unable to set angles.")

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)

    #Function that takes the motion_service and message as parameters sets the NAOs head yaw to the
    #specified angle
    @staticmethod
    def setHeadYawInit(motion_service, message):
        content = message["content"]

        motors = ["heady"]

        names = []
        angles = []
        fractionMaxSpeed = 0.1

        for axisNames in motors:
            axis = content[axisNames]
            stiffness_value = axis["s"]

            if stiffness_value:
                motion_service.setStiffnesses(axis["sa"], float(stiffness_value))
            else:
                print("Stiffness value is empty for axis:", axisNames)

            names.append(axis["ma"])
            if axis["mv"]:
                try:
                    motor_value = float(axis["mv"])
                    angles.append(motor_value * almath.TO_RAD)
                except ValueError as e:
                    print("Error converting motor value to float:", e)
            else:
                print("Empty motor value encountered for axis:", axisNames)

            if axis["sp"]:
                try:
                    fractionMaxSpeed = float(axis["sp"])
                except ValueError as e:
                    print("Error converting speed value to float:", e)
            else:
                print("Empty speed value encountered for axis:", axisNames)

        names = [name for name in names if name]

        if names:
            motion_service.setAngles("HeadYaw", angles, fractionMaxSpeed)
        else:
            print("No valid joint names provided. Unable to set angles.")

        time.sleep(3.0)  # TODO adjust stiffness
        for axisNames in motors:
            axis = content[axisNames]
            motion_service.setStiffnesses(axis["sa"], 0.0)
