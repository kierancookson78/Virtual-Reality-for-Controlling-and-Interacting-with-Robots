import socket
import threading
import time
import json


if __name__ == "__main__":
    port = 65431
    from Mock import Mock
    m = Mock(port)

class Mock:
    def __init__(self, PORT):

        # Standard loopback interface address (localhost)
        # -> this way it will use the default local network interface (the ip of the pc in the network used) to accept client sockets
        self.HOST = '127.0.0.1'
        # Port to listen on (non-privileged ports are > 1023)
        self.PORT = PORT

        # create socket with address protocol = AF_INET (IPv4) and connection protocol = SOCK_STREAM (TCP)
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        # bind socket to network interface (with its ip address and given port)
        s.bind((self.HOST, self.PORT))

        print("mock created with port " + str(self.PORT))
        print("waiting for client connection")
        # this is the TCP handshake, the conn object will be used to send and receive data
        s.listen(1)
        self.conn, addr = s.accept()

        print("client connected with ip " + str(addr))

        # start new thread to listen to received data in background forever
        receive_data_thread = threading.Thread(target=self.listen_for_data, args=())
        receive_data_thread.start()

    def listen_for_data(self):
        while True:
            msg = self.conn.recv(1024)

            if msg == "" or msg is None:
                continue
            if "~" in msg:
                msg = msg.split('~')[1]
            else:
                continue
            if "#" in msg:
                msg = msg.split('#')[0]
            else:
                continue
            print("Processed message: >" + str(msg) + "<")

            if msg == "connect":
                self.send_data("accept")
                continue

            message = json.loads(msg)

            self.message_received(message)
            if not message:
                break

    def message_received(self, message):

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

        elif message["topic"] == "environment":
            if message["subtopic"] == "light":
                print "[MOCK] light"
            if message["subtopic"] == "fan":
                print "[MOCK] fan"
            if message["subtopic"] == "window":
                print "[MOCK] window"

        elif message["topic"] == "robot":
            if message["subtopic"] == "move":
                print "[MOCK] move"
                content = message["content"]
                print "[MOCK] text " + content["text"]
                print "[MOCK] volume " + content["volume"]
                print "[MOCK] language " + content["language"]
                print "[MOCK] voice " + content["voice"]
                print "[MOCK] speed " + content["speed"]
            elif message["subtopic"] == "motorControl":
                print "[MOCK] motor control"
            elif message["subtopic"] == "behaviorTool":
                print "[MOCK] behavior tool"
            elif message["subtopic"] == "tablet":
                print "[MOCK] tablet"
            elif message["subtopic"] == "memory_data":
                print "[MOCK] memory & data"
            elif message["subtopic"] == "robotAutonomy":
                print "[MOCK] robot autonomy"

        elif message["topic"] == "user":
            if message["subtopic"] == "mood":
                print "[MOCK] mood"
            if message["subtopic"] == "userPosition":
                print "[MOCK] user position"

        elif message["topic"] == "externalTools":
            if message["subtopic"] == "weather":
                print "[MOCK] weather"
            if message["subtopic"] == "news":
                print "[MOCK] news"
            if message["subtopic"] == "speechRecognition":
                print "[MOCK] speech recognition"
            if message["subtopic"] == "userPosition":
                print "[MOCK] user position"
        print message
        
    def send_data(self, message):
        self.conn.sendall(message)
