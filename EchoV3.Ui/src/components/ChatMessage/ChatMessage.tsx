/* eslint-disable @typescript-eslint/no-non-null-assertion */
// react
import React from 'react';

// types
import * as ipc from '../../types/ipc';

// local
import './ChatMessage.scss';
import { GetNameColor } from './Utility/NameColorization';
import { RoleplayHighlight } from './Utility/RoleplayHighlight';
import { useAppSelector } from '../../redux/hooks';
import { selectPlayerDict } from '../../redux/reducers/actorReducer';
import { selectNameColorMode } from '../../redux/reducers/settingsReducer';

interface ChatMessageProps {
  message: ipc.ChatPayload;
}

interface MessageSetting {
  CSSClassName: string;
  ColoredNames: boolean;
  RoleplayHighlight: boolean;
  FormatSender?: (messageData: ipc.ChatPayload) => React.ReactNode;
  FormatMessage?: (messageData: ipc.ChatPayload) => React.ReactNode;
  Parse?: (messageData: ipc.ChatPayload) => React.ReactNode;
  GetMessageKey?: (messageData: ipc.ChatPayload) => string;
}

export const MessageTypeSettings: { [key: string]: MessageSetting } = {
  Default: {
    CSSClassName: 'msgtype-default',
    ColoredNames: false,
    RoleplayHighlight: false,
    FormatSender: function (messageData) {
      return this.ColoredNames ? (
        <>
          <span
            className="chat-message-sender"
            data-context-playerid={messageData.senderId}
            data-testid="chat-message-sender"
            style={{ color: GetNameColor(messageData) || 'inherit' }}
          >
            {messageData.senderName}
          </span>
          :{' '}
        </>
      ) : (
        <>
          <span
            className="chat-message-sender"
            data-context-playerid={messageData.senderId}
            data-testid="chat-message-sender"
          >
            {messageData.senderName}
          </span>
          :{' '}
        </>
      );
    },
    FormatMessage: function (messageData) {
      const msgKey = this.GetMessageKey!(messageData);
      if (this.RoleplayHighlight) {
        return RoleplayHighlight(msgKey, messageData.message);
      }
      return <>{messageData.message}</>;
    },
    Parse: function (messageData) {
      const defaultSettings = MessageTypeSettings['Default'];
      return (
        <p className={this.RoleplayHighlight ? 'quote-highlight' : undefined}>
          {this.FormatSender
            ? this.FormatSender(messageData)
            : (
                defaultSettings.FormatSender as (
                  messageData: ipc.ChatPayload,
                ) => React.ReactNode
              )(messageData)}
          <span className="chat-message-content" data-testid="chat-message-content">
            {this.FormatMessage
              ? this.FormatMessage(messageData)
              : (
                  defaultSettings.FormatMessage as (
                    messageData: ipc.ChatPayload,
                  ) => React.ReactNode
                )(messageData)}
          </span>
        </p>
      );
    },
    GetMessageKey: function (messageData) {
      return `${messageData.senderId}-${messageData.timestamp}`;
    },
  },
  System: {
    CSSClassName: 'msgtype-default',
    ColoredNames: false,
    RoleplayHighlight: false,
    Parse: function (messageData) {
      const defaultSettings = MessageTypeSettings['Default'];
      return (
        <p>
          {this.FormatSender
            ? this.FormatSender(messageData)
            : (
                defaultSettings.FormatSender as (
                  messageData: ipc.ChatPayload,
                ) => React.ReactNode
              )(messageData)}
          <span className="chat-message-content" data-testid="chat-message-content">
            {this.FormatMessage
              ? this.FormatMessage(messageData)
              : defaultSettings.FormatMessage!(messageData)}
          </span>
        </p>
      );
    },
  },
  Say: {
    CSSClassName: 'msgtype-say',
    ColoredNames: true,
    RoleplayHighlight: true,
    Parse: function (messageData) {
      const msgKey = this.GetMessageKey!(messageData);
      let roleplayHighlight;
      if (this.RoleplayHighlight) {
        const temp = RoleplayHighlight(msgKey, messageData.message);
        if (temp.length > 1) {
          roleplayHighlight = temp;
        }
      }
      return (
        <p className={roleplayHighlight !== undefined ? 'emote-mode' : undefined}>
          {this.FormatSender!(messageData)}
          <span className="chat-message-content" data-testid="chat-message-content">
            {roleplayHighlight !== undefined
              ? roleplayHighlight
              : this.FormatMessage!(messageData)}
          </span>
        </p>
      );
    },
  },
  Shout: {
    CSSClassName: 'msgtype-shout',
    ColoredNames: false,
    RoleplayHighlight: false,
  },
  CustomEmote: {
    CSSClassName: 'msgtype-emote',
    ColoredNames: true,
    RoleplayHighlight: true,
    FormatSender: function (messageData) {
      return this.ColoredNames ? (
        <>
          <span
            className="chat-message-sender"
            data-context-playerid={messageData.senderId}
            data-testid="chat-message-sender"
            style={{ color: GetNameColor(messageData) || 'inherit' }}
          >
            {messageData.senderName}
          </span>{' '}
        </>
      ) : (
        <>
          <span
            className="chat-message-sender"
            data-context-playerid={messageData.senderId}
            data-testid="chat-message-sender"
          >
            {messageData.senderName}
          </span>{' '}
        </>
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
  const nameColorMode = useAppSelector(selectNameColorMode);
  const playerActorDict = useAppSelector(selectPlayerDict);
  const messageSettings = MessageTypeSettings[message.messageType];
  const playerActor = playerActorDict[message.senderId];

  console.log('message', message);

  let msgClassName = 'chat-message';
  if (playerActor?.ignored) {
    msgClassName = 'chat-message msgtype-default';
  } else {
    if (messageSettings) {
      msgClassName = `chat-message ${messageSettings.CSSClassName}`;
    }
  }
  return (
    <div className={msgClassName} data-testid="chat-message">
      {playerActor?.ignored ? (
        (
          MessageTypeSettings['Default'].Parse as (
            messageData: ipc.ChatPayload,
          ) => React.ReactNode
        )({
          ...message,
          message: '<player ignored>',
        })
      ) : (
        <>
          {messageSettings?.Parse
            ? messageSettings?.Parse(message)
            : (
                MessageTypeSettings['Say'].Parse as (
                  messageData: ipc.ChatPayload,
                ) => React.ReactNode
              )(message)}
        </>
      )}
    </div>
  );
};
export default React.memo(ChatMessage);
