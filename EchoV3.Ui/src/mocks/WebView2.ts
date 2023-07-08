// types
import { MessageTypeSettings } from '../components/ChatMessage/ChatMessage';
import type {
  WebViewEvent,
  WebViewEventListener,
  WebViewEventListenerObject,
} from '../types/global.js';
import * as ipc from '../types/ipc.js';

const CallbackMap = new Map<
  string,
  WebViewEventListener | WebViewEventListenerObject
>();
export const MockWebview2IfUndefined = () => {
  if (window.chrome === undefined) {
    // aaaa
    document.body.classList.add('mocked');
    window.chrome = {
      webview: {
        addEventListener: function (
          type: string,
          listener: WebViewEventListener | WebViewEventListenerObject,
          options?: boolean | AddEventListenerOptions | undefined,
        ): void {
          //throw new Error('Function not implemented.');
          console.log(type, listener);
          CallbackMap.set(type, listener);
        },
        postMessage: function (message: string): void {
          throw new Error('Function not implemented.');
        },
        removeEventListener: function (
          type: string,
          listener: WebViewEventListener | WebViewEventListenerObject,
          options?: boolean | EventListenerOptions | undefined,
        ) {
          throw new Error('Function not implemented.');
        },
      },
    };

    setInterval(() => {
      const ActorData = [
        { name: 'Joe Test', actorId: 1 },
        { name: 'John Doe', actorId: 2 },
        { name: 'Jane Doe', actorId: 3 },
        { name: 'Mary Sue', actorId: 4 },
        { name: 'Gary Stu', actorId: 5 },
      ];
      const messageData = [
        'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Elementum sagittis vitae et leo duis ut diam.',
        'Cras adipiscing "enim" eu turpis.',
        'Eu sem integer vitae justo eget. Pulvinar mattis nunc sed blandit libero volutpat sed. In mollis nunc sed id semper risus. Ipsum suspendisse ultrices gravida dictum fusce ut. Massa vitae tortor condimentum lacinia quis. Quam elementum pulvinar etiam non quam.',
        'Commodo quis imperdiet massa tincidunt nunc.',
        'Massa "tincidunt nunc pulvinar sapien" et ligula ullamcorper "malesuada proin". ((Purus in mollis nunc sed id semper risus in hendrerit)).',
        'Enim diam vulputate ut pharetra sit amet aliquam id diam.',
        '((Lorem ipsum dolor sit amet, consectetur adipiscing elit, "sed do" eiusmod tempor incididunt ut labore et dolore magna aliqua.)) Adipiscing bibendum est ultricies integer quis. Nisl nisi scelerisque eu ultrices vitae auctor eu augue.',
        'Et netus et malesuada fames ac turpis egestas. Sapien eget mi proin sed libero enim sed faucibus. Diam in arcu cursus euismod quis viverra nibh cras.',
        '"Ut sem nulla pharetra diam sit amet nisl suscipit adipiscing. Ullamcorper sit amet risus nullam."',
        'Semper auctor neque vitae tempus quam pellentesque nec.',
        '((Cursus risus at ultrices mi tempus imperdiet nulla.))',
      ];

      const msgTypes = Array.from(Object.keys(MessageTypeSettings)).slice(1);
      const msgActor = ActorData[Math.floor(Math.random() * ActorData.length)];
      const ipcData: ipc.ChatPayload = {
        echoType: 'Chat',
        timestamp: new Date().toISOString(),
        sourceActorId: 1,
        destinationActorId: 1,
        messageType: msgTypes[Math.floor(Math.random() * msgTypes.length)],
        senderId: msgActor.actorId,
        senderName: msgActor.name,
        senderActor: undefined,
        message: messageData[Math.floor(Math.random() * messageData.length)],
      };
      MockEvent('message', {
        data: ipcData,
      });
    }, 100);
  }
};

export const MockEvent = (event: string, payload: unknown) => {
  const callback = CallbackMap.get(event);
  if (callback) {
    if (typeof callback === 'function') {
      callback(payload as WebViewEvent);
    } else {
      callback.handleEvent(payload as Event);
    }
  }
};
