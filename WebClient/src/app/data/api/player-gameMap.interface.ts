import { GameConfig } from "../game-config.interface";

export interface ApiShip{
    Id: string;
    Killed: boolean;
    Size: number;
}

export interface GameCell{
    X: number;
    Y: number;
    Hitted: boolean;
}

export interface ShipPositionOnMap{
    Ship: ApiShip;
    StartPosition: GameCell;
    ShipOrientation: number;
}

export interface PlayerGameMap{
    GameModeConfiguration: GameConfig;
    ShipPositions: ShipPositionOnMap[];
}