// redux
import store from '../../../redux/store';

// types
import * as ipc from '../../../types/ipc';

// local
import { JobColor } from '../../../constants';

let LastUserId = 0;
const UserIdMap = new Map<number, { color: string }>();
export const GetNameColor = (messageData: ipc.ChatPayload) => {
  const state = store.getState();
  const strategy =
    ColorStrategies[state.settings.nameColorMode as keyof typeof ColorStrategies];
  const senderData = store.getState().actors.playerDict[messageData.senderId];
  if (senderData?.playerColor) return senderData.playerColor;
  if (strategy) return strategy(messageData);
  return undefined;
};

const ColorStrategies = {
  CUSTOM: () => {
    return undefined;
  },
  RANDOM: (messageData: ipc.ChatPayload) => {
    const actorData = UserIdMap.get(messageData.senderId);

    let actorColor;
    if (actorData) {
      actorColor = actorData.color;
    } else {
      const lastUser = UserIdMap.get(LastUserId);
      actorColor = JobColor[Math.floor(Math.random() * JobColor.length)];
      while (actorColor === lastUser?.color) {
        actorColor = JobColor[Math.floor(Math.random() * JobColor.length)];
      }
      UserIdMap.set(messageData.senderId, {
        color: actorColor,
      });
      LastUserId = messageData.senderId;
    }
    return actorColor;
  },
};
