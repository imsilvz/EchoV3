import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';

const initialState = {
  chatSettings: {
    System: true,
    Say: true,
    Emote: true,
    CustomEmote: true,
    Shout: true,
    Yell: true,
  },
  ignoreMode: true,
  listenerMode: false,
  nameColorMode: 'RANDOM',
};

export const settingsSlice = createSlice({
  name: 'settings',
  // `createSlice` will infer the state type from the `initialState` argument
  initialState,
  reducers: {
    setChatSettings: (
      state,
      action: PayloadAction<{
        System?: boolean;
        Say?: boolean;
        Emote?: boolean;
        Shout?: boolean;
        Yell?: boolean;
      }>,
    ) => {
      const transformer: {
        System?: boolean;
        Say?: boolean;
        Emote?: boolean;
        CustomEmote?: boolean;
        Shout?: boolean;
        Yell?: boolean;
      } = {
        ...action.payload,
      };
      if (transformer.Emote !== undefined) {
        // sync these, but don't overwrite if not specified
        transformer.CustomEmote = transformer.Emote;
      }

      state.chatSettings = {
        ...state.chatSettings,
        ...transformer,
      };
    },
    setIgnoreMode: (state, action: PayloadAction<boolean>) => {
      state.ignoreMode = action.payload;
    },
    setListenerMode: (state, action: PayloadAction<boolean>) => {
      state.listenerMode = action.payload;
    },
    setNameColorMode: (state, action: PayloadAction<string>) => {
      state.nameColorMode = action.payload;
    },
  },
});
export const { setChatSettings, setIgnoreMode, setListenerMode, setNameColorMode } =
  settingsSlice.actions;
export const selectChatSettings = (state: RootState) => state.settings.chatSettings;
export const selectIgnoreMode = (state: RootState) => state.settings.ignoreMode;
export const selectListenerMode = (state: RootState) => state.settings.listenerMode;
export const selectNameColorMode = (state: RootState) =>
  state.settings.nameColorMode;
export default settingsSlice.reducer;
