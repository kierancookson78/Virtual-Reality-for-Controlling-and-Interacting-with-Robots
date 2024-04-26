from Queue import Queue

class MessageQueue:
    interrupt_message = None

    def __init__(self):
        self.low_priority_queue = Queue()
        self.mid_priority_queue = Queue()
        self.high_priority_queue = Queue()

    def queue_message(self, message):
        priority = message["priority"]
        if priority == "1":
            self.low_priority_queue.put(message)
        if priority == "2":
            self.mid_priority_queue.put(message)
        if priority == "3":
            self.high_priority_queue.put(message)

    def try_dequeue_message(self):
        if (self.high_priority_queue.qsize() > 0):
            return self.high_priority_queue.get()
        elif (self.mid_priority_queue.qsize() > 0):
            return self.mid_priority_queue.get()
        elif (self.low_priority_queue.qsize() > 0):
            return self.low_priority_queue.get()
        return None

    def is_empty(self):
        if (self.high_priority_queue.qsize() > 0):
            return False
        elif (self.mid_priority_queue.qsize() > 0):
            return False
        elif (self.low_priority_queue.qsize() > 0):
            return False
        return True

    def clear_queues(self):
        self.low_priority_queue.clear()
        self.mid_priority_queue.clear()
        self.high_priority_queue.clear()