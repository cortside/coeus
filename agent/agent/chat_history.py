from collections import defaultdict, deque

class ChatHistory:
    def __init__(self):
        self.history = defaultdict(lambda: deque(maxlen=50))

    def add_message(self, user_id, message):
        self.history[user_id].append(message)

    def get_history(self, user_id):
        return list(self.history[user_id])
