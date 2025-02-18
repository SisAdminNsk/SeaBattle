import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter,  OnInit } from '@angular/core';
import { Ship } from '../../data/ship.interface';
import { v4 as uuidv4 } from 'uuid';
import { GameCell } from '../../data/api/player-gameMap.interface';

@Component({
  selector: 'app-game-fieild',
  imports: [CommonModule],
  templateUrl: './game-fieild.component.html',
  styleUrl: './game-fieild.component.less',
})

export class GameFieildComponent implements OnInit {

  @Input() size: number = 10; 
  @Input() ships: Ship[] = []; 
  totalShips: number = 10;

  isGameStarted: boolean = false;

  grid: any[][] = []; 

  currentShipSize: number = 0;
  isCurrentShipHorizontal: boolean = false;

  @Output() shipPlaced = new EventEmitter<{shipSize: number}>(); 
  @Output() allShipPlaced = new EventEmitter();

  private getShipPosition(shipId: string): GameCell[] {

    const ship = this.ships.find(s => s.id === shipId);
    if (!ship) {
        return [];
    }

    const shipCells: GameCell[] = [];
    const { row, col, size, isHorizontal } = ship;

    for (let i = 0; i < size; i++) {
        const x = isHorizontal ? col + i : col; 
        const y = isHorizontal ? row : row + i; 

        const cell: GameCell = {
            X: x,
            Y: y,
            Hitted: false 
        };

        shipCells.push(cell); 
    }

    return shipCells; 
}

fillDeadzoneAroundKilledShip(shipId: string): void {

  const shipCells = this.getShipPosition(shipId);
      
  shipCells.forEach(cell => {
      const { X, Y } = cell; 

      this.grid[Y][X].isDeadBody = true;

      const neighbors = [
          { x: X - 1, y: Y - 1 }, // верхний левый
          { x: X - 1, y: Y },     // верхний
          { x: X - 1, y: Y + 1 }, // верхний правый
          { x: X, y: Y - 1 },     // левый
          { x: X, y: Y + 1 },     // правый
          { x: X + 1, y: Y - 1 }, // нижний левый
          { x: X + 1, y: Y },     // нижний
          { x: X + 1, y: Y + 1 }  // нижний правый
      ];

      neighbors.forEach(neighbor => {
          const { x, y } = neighbor;
          if (x >= 0 && x < this.size && y >= 0 && y < this.size && !this.grid[y][x].isDeadBody) {
              this.grid[y][x].isMiss = true; 
          }
      });
    });
  }

  markAsHitted(cell: GameCell): void{
    this.grid[cell.Y][cell.X].isHit = true;
  }

  markAsMissed(cell: GameCell): void{
    this.grid[cell.Y][cell.X].isMiss = true;
  }


  ngOnInit(): void {
    this.initializeGrid();
  }

  private initializeGrid(): void {
    this.grid = [];
    for (let i = 0; i < this.size; i++) {
      this.grid[i] = [];
      for (let j = 0; j < this.size; j++) {
        this.grid[i][j] = {
          isShip: this.ships.some(
            (ship) =>
              ship.isHorizontal
                ? ship.row === i && ship.col <= j && ship.col + ship.size > j
                : ship.col === j && ship.row <= i && ship.row + ship.size > i
          ), 
          isHit: false,
          isMiss: false,
          isHighlighted: false, 
          isDeadBody: false
        };
      }
    }
  }

  onDragEnd(): void {
    this.clearHighlight(); 
  }

  onCellClick(row: number, col: number): void {

    if (this.grid[row][col].isShip) {
      this.grid[row][col].isHit = true; 
    } else {
      this.grid[row][col].isMiss = true; 
    }
  }

  onShipPicked(size: number, isHorizontal: boolean ) {
     this.isCurrentShipHorizontal = isHorizontal;
     this.currentShipSize = size
  }

  onDragOver(event: DragEvent, row: number, col: number): void {
    event.preventDefault(); 
    this.clearHighlight();

    if (this.canPlaceShip(row, col, this.currentShipSize, this.isCurrentShipHorizontal)) {
      this.highlightCells(row, col, this.currentShipSize, this.isCurrentShipHorizontal);
    }
  }

  private highlightCells(row: number, col: number, size: number, isHorizontal: boolean): void {
    for (let i = 0; i < size; i++) {
      const newRow = isHorizontal ? row : row + i;
      const newCol = isHorizontal ? col + i : col;
      if (newRow < this.size && newCol < this.size) {
        this.grid[newRow][newCol].isHighlighted = true;
      }
    }
  }

  clearHighlight(): void {
    for (let i = 0; i < this.size; i++) {
      for (let j = 0; j < this.size; j++) {
        this.grid[i][j].isHighlighted = false;
      }
    }
  }

  onDrop(event: DragEvent, row: number, col: number): void {
    
    event.preventDefault();
    const data = event.dataTransfer!.getData('text/plain').split(',');
    const shipSize = +data[0];
    const isHorizontal = data[1] === 'true';

    if(this.canPlaceShip(row, col, shipSize, isHorizontal)) {

      this.placeShip(row, col, shipSize, isHorizontal);
      this.clearHighlight();

      this.shipPlaced.emit({shipSize: shipSize}) 
    } 
    else {
      alert('Невозможно разместить корабль здесь!');
    }
  }

  private canPlaceShip(row: number, col: number, size: number, isHorizontal: boolean): boolean {

    for (let i = 0; i < size; i++) {
      const newRow = isHorizontal ? row : row + i;
      const newCol = isHorizontal ? col + i : col;

      if (newRow >= this.size || newCol >= this.size || this.grid[newRow][newCol].isShip) {
        return false;
      }

      // Проверка соседних клеток
      for (let x = -1; x <= 1; x++) {
        for (let y = -1; y <= 1; y++) {
          const neighborRow = newRow + x;
          const neighborCol = newCol + y;
          if (
            neighborRow >= 0 &&
            neighborRow < this.size &&
            neighborCol >= 0 &&
            neighborCol < this.size &&
            this.grid[neighborRow][neighborCol].isShip
          ) {
            return false;
          }
        }
      }
    }

    return true;
  }

  private placeShip(row: number, col: number, size: number, isHorizontal: boolean): void {

    for (let i = 0; i < size; i++) {
      const newRow = isHorizontal ? row : row + i;
      const newCol = isHorizontal ? col + i : col;
      this.grid[newRow][newCol].isShip = true;
    }

    const id = uuidv4();

    this.ships.push({ row, col, size, isHorizontal, id });

    if(this.isAllShipsPlaced()){
      this.allShipPlaced.emit();
    }
  }

  private isAllShipsPlaced() : boolean{

    if(this.ships.length == this.totalShips){
      return true;
    }

    return false;
  }

  // Метод для получения буквы по индексу
  getLetter(rowIndex: number): string {
    return String.fromCharCode(65 + rowIndex); // 65 - это код 'A'
  }
}