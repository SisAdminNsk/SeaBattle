import { Injectable } from '@angular/core';
import { ApiShip, GameCell, PlayerGameMap, ShipPositionOnMap } from '../data/api/player-gameMap.interface';
import { GameConfig } from '../data/game-config.interface';
import { Ship } from '../data/ship.interface';

@Injectable({
  providedIn: 'root'
})
export class ApiConverterService {

  constructor() { }

  ConvertToPlayerGameMapRequest(config: GameConfig, shipsOnGameField: Ship[]): PlayerGameMap{

    const playerGameMap: PlayerGameMap = {
      GameModeConfiguration: config, 
      ShipPositions: this.ConvertToShipsPositionOnMap(shipsOnGameField), 
    };

    return playerGameMap;
  }

  private ConvertToShipsPositionOnMap(shipsOnGameField: Ship[]) : ShipPositionOnMap[]{

    const shipPositions: ShipPositionOnMap[] = [];

    for(const ship of shipsOnGameField){
    
      const apiShip: ApiShip = {
        Id: ship.id,
        Killed: false,
        Size: ship.size
      }

      const startPosition: GameCell = {
        X: ship.col,
        Y: ship.row,
        Hitted: false
      };

      let shipOrientation: number = 0;

      if(ship.isHorizontal){
          shipOrientation = 1;
      }

      const shipPosition: ShipPositionOnMap = {
        Ship: apiShip,
        StartPosition: startPosition,
        ShipOrientation: shipOrientation
      };

      shipPositions.push(shipPosition);
    }

    return shipPositions;
  }
}
