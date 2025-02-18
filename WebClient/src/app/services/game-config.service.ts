import { Injectable } from '@angular/core';
import { GameConfig } from '../data/game-config.interface';

@Injectable({
  providedIn: 'root'
})
export class GameConfigService {

  private gameConfig: GameConfig | null = null;

  setGameConfig(config: GameConfig): void {
    this.gameConfig = config;
  }

  getGameConfig(): GameConfig | null {
    return this.gameConfig;
  }
}
