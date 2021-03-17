import {createSlice} from '@reduxjs/toolkit'

const counterSlice = createSlice({
  name: "counter",
  initialState: 0,
  reducers: {
    increment: state => {console.log(state); return state + 1},
    decrement: state => state - 1
  }
});


export default counterSlice;

const { increment, decrement } = counterSlice.actions;


export const fetchIncrement = (newValue) => async dispatch => {
  console.log(newValue)
  try {
    dispatch(increment());
  } catch (err) {
    //dispatch(getCommentsFailure(err))
  }
};

export const fetchDecrement = (newValue) => async dispatch => {
  console.log(newValue)

  try {
    dispatch(decrement());
  } catch (err) {
    //dispatch(getCommentsFailure(err))
  }
};
