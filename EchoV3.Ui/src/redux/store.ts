import { configureStore } from '@reduxjs/toolkit';
import settingsReducer from './reducers/settingsReducer';

const store = configureStore({
  reducer: {
    settings: settingsReducer,
  },
});
export default store;

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
