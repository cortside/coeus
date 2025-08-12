import React from 'react';

type MessageProps = {
  sender: string;
  text: string;
};

const Message: React.FC<MessageProps> = ({ sender, text }) => (
  <div className="mb-2">
    <span className="font-bold mr-2">{sender}:</span>
    <span>{text}</span>
  </div>
);

export default Message;
