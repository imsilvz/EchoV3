export interface Actor {
  actorId: number;
}

export interface PlayerActor extends Actor {
  playerName: string;
  playerJob?: string;
  playerColor?: string;
}
