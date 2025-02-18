import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiShip, GameCell } from '../../data/api/player-gameMap.interface';

@Component({
  selector: 'app-enemy-game-field',
  imports: [CommonModule],
  templateUrl: './enemy-game-field.component.html',
  styleUrl: './enemy-game-field.component.less'
})

export class EnemyGameFieldComponent implements OnInit {

  @Input() size: number = 10; 
  grid: any[][] = []; 

  isGameStarted: boolean = false;

  @Output() cellClicked = new EventEmitter<{row: number, col: number}>(); 

  private shipToPosition: Map<ApiShip, GameCell[]> = new Map(); 

  markAsHitted(cell: GameCell): void{
    this.grid[cell.Y][cell.X].isHit = true;
  }

  markAsMissed(cell: GameCell): void{
    this.grid[cell.Y][cell.X].isMiss = true;
  }

  isContainsShip(shipId: string): boolean{

    let found = false;

    this.shipToPosition.forEach((value, key) => {
      if (key.Id === shipId) {
        found = true;
      }
    });

    return found;
  }

  getShipPosition(shipId: string): GameCell[]{
    const positions: GameCell[] = [];

    this.shipToPosition.forEach((cells, ship) => {
      if (ship.Id === shipId) {
        positions.push(...cells);
      }
    });

    return positions; 
  }

  addShip(ship: ApiShip): void{
    if (this.shipToPosition.has(ship)) {
      return; 
    }
    this.shipToPosition.set(ship, []);
  }

  addCellToShip(shipId: string, cell: GameCell): void{

    for (const [key, cells] of this.shipToPosition.entries()) {
      if (key.Id === shipId) {
        cells.push(cell);
        return; 
      }
    }
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

  ngOnInit(): void {
    this.initializeGrid();
  }

  private initializeGrid(): void {
    this.grid = [];
    for (let i = 0; i < this.size; i++) {
      this.grid[i] = [];
      for (let j = 0; j < this.size; j++) {
        
        this.grid[i][j] = {
          isShip: false,
          isHit: false,
          isMiss: false,
          isDeadBody: false
        };
      }
    }
  }

  getLetter(rowIndex: number): string {
    return String.fromCharCode(65 + rowIndex); // 65 - это код 'A'
  }

  onCellClick(row: number, col: number): void {
    this.cellClicked.emit({row: row, col: col})
  }
}
