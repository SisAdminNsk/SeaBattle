import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GameConfig } from '../data/game-config.interface';
import { Observable } from 'rxjs';
import { ApiAddressService } from './api-address.service';

@Injectable({
  providedIn: 'root'
})

export class ConfigService {

  constructor(private http: HttpClient, private apiAddressService: ApiAddressService) { }

  getGameConfig(): Observable<GameConfig[]> {

    let baseApiUrl = this.apiAddressService.getBaseApiAddress(); 

    const url = `http://${baseApiUrl}/Game/GetAllGameConfigs`;

    return this.http.get<GameConfig[]>(url);
  }
}
