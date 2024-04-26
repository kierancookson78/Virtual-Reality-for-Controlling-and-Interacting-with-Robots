import json

class RobotAutonomy:

    autonomy_service = None
    autonomous_blinking = None
    background_movement = None
    basic_awareness = None
    listening_movement = None
    speaking_movement = None
    connection = None

    def __init__(self, autonomy_service, autonomous_blinking, background_movement, basic_awareness, listening_movement, speaking_movement, connection):
        self.autonomy_service = autonomy_service
        self.autonomous_blinking = autonomous_blinking
        self.background_movement = background_movement
        self.basic_awareness = basic_awareness
        self.listening_movement = listening_movement
        self.speaking_movement = speaking_movement
        self.connection = connection

    def interpret_content(self, message):
        content = message["content"]
        command = content["command"]

        if command == "SET_AUTONOMY":
            self.autonomy_service.setState(content["value"])
        elif command == "AUTONOMOUS_BLINKING":
            self.do_autonomous_blinking(content)
        elif command == "BACKGROUND_MOVEMENT":
            self.do_background_movement(content)
        elif command == "BASIC_AWARENESS":
            self.do_basic_awareness(content)
        elif command == "LISTENING_MOVEMENT":
            self.do_listening_movement(content)
        elif command == "SPEAKING_MOVEMENT":
            self.do_speaking_movement(content)

    def do_autonomous_blinking(self, content):
        subcommand = content["subcommand"]
        if subcommand == "set_enabled":
            value = content["value"]
            if value == "True":
                self.autonomous_blinking.setEnabled(True)
            else:
                self.autonomous_blinking.setEnabled(False)
        elif subcommand == "is_enabled":
            is_enabled = self.autonomous_blinking.isEnabled()
            x = {"type": "AUTONOMOUS_BLINKING", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())

    def do_background_movement(self, content):
        subcommand = content["subcommand"]
        if subcommand == "set_enabled":

            value = content["value"]
            if value == "True":
                self.background_movement.setEnabled(True)
            else:
                self.background_movement.setEnabled(False)
        elif subcommand == "is_enabled":
            is_enabled = self.background_movement.isEnabled()
            x = {"type": "BACKGROUND_MOVEMENT", "value_type:": "is_enabled", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "is_running":
            is_enabled = self.background_movement.isRunning()
            x = {"type": "BACKGROUND_MOVEMENT", "value_type:": "is_running", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())

    def do_basic_awareness(self, content):

        subcommand = content["subcommand"]

        if subcommand == "set_enabled":
            value = content["value"]
            if value == "True":
                self.basic_awareness.setEnabled(True)
            else:
                self.basic_awareness.setEnabled(False)
        elif subcommand == "is_enabled":
            is_enabled = self.basic_awareness.isEnabled()
            x = {"type": "BASIC_AWARENESS", "value_type:": "is_enabled", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "is_running":
            is_enabled = self.basic_awareness.isRunning()
            x = {"type": "BASIC_AWARENESS", "value_type:": "is_running", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "pause_awareness":
            self.basic_awareness.pauseAwareness()
        elif subcommand == "resume_awareness":
            self.basic_awareness.resumeAwareness()
        elif subcommand == "is_awareness_paused":
            is_paused = self.basic_awareness.isAwarenessPaused()
            x = {"type": "BASIC_AWARENESS", "value_type:": "is_paused", "value": str(is_paused)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "is_stimulus_detection_enabled":
            stim_type = content["value"]
            is_stimulus_detection_enabled = self.basic_awareness.isStimulusDetectionEnabled(stim_type)
            x = {"type": "BASIC_AWARENESS", "value_type:": "is_stimulus_detection_enabled", "value": str(is_stimulus_detection_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "set_stimulus_detection_enabled":
            stim_type = content["value"]
            enable = content["secondary_value"]
            self.basic_awareness.setStimulusDetectionEnabled(stim_type, enable)
        elif subcommand == "set_engagement_mode":
            mode_name = content["value"]
            self.basic_awareness.setEngagementMode(mode_name)
        elif subcommand == "get_engagement_mode":
            engagement_mode = self.basic_awareness.getEngagementMode()
            x = {"type": "BASIC_AWARENESS", "value_type:": "engagement_mode", "value": str(engagement_mode)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "engage_person":
            person_id = content["value"]
            success = self.basic_awareness.engagePerson(person_id)
            x = {"type": "BASIC_AWARENESS", "value_type:": "engage_person_success", "value": str(success)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "trigger_stimulus":
            position_frame_world = content["value"]
            target_found = self.basic_awareness.triggerStimulus(position_frame_world)
            x = {"type": "BASIC_AWARENESS", "value_type:": "trigger_stimulus_success", "value": str(target_found)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "set_tracking_mode":
            tracking_mode = content["value"]
            self.basic_awareness.setTrackingMode(tracking_mode)
        elif subcommand == "get_tracking_mode":
            tracking_mode = self.basic_awareness.getTrackingMode()
            x = {"type": "BASIC_AWARENESS", "value_type:": "tracking_mode", "value": str(tracking_mode)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "set_parameter":
            param_name = content["value"]
            param_value = content["secondary_value"]
            self.basic_awareness.setParameter(param_name, param_value)
        elif subcommand == "get_parameter":
            param_name = content["value"]
            param_value = self.basic_awareness.getParameter(param_name)
            x = {"type": "BASIC_AWARENESS", "value_type:": "param_name", "value_name:": str(param_name), "value": str(param_value)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "reset_all_parameters":
            self.basic_awareness.resetAllParameters()

    def do_listening_movement(self, content):
        subcommand = content["subcommand"]
        if subcommand == "set_enabled":
            value = content["value"]
            if value == "True":
                self.listening_movement.setEnabled(True)
            else:
                self.listening_movement.setEnabled(False)
        elif subcommand == "is_enabled":
            is_enabled = self.listening_movement.isEnabled()
            x = {"type": "LISTENING_MOVEMENT", "value_type:": "is_enabled", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "is_running":
            is_enabled = self.listening_movement.isRunning()
            x = {"type": "LISTENING_MOVEMENT", "value_type:": "is_running", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())

    def do_speaking_movement(self, content):
        subcommand = content["subcommand"]
        if subcommand == "set_enabled":
            value = content["value"]
            if value == "True":
                self.speaking_movement.setEnabled(True)
            else:
                self.speaking_movement.setEnabled(False)
        elif subcommand == "is_enabled":
            is_enabled = self.speaking_movement.isEnabled()
            x = {"type": "SPEAKING_MOVEMENT", "value_type:": "is_enabled", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "is_running":
            is_enabled = self.speaking_movement.isRunning()
            x = {"type": "SPEAKING_MOVEMENT", "value_type:": "is_running", "value": str(is_enabled)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "set_mode":
            mode = content["value"]
            self.speaking_movement.setMode(mode)
        elif subcommand == "get_mode":
            mode = self.speaking_movement.getMode()
            x = {"type": "SPEAKING_MOVEMENT", "value_type:": "get_mode", "value": str(mode)}
            y = json.dumps(x)
            self.connection.sendall(y.encode())
        elif subcommand == "add_tags_to_words":
            tag = content["value"]
            words = content["secondary_value"]
            ttw = {tag: words}
            self.speaking_movement.addTagsToWords(ttw)
        elif subcommand == "reset_tags_to_words":
            self.speaking_movement.resetTagsToWords()