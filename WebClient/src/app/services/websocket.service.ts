import { Injectable } from '@angular/core';
import { Observable, Subject, throwError  } from 'rxjs';
import { BasePlayerResponse } from '../data/api/base-player-response.interface';
import { catchError, filter, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class WebsocketService {

  private socket!: WebSocket;
  private messageSubject = new Subject<BasePlayerResponse>();
  public messages$ = this.messageSubject.asObservable();


  constructor() { }

  public connect(url: string): void {
    this.socket = new WebSocket(url);

    this.socket.onmessage = (event) => {
      const response: BasePlayerResponse = JSON.parse(event.data);
      this.messageSubject.next(response);
    };

    this.socket.onerror = (error) => {
      this.messageSubject.error(error);
    };

    this.socket.onclose = (event) => {
      if (!event.wasClean) {
        this.messageSubject.error(`Connection closed: ${event.reason}`);
      }
    };
  }

  public sendMessage<T>(command: string, data: T): void {

    if (this.socket.readyState === WebSocket.OPEN) {
      const message = JSON.stringify({
        MessageType: command,
        Request: data
      });
      this.socket.send(message);
    } else {
      console.error('WebSocket is not connected.');
    }
  }

  public close(): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      this.socket.close();
    }
  }

  public isConnected(): boolean {
    return this.socket !== null && this.socket.readyState === WebSocket.OPEN;
  }

  public getMessageObservable<T>(messageType: string): Observable<T> {
    return this.messages$.pipe(
      filter(response => response.MessageType === messageType),
      map(response => response.Response as T),
      catchError(error => throwError(error))
    );
  }

}
