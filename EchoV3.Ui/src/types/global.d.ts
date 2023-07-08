interface WebViewEvent extends Event {
  data: unknown;
}

interface WebViewEventListener {
  (evt: WebViewEvent): void;
}

interface WebViewEventListenerObject {
  handleEvent(object: Event): void;
}

type WebViewEventListenerOrEventListenerObject =
  | WebViewEventListener
  | WebViewEventListenerObject;

declare global {
  interface Window {
    chrome: {
      webview: {
        addEventListener: (
          type: string,
          listener: WebViewEventListenerOrEventListenerObject,
          options?: boolean | AddEventListenerOptions,
        ) => void;
        postMessage: (message: string) => void;
        removeEventListener(
          type: string,
          listener: WebViewEventListenerOrEventListenerObject,
          options?: boolean | EventListenerOptions,
        );
      };
    };
  }
}

export {
  WebViewEvent,
  WebViewEventListener,
  WebViewEventListenerObject,
  WebViewEventListenerOrEventListenerObject,
};
