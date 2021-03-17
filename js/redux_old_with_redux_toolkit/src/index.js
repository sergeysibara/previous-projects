import React from 'react'
import { render } from 'react-dom'
// import { createStore } from 'redux'
import { Provider } from 'react-redux'
import App from './components/App'
import rootReducer from './reducers'

import {configureStore, Action} from '@reduxjs/toolkit'
import CounterBlock from './redux-toolkit/counterBlock';
import counterSlice from './redux-toolkit/counterSlice';
import todos from './reducers/todos';
import visibilityFilter from './reducers/visibilityFilter';

// const store = createStore(rootReducer)

const store = configureStore({
  reducer: {
    todos,
    visibilityFilter,
    counter: counterSlice.reducer,
  }
});

// // мой код, чтобы диспатч из компонента не перетаскиать. не работает
// console.log(store);
// export { store };

// if (process.env.NODE_ENV === 'development' && module.hot) {
//   module.hot.accept('./rootReducer', () => {
//     const newRootReducer = require('./rootReducer').default
//     store.replaceReducer(newRootReducer)
//   })
// }

render(
  <Provider store={store}>
    <App />
    <CounterBlock />
  </Provider>,
  document.getElementById('root')
);
