export interface GameConfig {
    gameMapSize: number;
    ships: ShipConfig[];
}
  
export interface ShipConfig {
    name: string;
    size: number;
    count: number;
}