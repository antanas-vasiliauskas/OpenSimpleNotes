import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';

// Add either:
export {}; // This makes it a module

// OR export something meaningful like:
// export const root = ReactDOM.createRoot(
//   document.getElementById('root') as HTMLElement
// );

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);