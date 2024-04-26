import threading
import time
from ..MessageQueue import MessageQueue

class Movement():
    motion_service = None
    speech_service = None
    connection = None
    last_status = ""

    def __init__(self, motion_service, speech_service, memory_service, connection):
        self.motion_service = motion_service
        self.speech_service = speech_service
        self.memory_service = memory_service

        self.connection = connection

        def callback(value):
            print value[1]
            self.last_status = value[1]
            self.try_dequeue_move()

        self.status = self.memory_service.subscriber("")
        self.status.signal.connect(callback)

        self.message_queue = MessageQueue()
        self.interrupt_message = None

        def move_failed_callback(value):
            print value[1]
            if(value[1] == True):
                self.speech_service.say("Bewegung Fehlgeschlagen")

        self.move_failed_status = self.memory_service.subscriber("ALMotion/MoveFailed")
        self.move_failed_status.signal.connect(move_failed_callback)

    def interpretContent(self, message):
        content = message["content"]

        if content["move_command"] == "move_move":
            self.move(content)
        if content["move_command"] == "move_toward":
            self.move_toward(content)
        if content["move_command"] == "move_moveTo":
            self.moveTo(message)
        if content["move_command"] == "move_isActive":
            self.moveIsActive()
        if content["move_command"] == "move_waitMoveFinished":
            self.move_waitMoveFinished()
        if content["move_command"] == "move_stopMove":
            self.stopMove()
        if content["move_command"] == "move_getRobotPosition":
            self.getRobotPosition()
        if content["move_command"] == "":
            self.getRobotPositionNoSensors()
        if content["move_command"] == "move_getNextRobotPosition":
            self.getNextRobotPosition()
        if content["move_command"] == "move_getRobotVelocity":
            self.getRobotVelocity()
        if content["move_command"] == "move_getMoveConfig":
            self.getMoveConfig()

    def queue_move(self, message):
        self.message_queue.queue_message(message)
        self.try_dequeue_move()

    def try_dequeue_move(self):
        if(self.last_status == "stopped" or self.last_status == "done" or self.last_status == ""):
            if(self.interrupt_message is not None):
                self.moveTo(self.interrupt_message)
                self.interrupt_message = None
            else:
                dequeued_message = self.message_queue.try_dequeue_message()
                if (dequeued_message is not None):
                    self.moveTo(dequeued_message)

    def clear_queues(self):
        self.message_queue.clear_queues()

    def skip_queue(self, message):
        self.interrupt_message = message
        self.stopMove()

    def move(self, content):
        self.motion_service.move(float(content["x"]), float(content["y"]), float(content["theta"]))

    def move_toward(self, content):
        self.motion_service.moveToward(float(content["x"]), float(content["y"]), float(content["theta"]))

    def moveTo(self, message):
        move_thread = MoveThread(message, self.motion_service, self.speech_service, self.connection)
        move_thread.start()

    def moveIsActive(self):
        isActive = self.motion_service.moveIsActive()
        self.send(str(isActive))

    def move_waitMoveFinished (self):
        self.motion_service.waitUntilMoveIsFinished() #block until move is finished
        self.send("move finished")

    def stopMove(self):
        stopMotion = self.motion_service.stopMove()
        print stopMotion
        self.send(str(stopMotion))

    def getRobotPosition(self):
        position = self.motion_service.getRobotPosition(True)
        self.send(str(position))

    def getRobotPositionNoSensors(self):
        position = self.motion_service.getRobotPosition(False)
        self.send(str(position))

    def getNextRobotPosition(self):
        nextPosition = self.motion_service.getNextRobotPosition()
        self.send(str(nextPosition))

    def getRobotVelocity(self):
        velocity = self.motion_service.getRobotVelocity()
        self.send(str(velocity))

    def getMoveConfig(self, content):
        config = content["config"]
        result = self.motion_service.getMoveConfig(config)
        self.send(str(result))

    def send(self, message):
        print "sending: " + message.encode()
        self.connection.sendall(message.encode())

    def notify(self, value):
        print value


class MoveThread(threading.Thread):
    def __init__(self, message, motion_service, speech_service, conn):
        threading.Thread.__init__(self)
        self.content = message["content"]
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
        else:
            print("Angle smaller 0.01")

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

class IsMovingThread(threading.Thread):
    def __init__(self, motion_service):
        threading.Thread.__init__(self)
        self.motion_service = motion_service
        self.motion_status = self.motion_service.moveIsActive()
        self.subscribers = []

    def notify_subscribers(self):
        for subscriber in self.subscribers:
            subscriber.notify(self.motion_status)

    def run(self):
        while True:
            if(self.motion_service.moveIsActive() != self.motion_status):
                self.motion_status = self.motion_service.moveIsActive()
                self.notify_subscribers()

    def subscribe(self, new_subscriber):
        self.subscribers.append(new_subscriber)