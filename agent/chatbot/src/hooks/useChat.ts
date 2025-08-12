import { useState } from 'react';
import { sendChatMessage } from '../api/backend';
import { useAuth } from './useAuth';

export function useChat() {
  const [messages, setMessages] = useState<{ sender: string; text: string }[]>([]);
  const { token } = useAuth();

  async function sendMessage(text: string) {
    setMessages(msgs => [...msgs, { sender: 'You', text }]);
    if (!token) return;
    // For demo: just echo
    setMessages(msgs => [...msgs, { sender: 'Bot', text: '...' }]);
    // Uncomment for real backend:
    // const response = await sendChatMessage(token, text);
    // setMessages(msgs => [...msgs, { sender: 'Bot', text: response }]);
  }

  return { messages, sendMessage };
}
