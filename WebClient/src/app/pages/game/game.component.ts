import { Component, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { GameFieildComponent } from '../../ui/game-fieild/game-fieild.component';
import { ShipPickerComponent } from '../../ui/ship-picker/ship-picker.component';
import { CommonModule } from '@angular/common';
import { GameConfig } from '../../data/game-config.interface';
import { ConfigService } from '../../services/config.service';
import { ChangeDetectorRef } from '@angular/core';
import {MatProgressSpinnerModule } from '@angular/material/progress-spinner'; 
import { GameMapValidatorService } from '../../services/game-map-validator-service.service';
import { ApiConverterService } from '../../services/api-converter-service.service';
import { WebsocketService } from '../../services/websocket.service';
import { BasePlayerResponse } from '../../data/api/base-player-response.interface';
import { Subscription } from 'rxjs';
import { GameSessionStartedResponse } from '../../data/api/game-session-started-response.interface';
import { PlayerTurnChanged } from '../../data/api/player-turn-changed-response.interface';
import { PlayerWinResponse } from '../../data/api/player-win-response.interface';
import { MatDialog } from '@angular/material/dialog';
import { VictoryDialogComponent } from '../../ui/victory-dialog/victory-dialog.component';
import { SessionFinished } from '../../data/api/session-finished-response.interface';
import { EnemyGameFieldComponent } from '../../ui/enemy-game-field/enemy-game-field.component';
import { HitGameMapResponse, PlayerHitResponse } from '../../data/api/player-hit-response.interface';
import { GameCell } from '../../data/api/player-gameMap.interface';
import { HitRequest } from '../../data/api/player-hit-request.interface';
import { ApiAddressService } from '../../services/api-address.service';

@Component({
  selector: 'app-game',
  imports: [GameFieildComponent, EnemyGameFieldComponent, ShipPickerComponent, CommonModule, MatProgressSpinnerModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.less',
  providers: [
    {
      provide: WebsocketService,
      useFactory: () => new WebsocketService(),
    },
  ]
})

export class GameComponent implements AfterViewInit, OnDestroy {

  @ViewChild(GameFieildComponent) gameFieldComponent!: GameFieildComponent;
  @ViewChild(ShipPickerComponent) shipPickerComponent!: ShipPickerComponent;
  @ViewChild(EnemyGameFieldComponent) enemyFieldComponent! : EnemyGameFieldComponent;

  private forbiddenToHitCells: Set<GameCell> = new Set<GameCell>(); 

  private isCellForbiddenToHit(col: number, row: number): boolean{

    let isFound: boolean = false;

    this.forbiddenToHitCells.forEach(cell => {
        if (cell.X === col && cell.Y === row) {
          isFound = true;
        }
    });

    return isFound;
  }


  title = 'Морской бой онлайн';
  gameStatusMessage = 'Расставьте корабли на поле.'; 
  playerTurnMessage = '';

  gameConfig: GameConfig | null = null; 
  
  isStartGameButtonDisabled: boolean = false;

  isFetchingAccessToken: boolean = false;
  isFetchingConfigs: boolean = true; // должно получаться с самого начала рендернига компонента
  isWaitingToStartGame: boolean = false;
  isGameStarted: boolean = false;

  isWaitingServerResponse: boolean = false;

  isOpponentTurn: boolean = false;

  isWaitingOpponent: boolean = false;

  private subscriptions = new Subscription();
  private gameSessionData: GameSessionStartedResponse | null = null;

  constructor(
  private router: Router,
   private configService: ConfigService,
    private cdr: ChangeDetectorRef,
      private apiModelsConverter: ApiConverterService,
        private gameMapValidatorService: GameMapValidatorService,
          private wsService: WebsocketService,
            private dialog: MatDialog,
              private apiAddressSerivce: ApiAddressService) {}

  ngAfterViewInit(): void {

    this.configService.getGameConfig().subscribe({
      next: (config) => {
        console.log('Конфигурации загружены:', config);

        if (config.length > 0) {
          const firstGameConfig = config[0];  
          this.gameConfig = firstGameConfig;
          this.shipPickerComponent.initializeShips(firstGameConfig);
          this.gameFieldComponent.size = firstGameConfig.gameMapSize;
          this.gameFieldComponent.totalShips = this.calcTotalShipsCount(firstGameConfig);
          this.gameFieldComponent.ngOnInit();
          console.log('Первый элемент gameConfig сохранен в сервисе:', firstGameConfig);
        } else {
          console.warn('Массив конфигураций пуст.');
        }
      },
      error: (error) => {
        console.error('Ошибка при загрузке конфигураций:', error);
        this.router.navigate(['/api-error'], {
          state: { error: 'Не удалось загрузить конфигурации'}
        });
      },
      complete: () => {
        this.isFetchingConfigs = false; 
        this.cdr.detectChanges(); 
      }
    });
  }

  private calcTotalShipsCount(gameConfig: GameConfig) : number{

    let totalShips: number = 0;

    for (const ship of gameConfig.ships){
      for (let i = 0; i < ship.size; i++){
        totalShips++;
      }
    }

    return totalShips;
  }

  onShipPicked(ev: { size: number; isHorizontal: boolean; }) : void {
    if (this.gameFieldComponent) {
      this.gameFieldComponent.onShipPicked(ev.size, ev.isHorizontal);
    }
  }

  onShipPlaced(ev: {shipSize: number}) : void {
    if(this.shipPickerComponent){
      this.shipPickerComponent.decreaseShipCount(ev.shipSize);
    }
  }

  onAllShipPlaced() : void {
    console.log("Все корабли установлены");
    this.isWaitingToStartGame = true;
    this.changeGameStatusMessage('Начните игру');  
  }

  async startGame() : Promise<void> {

    const accessToken = await this.fetchAccessToken();

    if (accessToken) {
      this.connectToGame(accessToken);
      this.setupGameEventsHandlers();    
      this.isWaitingOpponent = true; 
      this.changeGameStatusMessage('Ожидание подключения противника...');  
    }
  }

  private changeGameStatusMessage(newGameStatusMessage: string) : void{
    this.gameStatusMessage = newGameStatusMessage;
  }

  private changePlayerTurnMessage(newPlayerTurnMessage: string) : void{
    this.playerTurnMessage = newPlayerTurnMessage;
  }

  private connectToGame(accessToken : string) : void{

    let baseApiUrl = this.apiAddressSerivce.getBaseApiAddress();

    //const url = `ws://localhost:8084/Game/StartGame?startGameAccessToken=${encodeURIComponent(accessToken)}`;
    const url = `ws://${baseApiUrl}/Game/StartGame?startGameAccessToken=${encodeURIComponent(accessToken)}`;

    this.wsService.connect(url);
  }

  private setupGameEventsHandlers() : void{
    this.subscriptions.add(
      this.wsService.messages$.subscribe({
        next: (response) => this.handleMessage(response),
        error: (err) => this.handleError(err)
      })
    );
  }

  private handleMessage(response: BasePlayerResponse): void {

    switch(response.MessageType){
      case 'SessionStarted':
        const sessionData = response.Response as GameSessionStartedResponse;
        this.handleSessionStarted(sessionData);
        break;

      case 'PlayerTurnChanged':
        const playerTurnChangedResponse = response.Response as PlayerTurnChanged;
        this.handlePlayerTurnChanged(playerTurnChangedResponse);
        break; 

      case 'PlayerWin':
        const playerWinResponse = response.Response as PlayerWinResponse;
        this.handleOpponentDisconnect(playerWinResponse);
        break;

      case 'SessionFinished':
        const sessionFinishedResponse = response.Response as SessionFinished
        this.handleSessionFinished(sessionFinishedResponse);
        break;

      case 'PlayerHit':
        const playerHitResponse = response.Response as PlayerHitResponse
        this.handlePlayerHitResponse(playerHitResponse);
        break;

      case 'OpponentHit':
        const opponentHitResponse = response.Response as PlayerHitResponse;
        this.handleOpponentHitResponse(opponentHitResponse);
        break;

      default:
        break; 
    }
  }

  private handleOpponentHitResponse(response: PlayerHitResponse): void{

    if(!response.Success){
      let errorMessage = 'Возникла ошибка при получении ответа с сервера. (OpponentHitError)'
      this.redirectToApiErrorPage(errorMessage);
    }

    if(this.gameSessionData?.YourId == response.PlayerTurnId){ // смена хода вынести в отдельные функции
      this.changePlayerTurnMessage('Сейчас ваш ход');
      this.isOpponentTurn = false;
    }
    else{
      this.changePlayerTurnMessage('Противник ходит');
      this.isOpponentTurn = true;
    }

    if(response.HitGameMapResponse != null){
      this.markHitOnPlayerMap(response.HitGameMapResponse);
    }
  }

  private handleSessionStarted(sessionData: GameSessionStartedResponse){
    
    console.log('Игровая сессия началась:', sessionData);
    this.gameSessionData = sessionData;
    this.isWaitingToStartGame = false;
    this.isGameStarted = true;
    this.enemyFieldComponent.isGameStarted = true;

    let yourId = this.gameSessionData.YourId;
    let playerTurnId = this.gameSessionData.PlayerTurnId;

    if(yourId == playerTurnId){
      this.changePlayerTurnMessage('Сейчас ваш ход');
      this.isOpponentTurn = false;
    }
    else{
      this.changePlayerTurnMessage('Противник ходит');
      this.isOpponentTurn = true;
    }

    this.changeGameStatusMessage('Игра началась');
  }

  private handlePlayerTurnChanged(data: PlayerTurnChanged){

    console.log('Ход игрока поменялся:', data);

    if(this.gameSessionData?.YourId == data.CurrentTurnPlayerId){
      this.changePlayerTurnMessage('Сейчас ваш ход');
      this.isOpponentTurn = false;
    }
    else{
      this.changePlayerTurnMessage('Противник ходит');
      this.isOpponentTurn = true;
    }
  }

  private handleError(error: any): void {
    console.error('WebSocket error:', error);
    this.redirectToApiErrorPage('Соединение с сервером потеряно');
  }

  private handleOpponentDisconnect(response: PlayerWinResponse): void{

    console.log('Противник отключился');

    this.showWinnerPlayerDialog('win','Победа', 'Противник отключился');
  }

  private handlePlayerHitResponse(response: PlayerHitResponse): void{

    this.isWaitingServerResponse = false;
    
    if(!response.Success){
      let errorMessage = 'Возникла ошибка при получении ответа с сервера.';
      this.redirectToApiErrorPage(errorMessage);
    }

    if(this.gameSessionData?.YourId == response.PlayerTurnId){
      this.changePlayerTurnMessage('Сейчас ваш ход');
      this.isOpponentTurn = false;
    }
    else{
      this.changePlayerTurnMessage('Противник ходит');
      this.isOpponentTurn = true;
    }

    if(!response.HitGameMapResponse.Success){
      let errorMessage = 'Возникла ошибка при получении ответа с сервера. (GameMapResponse)';
      this.redirectToApiErrorPage(errorMessage);
    }

    let hittedCell = response.HitGameMapResponse.HittedCell;

    this.forbiddenToHitCells.add(hittedCell); // по этой клетке больше нельзя стрелять 

    this.markHitOnOpponentMap(response.HitGameMapResponse); // помечаем результат выстрела на вражеской карте
  }

  private markHitOnPlayerMap(response: HitGameMapResponse): void{

    let hittedCell = response.HittedCell;

    if(response.HittedShip != null){

      let hittedShip = response.HittedShip;

      this.gameFieldComponent.markAsHitted(hittedCell); 
      
      if(hittedShip.Killed){
        this.gameFieldComponent.fillDeadzoneAroundKilledShip(hittedShip.Id);
      }
    }
    else{
      this.gameFieldComponent.markAsMissed(hittedCell);
    }
  }

  private markHitOnOpponentMap(response: HitGameMapResponse): void{

    let hittedCell = response.HittedCell;

    if(response.HittedShip != null){

      let hittedShip = response.HittedShip;

      if(!this.enemyFieldComponent.isContainsShip(hittedShip.Id)){
        this.enemyFieldComponent.addShip(hittedShip);
      }

      this.enemyFieldComponent.markAsHitted(hittedCell); 
      this.enemyFieldComponent.addCellToShip(hittedShip.Id, hittedCell);
      
      if(hittedShip.Killed){
        this.enemyFieldComponent.fillDeadzoneAroundKilledShip(hittedShip.Id);
      }
    }
    else{
      this.enemyFieldComponent.markAsMissed(hittedCell);
    }
  }

  private handleSessionFinished(response: SessionFinished): void{

    console.log('Сессия завершена');

    if(response.IsDraw){
      this.showWinnerPlayerDialog('draw', 'Ничья', 'Сессия завершилась');
      return;
    }

    if(this.gameSessionData?.YourId == response.WinnerPlayerId){
      this.showWinnerPlayerDialog('win', 'Победа', 'Нажмите в любое место');
      if(response.HitGameMapResponse != null ){ // пометить оставшийся убитый корабль 
        this.markHitOnOpponentMap(response.HitGameMapResponse);
      }  
    }
    else{
      this.showWinnerPlayerDialog('defeat','Поражение','Нажмите в любое место');
      if(response.HitGameMapResponse != null ){ // пометить оставшийся убитый корабль 
        this.markHitOnPlayerMap(response.HitGameMapResponse);
      }  
    }
  }

  private showWinnerPlayerDialog(status: string, statusMessage: string, message: string): void{

    const dialogRef = this.dialog.open(VictoryDialogComponent, {
      width: '300px',
      data: { status: status, statusMessage: statusMessage, message: message } 
    })

    dialogRef.afterClosed().subscribe(() => {
      window.location.reload();
    });
  }

  private async fetchAccessToken(): Promise<string | null> {

    this.isFetchingAccessToken = true;

    if (this.gameConfig == null) {
      console.error("Невозможно начать игру, конфигурация не инициализирована");
      return null;
    }

    const playerGameMap = this.apiModelsConverter.ConvertToPlayerGameMapRequest(this.gameConfig, this.gameFieldComponent.ships);

    try {

        const response = await this.gameMapValidatorService.validateGameMap(playerGameMap).toPromise();
        console.log("Запрос на валидацию завершен:", response);

        if (response?.success) {
          console.log("Access token успешно получен");
          return response.accessToken; 
        } 
        else {
          console.error("Валидация не прошла:", response?.errorMessage);
          let errorMessage = '';
          if(response?.errorMessage){
            errorMessage = response.errorMessage;
          }
          this.redirectToApiErrorPage(errorMessage);
          return null; 
        }
    } 
    catch (error) {
      console.error("Ошибка при валидации карты:", error);
      this.redirectToApiErrorPage('Не удалось получить токен для старта игры');
      return null;
    }
    finally{
      this.isFetchingAccessToken = false;
    }
  }

  private redirectToApiErrorPage(errorMessage: string) : void{

    this.router.navigate(['/api-error'], {
      state: { error: errorMessage}
    });
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
    if (this.wsService) {
      this.wsService.close();
    }
  }

  onEnemyFieldClicked(ev: {row: number; col: number}) : void{

    if(!this.isOpponentTurn && !this.isWaitingServerResponse){

      if(!this.isCellForbiddenToHit(ev.col, ev.row)){
    
        const gameCell: GameCell = {
          X: ev.col,
          Y: ev.row,
          Hitted: false
        };

        const hitRequest: HitRequest = {
          CellToHit: gameCell
        }

        this.isWaitingServerResponse = true;
        this.wsService.sendMessage('HitRequest', hitRequest);
      }
    }
  }
}