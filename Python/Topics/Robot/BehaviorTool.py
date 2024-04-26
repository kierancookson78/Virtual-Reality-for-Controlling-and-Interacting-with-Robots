import time
from ..MessageQueue import MessageQueue
import json

class BehaviorTool():
    def __init__(self, behavior_mng_service, memory_service, connection):
        self.behavior_mng_service = behavior_mng_service
        self.memory_service = memory_service

        self.message_queue = MessageQueue()
        self.interrupt_message = None

        def callback():
            self.try_dequeue_behavior()

        self.status = self.memory_service.subscriber("behaviorStopped")
        self.status.signal.connect(callback)

        self.lastbehavior = ""
        self.last_status = ""

        self.connection = connection

    def queue_behavior(self, message):
        self.message_queue.queue_message(message)
        self.try_dequeue_behavior()

    def try_dequeue_behavior(self):
        if(self.last_status == "stopped" or self.last_status == "done" or self.last_status == ""):
            if(self.interrupt_message is not None):
                content = self.interrupt_message["content"]
                self.startBehavior(content)
                self.interrupt_message = None
            else:
                dequeued_message = self.message_queue.try_dequeue_message()
                if (dequeued_message is not None):
                    content = dequeued_message["content"]
                    self.startBehavior(content)

    def clear_queues(self):
        self.message_queue.clear_queues()

    def skip_queue(self, message):
        self.interrupt_message = message
        #stop current behavior

    def interpretContent(self, message):
        content = message["content"]

        if content["behavior_command"] == "running":
            names = self.behavior_mng_service.getRunningBehaviors()
            print("Running:")
            print(names)

        elif content["behavior_command"] =="stopall":
            self.stopAllBehaviors()

        elif content["behavior_command"] == "start":
            self.startBehavior(content)

        elif content["behavior_command"] == "list":
            installed_names = self.behavior_mng_service.getInstalledBehaviors()
            print("Installed:")
            print(installed_names)

        elif content["behavior_command"] == "stop":
            self.stopBehavior(message)

        elif content["behavior_command"] == "startPrevious":
            if self.lastbehavior == "":
                print("No behavior has been run during this session")
            try:
                self.launchAndStopBehavior(self.behavior_mng_service, self.lastbehavior)
            except Exception as e:
                print str(e)

        elif content["behavior_command"] == "help":
            print("\nList of available commands:\n  help - Shows information on the available commands\n\n  start - Prompts you to enter the "
                  "name of the behavior you want to start\n\n  stop - Prompts you to enter the name of the behavior you want to stop\n\n"
                  "  list - lists all installed behaviors\n\n  running - lists all running behaviors\n\n  stopall - attempts to stop all running behaviors\n\n")

        elif content["behavior_command"] == "getBehavior":
            self.getBehaviors()

        elif content["behavior_command"] == "getDefaultBehaviors":
            self.printDefaultBehaviors()

        elif content["behavior_command"] == "addDefaultBehavior":
            self.addDefaultBehavior(content["behaviorName"])

        elif content["behavior_command"] == "launchAndStopBehavior":
            self.launchAndStopBehavior(content["behaviorName"])

    def getBehaviors(self):
        """
        Know which behaviors are on the robot.
        """

        installed_behaviors = self.behavior_mng_service.getInstalledBehaviors()
        print "Behaviors on the robot:"
        print installed_behaviors

        bodyTalk = []
        emotions = []
        gestures1 = []
        gestures2 = []
        gestures3 = []
        allGestures = []
        reactions = []
        waiting = []
        misc = []

        gesture_size = 0

        for behavior in installed_behaviors:
            if "animations/Stand" in behavior:

                behavior = str(behavior).strip("animations/Stand")

                if "BodyTalk" in str(behavior):
                    bodyTalk.append(behavior)
                elif "Emotions" in str(behavior):
                    emotions.append(behavior)
                elif "Gestures" in str(behavior):
                    gesture_size += len(str(behavior))
                    allGestures.append(behavior)
                    if gesture_size < 3000:
                        gestures1.append(behavior)
                    elif gesture_size < 6000:
                        gestures2.append(behavior)
                    else:
                        gestures3.append(behavior)
                elif "Reactions" in str(behavior):
                    reactions.append(behavior)
                elif "Waiting" in str(behavior):
                    waiting.append(behavior)
            else:
                misc.append(behavior)

        bodyTalk_list = {"type": "BODY_TALK", "values": bodyTalk}
        bodyTalk_JSON = json.dumps(bodyTalk_list)
        self.connection.sendall(bodyTalk_JSON.encode())
        time.sleep(0.5)
        emotions_list = {"type": "EMOTIONS", "values": emotions}
        emotions_JSON = json.dumps(emotions_list)
        self.connection.sendall(emotions_JSON.encode())
        time.sleep(0.5)
        gestures1_list = {"type": "GESTURES", "values": gestures1}
        gestures1_JSON = json.dumps(gestures1_list)
        self.connection.sendall(gestures1_JSON.encode())
        time.sleep(0.5)
        gestures2_list = {"type": "GESTURES", "values": gestures2}
        gestures2_JSON = json.dumps(gestures2_list)
        self.connection.sendall(gestures2_JSON.encode())
        time.sleep(0.5)
        gestures3_list = {"type": "GESTURES", "values": gestures3}
        gestures3_JSON = json.dumps(gestures3_list)
        self.connection.sendall(gestures3_JSON.encode())
        time.sleep(0.5)
        reactions_list = {"type": "REACTIONS", "values": reactions}
        reactions_JSON = json.dumps(reactions_list)
        self.connection.sendall(reactions_JSON.encode())
        time.sleep(0.5)
        waiting_list = {"type": "WAITING", "values": waiting}
        waiting_JSON = json.dumps(waiting_list)
        self.connection.sendall(waiting_JSON.encode())
        time.sleep(0.5)
        misc_list = {"type": "MISC", "values": misc}
        misc_JSON = json.dumps(misc_list)
        self.connection.sendall(misc_JSON.encode())
        time.sleep(0.5)
        done = "done"
        self.connection.sendall(done.encode())

    def sendJSON(self, type, list):

        x = {"type": type, "values": list}
        y = json.dumps(x)
        self.connection.sendall(y.encode())

    def getRunningBehaviors(self):
        running_behaviors = self.behavior_mng_service.getRunningBehaviors()
        print "Running behaviors:"
        print running_behaviors


    def launchAndStopBehavior(self, behavior_name, async):
        """
        Launch and stop a behavior, if possible.
        """
        # Check that the behavior exists.
        if (self.behavior_mng_service.isBehaviorInstalled(behavior_name)):
            # Check that it is not already running.
            if (not self.behavior_mng_service.isBehaviorRunning(behavior_name)):
                # Launch behavior. This is a blocking call, use _async=True if you do not
                # want to wait for the behavior to finish.
                if async == "true":
                    self.behavior_mng_service.runBehavior(behavior_name, _async=True)
                    time.sleep(0.5)
                else:
                    self.behavior_mng_service.runBehavior(behavior_name, _async=False)
                    time.sleep(0.5)
            else:
                print "Behavior is already running."

        else:
            print "Behavior not found."
        return

        names = behavior_mng_service.getRunningBehaviors()
        print "Running behaviors:"
        print names

        # Stop the behavior.
        if (behavior_mng_service.isBehaviorRunning(behavior_name)):
            behavior_mng_service.stopBehavior(behavior_name)
            time.sleep(1.0)
        else:
            print "Behavior is already stopped."

        names = behavior_mng_service.getRunningBehaviors()
        print "Running behaviors:"
        print names

    def startBehavior(self, content):
        behavior_name = content["behaviorName"]
        async = content["async"]
        try:
            self.lastbehavior = behavior_name
            self.launchAndStopBehavior(behavior_name, "true")

        except Exception as e:
            self.lastbehavior = ""
            print str(e)

    def stopBehavior(self, message):
        content = message["content"]
        behaviorName = content["behaviorName"]
        try:
            self.behavior_mng_service.stopBehavior(behaviorName)
        except Exception as e:
            print str(e)

    def stopAllBehaviors(self):
        self.behavior_mng_service.stopAllBehaviors()


    def printDefaultBehaviors(self):
        names = self.behavior_mng_service.getDefaultBehaviors()
        print "Default behaviors:"
        print names

    def addDefaultBehavior(self, behavior_name):
        # Add behavior to default.
        self.behavior_mng_service.addDefaultBehavior(behavior_name)
