import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ValidateGameMapResponse } from '../data/api/validate-gameMap-response.interface';
import { PlayerGameMap } from '../data/api/player-gameMap.interface';
import { ApiAddressService } from './api-address.service';

@Injectable({
  providedIn: 'root'
})

export class GameMapValidatorService {

  constructor(private http: HttpClient, private apiAddressService: ApiAddressService) { }

  validateGameMap(validateGameMapRequest: PlayerGameMap): Observable<ValidateGameMapResponse> {
  
    let baseApiUrl = this.apiAddressService.getBaseApiAddress();

    const url = `http://${baseApiUrl}/Game/ValidateGameMap`;
  
    return this.http.post<ValidateGameMapResponse>(url,validateGameMapRequest);
  }
}
