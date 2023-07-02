// react
import React, { useEffect, useMemo, useState } from 'react';
import { Virtuoso } from 'react-virtuoso';

// redux
import { useAppSelector } from '../redux/hooks';
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
import ContextMenu from './ContextMenu/ContextMenu';

const App = () => {
  const chatSettings = useAppSelector(selectChatSettings);
  const listenerMode = useAppSelector(selectListenerMode);
  const [currentTargetId, setCurrentTargetId] = useState<number>(-1);
  const [messageList, setMessageList] = useState<ipc.ChatPayload[]>([]);
  const [showContextMenu, setShowContextMenu] = useState<{
    show: boolean;
    xPos: number;
    yPos: number;
  }>({
    show: false,
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
      console.log(arg.data);
      switch (payload.echoType) {
        case 'Chat':
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
      setShowContextMenu({
        show: true,
        xPos: e.clientX,
        yPos: e.clientY,
      });
      e.preventDefault();
    };

    window.addEventListener('contextmenu', contextMenuHandler);
    return () => {
      window.removeEventListener('contextmenu', contextMenuHandler);
    };
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
      {showContextMenu.show && (
        <ContextMenu
          xPos={showContextMenu.xPos}
          yPos={showContextMenu.yPos}
          onClose={() =>
            setShowContextMenu((contextMenuData) => ({
              ...contextMenuData,
              show: false,
            }))
          }
        />
      )}
    </>
  );
};
export default App;
