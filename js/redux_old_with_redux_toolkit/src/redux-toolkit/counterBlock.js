import React from 'react'
import { useStore, useDispatch, useSelector } from 'react-redux'
import { fetchDecrement, fetchIncrement } from './counterSlice';

const CounterBlock = ({ active, children, onClick }) => {
  //const store = useStore();
  const dispatch = useDispatch();
  //console.log(store.getState());
  const count = useSelector(state => state.counter);
  console.log(count);

  return (
    <div>
      <p>
        Clicked: <span id="value">{count}</span> times
        <button id="increment" onClick={() => {
          dispatch(fetchIncrement());
        }}>+</button>
        <button id="decrement" onClick={() => {
          dispatch(fetchDecrement());
        }}>-
        </button>
      </p>
    </div>
  );
};

export default CounterBlock;


