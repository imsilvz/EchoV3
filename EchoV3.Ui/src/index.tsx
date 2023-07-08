// react
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';

// redux
import store from './redux/store';

// local
import { MockWebview2IfUndefined } from './mocks/WebView2';
import App from './components/App';
import './index.scss';

// for dev outside webview2 env
MockWebview2IfUndefined();

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode>,
);
