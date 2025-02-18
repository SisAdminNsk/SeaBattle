import { ApiShip, GameCell } from "./player-gameMap.interface";

export interface HitGameMapResponse{
    Success: boolean;
    ErrorMessage: string | null;
    HittedCell: GameCell
    HitStatus: number;
    HittedShip: ApiShip | null;
}

export interface PlayerHitResponse{
    Success: boolean;
    ErrorMessage: string | null;
    PlayerTurnId: string;
    HitGameMapResponse: HitGameMapResponse;
}