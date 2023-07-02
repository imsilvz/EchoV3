// react
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';

// redux
import store from './redux/store';

// local
import App from './components/App';
import './index.scss';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode>,
);
