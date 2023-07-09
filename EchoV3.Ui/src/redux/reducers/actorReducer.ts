import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';
import { PlayerActor } from '../../types/actors';

interface ActorReducerState {
  playerDict: {
    [key: number]: PlayerActor;
  };
}

const initialState: ActorReducerState = {
  playerDict: {},
};

interface PlayerActorUpdate {
  actorId: number;
  playerName?: string;
  playerJob?: string;
  playerColor?: string;
  ignored?: boolean;
}

const DefaultPlayer: PlayerActor = {
  actorId: 0,
  playerName: 'Unknown',
  playerJob: undefined,
  playerColor: undefined,
  ignored: false,
};

export const actorSlice = createSlice({
  name: 'actors',
  initialState,
  reducers: {
    addOrUpdatePlayer: (state, action: PayloadAction<PlayerActorUpdate>) => {
      const currPlayer = state.playerDict[action.payload.actorId];
      const updatedPlayer = {
        ...DefaultPlayer,
        ...currPlayer,
        ...action.payload,
      };
      if (currPlayer) {
        // do compare
        if (JSON.stringify(currPlayer) === JSON.stringify(updatedPlayer)) {
          // if there is no actual update, don't update!
          return;
        }
      }
      // perform the update action
      state.playerDict = {
        ...state.playerDict,
        [action.payload.actorId]: updatedPlayer,
      };
    },
  },
});
export const { addOrUpdatePlayer } = actorSlice.actions;
export const selectPlayerDict = (state: RootState) => state.actors.playerDict;
export default actorSlice.reducer;
