import threading
from Queue import Queue
from ..MessageQueue import MessageQueue
import time
from datetime import datetime
import json


class SpeechThread(threading.Thread):
    def __init__(self, text, speech_service):
        threading.Thread.__init__(self)
        self.text = text
        self.speech_service = speech_service

    def run(self):
        self.speech_service.say(self.text)

class Speech:
    speech_service = None
    last_status = ""
    interrupt_message = None
    connection = None
    current_sentence = ""
    current_word = ""

    def __init__(self, speech_service, memory_service, connection):
        self.speech_service = speech_service
        self.memory_service = memory_service
        self.connection = connection

        def callback(value):
            print value[1]
            self.last_status = value[1]
            self.try_dequeue_speech(None)

            x = {"speech_status": str(self.last_status), "current_sentence": str(self.current_sentence)}
            y = json.dumps(x)
            connection.sendall(y.encode())

        self.status = self.memory_service.subscriber("ALTextToSpeech/Status")
        self.status.signal.connect(callback)

        def currentSentenceCallback(value):
            self.current_sentence = value
            print("Current sentence: " + value)

        self.status = self.memory_service.subscriber("ALTextToSpeech/CurrentSentence")
        self.status.signal.connect(currentSentenceCallback)

        #Uncomment to also send current word. Current word is not very accurate.
        #def currentWordCallback(value):
            #self.current_word = value
            #print("Current word: " + value)

        #self.status = self.memory_service.subscriber("ALTextToSpeech/CurrentWord")
        #self.status.signal.connect(currentWordCallback)


        self.message_queue = MessageQueue()


    def interpretContent(self, message):
        content = message["content"]
        command = content["command"]
        value = content["value"]
        if command == "STOP":
            self.stop_speech()
        elif command == "SPEAK" or command == "SEND_ALL":
            self.queue_speech(message)
        elif command == "VOLUME":
            self.set_volume(value)
        elif command == "LANGUAGE":
            self.set_language(value)
        elif command == "VOICE":
            self.set_voice(value)
        elif command == "SPEED":
            self.set_speed(value)
        elif command == "PITCH":
            self.set_pitch(value)
        elif command == "SKIP_QUEUE":
            self.skip_queue(message)
        elif command == "CLEAR_QUEUE":
            self.clear_queues()



    def queue_speech(self, message):
        self.message_queue.queue_message(message)
        self.try_dequeue_speech(None)

    def try_dequeue_speech(self, interrupt_message_):
        if(self.last_status == "stopped" or self.last_status == "done" or self.last_status == ""):
            if(interrupt_message_ is not None):
                self.speak(interrupt_message_)
                interrupt_message_ = None
            else:
                dequeued_message = self.message_queue.try_dequeue_message()
                if(dequeued_message is not None):
                    self.speak(dequeued_message)

    def clear_queues(self):
        self.message_queue.clear_queues()

    def skip_queue(self, message):
        self.interrupt_message = message
        self.stop_speech()
        self.try_dequeue_speech(message)

    def speak(self, message):

        time_stamp = time.time()
        date_time = datetime.fromtimestamp(time_stamp)
        print("Timestamp Speech Started:", date_time)

        Speech.say(self, message)

        time_stamp = time.time()
        date_time = datetime.fromtimestamp(time_stamp)
        print("Timestamp Speech Finished:", date_time)


    def say(self, message):
        content = message["content"]
        command = content["command"]

        if command == "SEND_ALL":
            value = content["value"]

            text = value["text"]
            volume = value["volume"]
            language = value["language"]
            voice = value["voice"]
            pitch = value["pitch"]
            speed = value["speed"]

            self.set_volume(volume)
            self.set_language(language)
            self.set_voice(voice)
            self.set_pitch(pitch)
            self.set_speed(speed)
            speech_thread = SpeechThread(text, self.speech_service)
            speech_thread.start()

        else:
            speech_thread = SpeechThread(content["value"], self.speech_service)
            speech_thread.start()

    #Setter
    def set_volume(self, value):
        self.speech_service.setVolume(float(value))

    def set_language(self, value):
        self.speech_service.setLanguage(value)

    def set_voice(self, value):
        self.speech_service.setVoice(value)

    def set_speed(self, value):
        self.speech_service.setParameter("speed", float(value))

    def set_pitch(self, value):
        self.speech_service.setParameter("pitchShift", float(value))

    #ALAnimatedSpeech
    def say_animated(self, animated_speech_service, message):
        content = message["content"]
        animated_speech_service.say(content["text"])

    #ALAudioPlayerProxy
    def play_audio_file(self, audio_player_service, message):
        content = message["content"]
        fileId = audio_player_service.loadFile(content["fileId"])
        audio_player_service.play(fileId)

    def stop_audio_file(self, audio_player_service):
        audio_player_service.stopAll()

    def stop_speech(self):
        self.speech_service.stopAll()
