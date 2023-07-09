// react
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Virtuoso } from 'react-virtuoso';

// redux
import { useAppDispatch, useAppSelector } from '../redux/hooks';
import {
  selectChatSettings,
  selectListenerMode,
} from '../redux/reducers/settingsReducer';

// types
import '../types/global.d.ts';
import * as ipc from '../types/ipc';

// local
import './App.scss';
import ChatMessage from './ChatMessage/ChatMessage';
import ContextMenu, { ContextType } from './ContextMenu/ContextMenu';
import { addOrUpdatePlayer } from '../redux/reducers/actorReducer.ts';

const App = () => {
  const dispatch = useAppDispatch();
  const chatSettings = useAppSelector(selectChatSettings);
  const listenerMode = useAppSelector(selectListenerMode);
  const [currentTargetId, setCurrentTargetId] = useState<number>(-1);
  const [messageList, setMessageList] = useState<ipc.ChatPayload[]>([]);
  const [showContextMenu, setShowContextMenu] = useState<{
    show: boolean;
    contextType: ContextType;
    contextData?: unknown;
    xPos: number;
    yPos: number;
  }>({
    show: false,
    contextType: null,
    xPos: 0,
    yPos: 0,
  });

  // listener mode
  const targetMessageList = useMemo(() => {
    return messageList.filter((msg) => msg.senderId === currentTargetId);
  }, [currentTargetId, messageList]);

  const currentMessageList = useMemo(
    () =>
      (listenerMode ? targetMessageList : messageList).filter((msg) => {
        return chatSettings[msg.messageType as keyof typeof chatSettings] || false;
      }),
    [chatSettings, listenerMode, messageList, targetMessageList],
  );

  // event listener hook!
  useEffect(() => {
    window.chrome.webview.addEventListener('message', (arg) => {
      const payload = arg.data as ipc.IpcPayload;
      // console.log(arg.data);
      switch (payload.echoType) {
        case 'Chat':
          if ((payload as ipc.ChatPayload).senderName) {
            dispatch(
              addOrUpdatePlayer({
                actorId: (payload as ipc.ChatPayload).senderId,
                playerName: (payload as ipc.ChatPayload).senderName,
              }),
            );
          }
          setMessageList((currList) => [...currList, payload as ipc.ChatPayload]);
          break;
        case 'LocalTarget':
          setCurrentTargetId((payload as ipc.LocalTargetPayload).targetId);
          break;
        default:
          break;
      }
    });
  }, []);

  // context menu
  useEffect(() => {
    const contextMenuHandler = (e: MouseEvent) => {
      const eventTarget = e.target as HTMLElement;
      const contextPlayerId = eventTarget.getAttribute('data-context-playerid');
      if (contextPlayerId !== null) {
        console.log('Player ID', contextPlayerId);
        setShowContextMenu({
          show: true,
          contextType: 'PLAYER',
          contextData: contextPlayerId,
          xPos: e.clientX,
          yPos: e.clientY,
        });
      } else {
        setShowContextMenu({
          show: true,
          contextType: null,
          contextData: undefined,
          xPos: e.clientX,
          yPos: e.clientY,
        });
      }
      e.preventDefault();
    };

    window.addEventListener('contextmenu', contextMenuHandler);
    return () => {
      window.removeEventListener('contextmenu', contextMenuHandler);
    };
  }, []);

  const closeContextMenu = useCallback(() => {
    setShowContextMenu((contextMenuData) => ({
      ...contextMenuData,
      show: false,
    }));
  }, []);

  // render
  return (
    <>
      <div className="echo-list">
        <Virtuoso
          alignToBottom={true}
          className="echo-list-internal"
          followOutput={true}
          itemContent={(idx) => {
            return <ChatMessage message={currentMessageList[idx]} />;
          }}
          totalCount={currentMessageList.length}
          initialTopMostItemIndex={currentMessageList.length - 1}
        />
      </div>
      <ContextMenu
        show={showContextMenu.show}
        contextType={showContextMenu.contextType}
        contextData={showContextMenu.contextData}
        xPos={showContextMenu.xPos}
        yPos={showContextMenu.yPos}
        onClose={closeContextMenu}
      />
    </>
  );
};
export default App;
