import { ApiShip } from "./player-gameMap.interface";
import { HitGameMapResponse } from "./player-hit-response.interface";

export interface SessionFinished{
    SessionId: string;
    WinnerPlayerId: string;
    IsDraw: boolean;
    Message: string;
    HitGameMapResponse: HitGameMapResponse | null;
}