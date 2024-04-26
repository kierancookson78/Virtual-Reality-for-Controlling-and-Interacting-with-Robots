import socket
import threading
import json
import GeneralInterpreter
import time
from datetime import datetime

class Server:
    def __init__(self):
        s = socket.socket()
        host = '127.0.0.1'
        port = 65431

        print "Server started... Waiting for clients..."

        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        s.bind((host, port))
        s.listen(1)

        while True:
            self.cs, addr = s.accept()
            t = threading.Thread(target=self.new_client, args=(self.cs, addr))
            t.start()
            print "Client accepted: " + str(addr) + " / " + str(self.cs)
            self.cs.sendall("client accepted".encode())
        s.close()

    def new_client(self, clientsocket, addr):
        interpreter = GeneralInterpreter.Interpreter()

        while True:
            msg = clientsocket.recv(1024 * 4)
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
                clientsocket.sendall("accept".encode())
                continue
            if msg == "dc":
                break

            time_stamp = time.time()
            date_time = datetime.fromtimestamp(time_stamp)
            print("Timestamp Message Received:", date_time)

            message = json.loads(msg)
            interpreter.interpret_content(message, clientsocket)

        clientsocket.close()
        print("disconnected")