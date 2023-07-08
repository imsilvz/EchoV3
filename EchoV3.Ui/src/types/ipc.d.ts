export interface IpcPayload {
  echoType: string;
  timestamp: string;
  sourceActorId: number;
  destinationActorId: number;
}

export interface ChatPayload extends IpcPayload {
  messageType: string;
  senderId: number;
  senderName: string;
  senderActor: unknown;
  message: string;
}

export interface LocalTargetPayload extends IpcPayload {
  targetId: number;
}
