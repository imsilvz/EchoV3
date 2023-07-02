// react
import React from 'react';

// types
import * as ipc from '../../types/ipc';

// local
import './ChatMessage.scss';
import { GetNameColor } from './Utility/NameColorization';
import { QuoteHighlight } from './Utility/QuoteHighlight';

interface ChatMessageProps {
  message: ipc.ChatPayload;
}

interface MessageSetting {
  CSSClassName: string;
  ColoredNames: boolean;
  HighlightQuotes: boolean;
  FormatSender?: (messageData: ipc.ChatPayload) => React.ReactNode;
  FormatMessage?: (messageData: ipc.ChatPayload) => React.ReactNode;
  Parse?: (messageData: ipc.ChatPayload) => React.ReactNode;
}

const MessageTypeSettings: { [key: string]: MessageSetting } = {
  Default: {
    CSSClassName: 'msgtype-say',
    ColoredNames: false,
    HighlightQuotes: false,
    FormatSender: function (messageData) {
      let nameClass;
      return this.ColoredNames ? (
        <>
          <span className={nameClass} style={{ color: GetNameColor(messageData) }}>
            {messageData.senderName}
          </span>
          :{' '}
        </>
      ) : (
        <>{messageData.senderName}: </>
      );
    },
    FormatMessage: function (messageData) {
      if (this.HighlightQuotes) {
        return QuoteHighlight(messageData.message);
      }
      return <>{messageData.message}</>;
    },
    Parse: function (messageData) {
      const defaultSettings = MessageTypeSettings['Default'];
      return (
        <p className={this.HighlightQuotes ? 'quote-highlight' : undefined}>
          {this.FormatSender
            ? this.FormatSender(messageData)
            : (
                defaultSettings.FormatSender as (
                  messageData: ipc.ChatPayload,
                ) => React.ReactNode
              )(messageData)}
          {this.FormatMessage
            ? this.FormatMessage(messageData)
            : (
                defaultSettings.FormatMessage as (
                  messageData: ipc.ChatPayload,
                ) => React.ReactNode
              )(messageData)}
        </p>
      );
    },
  },
  Say: {
    CSSClassName: 'msgtype-say',
    ColoredNames: true,
    HighlightQuotes: true,
    Parse: function (messageData) {
      let quoteHighlight;
      if (this.HighlightQuotes) {
        const temp = QuoteHighlight(messageData.message);
        if (temp.length > 1) {
          quoteHighlight = temp;
        }
      }
      return (
        <p
          className={`${this.HighlightQuotes ? 'quote-highlight ' : ''}${
            quoteHighlight !== undefined ? 'emote-mode' : ''
          }`}
        >
          {this.FormatSender!(messageData)}
          {quoteHighlight !== undefined
            ? quoteHighlight
            : this.FormatMessage!(messageData)}
        </p>
      );
    },
  },
  Shout: {
    CSSClassName: 'msgtype-shout',
    ColoredNames: false,
    HighlightQuotes: false,
  },
  CustomEmote: {
    CSSClassName: 'msgtype-emote',
    ColoredNames: true,
    HighlightQuotes: true,
    FormatSender: function (messageData) {
      let nameClass;
      return this.ColoredNames ? (
        <>
          <span className={nameClass} style={{ color: GetNameColor(messageData) }}>
            {messageData.senderName}{' '}
          </span>
        </>
      ) : (
        <>{messageData.senderName} </>
      );
    },
  },
};

// ensure 'this' keyword does what we want
const MessageTypeKeys = Object.keys(MessageTypeSettings);
for (let i = 0; i < MessageTypeKeys.length; i++) {
  const messageTypeKey = MessageTypeKeys[i];
  MessageTypeSettings[messageTypeKey] = Object.assign(
    {},
    MessageTypeSettings['Default'],
    MessageTypeSettings[messageTypeKey] || {},
  );
}

const ChatMessage = ({ message }: ChatMessageProps) => {
  const messageSettings = MessageTypeSettings[message.messageType];

  let msgClassName = 'chat-message';
  if (messageSettings) {
    msgClassName = `chat-message ${messageSettings.CSSClassName}`;
  }
  return (
    <div className={msgClassName}>
      {messageSettings?.Parse
        ? messageSettings?.Parse(message)
        : (
            MessageTypeSettings['Say'].Parse as (
              messageData: ipc.ChatPayload,
            ) => React.ReactNode
          )(message)}
    </div>
  );
};
export default React.memo(ChatMessage);
