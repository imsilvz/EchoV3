// types
import * as ipc from '../../../types/ipc';

// local
import { JobColor } from '../../../constants';

let LastUserId = 0;
const UserIdMap = new Map<number, { color: string }>();
export const GetNameColor = (messageData: ipc.ChatPayload) => {
  return ColorStrategies['random'](messageData);
};

const ColorStrategies = {
  custom: (messageData: ipc.ChatPayload) => {
    return undefined;
  },
  random: (messageData: ipc.ChatPayload) => {
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
