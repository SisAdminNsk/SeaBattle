import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiAddressService {

  private baseApiUrl: string = '25.29.173.214:8084'; 

  constructor() { }

  getBaseApiAddress(): string {
    return this.baseApiUrl;
  }
}
