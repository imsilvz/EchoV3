import React from 'react';
import { assert, describe, expect, it } from 'vitest';
import { render, screen } from '@testing-library/react';
import ChatMessage, { MessageTypeSettings } from './ChatMessage';

describe('Chat Message Rendering', () => {
  it('basic message test', () => {
    render(
      <ChatMessage
        message={{
          echoType: 'Chat',
          timestamp: new Date(Date.now()).toISOString(),
          sourceActorId: 1,
          destinationActorId: 1,
          messageType: 'Say',
          senderId: 2,
          senderName: 'Joe Test',
          senderActor: undefined,
          message: 'This is a test message.',
        }}
      />,
    );

    const chatMessage = screen.queryByTestId('chat-message');
    const messageSender = screen.queryByTestId('chat-message-sender');
    const messageContent = screen.queryByTestId('chat-message-content');

    // in dom
    expect(chatMessage).toBeInTheDocument();
    expect(messageSender).toBeInTheDocument();
    expect(messageContent).toBeInTheDocument();

    // check contents
    expect(messageSender).toHaveTextContent('Joe Test:');
    expect(messageContent).toHaveTextContent('This is a test message.');
  });

  const messageTypes = Array.from(Object.keys(MessageTypeSettings));
  messageTypes.forEach((key) => {
    it(`messagetype test: ${key}`, () => {
      const typeSettings = MessageTypeSettings[key];
      render(
        <ChatMessage
          message={{
            echoType: 'Chat',
            timestamp: new Date(Date.now()).toISOString(),
            sourceActorId: 1,
            destinationActorId: 1,
            messageType: key,
            senderId: 2,
            senderName: 'Joe Test',
            senderActor: undefined,
            message: 'This is "a test" message.',
          }}
        />,
      );

      const chatMessage = screen.queryByTestId('chat-message');
      const messageSender = screen.queryByTestId('chat-message-sender');
      const messageContent = screen.queryByTestId('chat-message-content');

      // in dom
      expect(chatMessage).toBeInTheDocument();
      expect(messageSender).toBeInTheDocument();
      expect(messageContent).toBeInTheDocument();

      // sender can vary, but message rarely does.
      expect(messageContent).toHaveTextContent('This is "a test" message.');

      const quoteHighlight = screen.queryByTestId('quoted-span');
      if (typeSettings.RoleplayHighlight) {
        expect(quoteHighlight).toBeInTheDocument();
        expect(quoteHighlight).toHaveTextContent('"a test"');
      } else {
        expect(quoteHighlight).not.toBeInTheDocument();
      }
    });
  });
});
