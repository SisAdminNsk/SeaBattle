import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { ConfigService } from '../services/config.service';
import { GameConfigService } from '../services/game-config.service';

@Injectable({
  providedIn: 'root',
})
export class GameConfigResolver implements Resolve<void> {
  constructor(
    private configService: ConfigService,
    private gameConfigService: GameConfigService
  ) {}

  resolve(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.configService.getGameConfig().subscribe({
        next: (config) => {
          if (config.length > 0) {
            this.gameConfigService.setGameConfig(config[0]);
            resolve(); 
          } else {
            reject('Массив конфигураций пуст.'); 
          }
        },
        error: (error) => {
          reject(error); 
        },
      });
    });
  }
}