import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiAddressService {

  private baseApiUrl: string = 'localhost:8084';

  constructor() { }

  getBaseApiAddress(): string {
    return this.baseApiUrl;
  }
}
