import threading
import time

import Topics.Robot.Speech as Speech
import Topics.Robot.Movement as Movement
import Topics.Robot.MotorControl as MotorControl
import Topics.Robot.BehaviorTool as BehaviorTool
import Topics.Robot.Tablet as Tablet
import Topics.Robot.Memory_Data as Memory_Data
import Topics.Robot.RobotAutonomy as RobotAutonomy
from naoqi import *

import almath


class MoveThread(threading.Thread):
    def __init__(self, content, motion_service, speech_service, conn):
        threading.Thread.__init__(self)
        self.content = content
        self.motion_service = motion_service
        self.speech_service = speech_service
        self.conn = conn

    def send_data(self, message):
        print("TO TCP CLIENT sending: " + message)
        self.conn.sendall(message.encode())

    def run(self):
        self.speech_service.say("Starte Bewegung")
        motionResult = True

        distance = self.content["x"]
        angle = self.content["theta"]
        # ONLY SEND ROTATE COMMAND IF ANGLE > 0
        if abs(angle) > 0.01:
            print("COMMAND 1 TO ROBOT sending: moveTo(", 0, ", ", 0, ", ", angle, ")")
            motionResult = self.motion_service.moveTo(0, 0, angle)
            print("result: " + str(motionResult))

        # ONLY WALK IF TURN DID NOT FAIL
        if motionResult:
            time.sleep(1)
            # SEND MOVE COMMAND
            print("COMMAND 2 TO ROBOT sending: moveTo(", distance, ", ", 0, ", ", 0, ")")
            motionResult = self.motion_service.moveTo(distance, 0, 0)
            print("result: " + str(motionResult))
        else:
            self.speech_service.say("Drehung fehlgeschlagen")

        if not motionResult:
            self.speech_service.say("Bewegung fehlgeschlagen")

        # Reply to TCP Client with move result
        self.send_data('{"type":"motionResult", "content":"' + str(motionResult) + '"}')


class Interpreter:
    def __init__(self):
        self.session = None
        self.motion_service = None
        self.speech_service = None
        self.tablet_service = None
        self.behavior_mng_service = None
        self.diagnosis_service = None
        self.recharge_service = None
        self.memory_service = None
        self.posture_service = None
        self.autonomy_service = None
        self.autonomous_blinking = None
        self.background_movement = None
        self.basic_awareness = None
        self.listening_movement = None
        self.speaking_movement = None

        self.speech = None
        self.movement = None
        self.behavior_tool = None
        self.tablet = None
        self.autonomy = None

        self.temp_message = None



    def interpret_content(self, message, conn):  # motion_service, speech_service,
        # behavior_mng_service, diagnosis_service):

        if message["topic"] != "init" and self.speech_service is None:
            conn.sendall("accept".encode())
            print "accept 1: crash prevention"
            self.temp_message = message
            return

        print message["topic"]
        if message["topic"] == "testTopic":
            if message["subtopic"] == "testSubtopic":
                print message["priority"]
                content = message["content"]
                print content["gameObject"]
                print content["vector3x"]
                print content["vector3y"]
                print content["vector3z"]
                print content["value"]
                print content["boolean"]
                print content["text"]

        elif message["topic"] == "init":
            content = message["content"]

            IP = content["IP"]
            port = content["port"]
            autonomy_mode = content["autonomy_mode"]

            self.session = qi.Session()
            try:
                self.session.connect("tcp://" + str(IP) + ":" + str(port))
            except RuntimeError:
                print ("Can't connect to Naoqi at ip \"" + str(IP) + "\" on port " +
                       str(port) + ".\nPlease check your script arguments. Run with -h option for help.")
                sys.exit(1)

            self.motion_service = self.session.service("ALMotion")

            self.speech_service = self.session.service("ALTextToSpeech")

            self.behavior_mng_service = self.session.service("ALBehaviorManager")

            self.diagnosis_service = self.session.service("ALDiagnosis")

            self.memory_service = self.session.service("ALMemory")

            self.posture_service = self.session.service("ALRobotPosture")

            self.motion_service = self.session.service("ALMotion")
            if self.temp_message is None:
                self.speech_service.say("verbunden")

            # self.background_movement = self.session.service("ALBackgroundMovement")
            # self.basic_awareness = self.session.service("ALBasicAwareness")
            # self.listening_movement = self.session.service("ALListeningMovement")
            # self.speaking_movement = self.session.service("ALSpeakingMovement")

            self.speech = Speech.Speech(self.speech_service, self.memory_service, conn)
            self.movement = Movement.Movement(self.motion_service, self.speech_service, self.memory_service, conn)
            self.behavior_tool = BehaviorTool.BehaviorTool(self.behavior_mng_service, self.memory_service, conn)

            message = "successfully connected"
            conn.sendall(message.encode())

            if self.temp_message is not None:
                self.interpret_content(self.temp_message, conn)
                self.temp_message = None

        elif message["topic"] == "environment":
            if message["subtopic"] == "light":
                print "light"
            if message["subtopic"] == "fan":
                print "fan"
            if message["subtopic"] == "window":
                print "window"

        elif message["topic"] == "robot":
            if message["subtopic"] == "move":
                self.movement.interpretContent(message)
            elif message["subtopic"] == "speech":
                self.speech.interpretContent(message)
            elif message["subtopic"] == "motorControl":
                MotorControl.MotorControl.interpret(self.motion_service, message, conn, self.posture_service)
            elif message["subtopic"] == "behaviorTool":
                self.behavior_tool.interpretContent(message)
            elif message["subtopic"] == "memory_data":
                Memory_Data.Memory_Data.interpretContent(self.memory_service, conn, message)
                print ""

        elif message["topic"] == "user":
            if message["subtopic"] == "mood":
                print ""
            if message["subtopic"] == "userPosition":
                print ""

        elif message["topic"] == "externalTools":
            if message["subtopic"] == "weather":
                print ""
            if message["subtopic"] == "news":
                print ""
            if message["subtopic"] == "speechRecognition":
                print ""
            if message["subtopic"] == "userPosition":
                print ""
        else:
            print("Command: " + message["topic"] + " unknown.")
