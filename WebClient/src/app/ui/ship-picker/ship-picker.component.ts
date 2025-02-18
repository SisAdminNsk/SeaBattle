import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameConfig, ShipConfig } from '../../data/game-config.interface';

@Component({
  selector: 'app-ship-picker',
  imports: [CommonModule],
  templateUrl: './ship-picker.component.html',
  styleUrl: './ship-picker.component.less',
})

export class ShipPickerComponent {

  ships: { size: number; count: number }[] = [];

  initializeShips(config : GameConfig) {

    if (config && config.ships) {
      this.ships = config.ships.map((ship: ShipConfig) => ({
        size: ship.size,
        count: ship.count
      }));
      
      console.log('Инициализированные корабли:', this.ships);
    }
  }

  @Output() shipPicked = new EventEmitter<{ size: number; isHorizontal: boolean }>(); 

  selectedShip: { size: number; count: number } | null = null;
  isHorizontal: boolean = true; // По умолчанию горизонтальное расположение

  selectShip(ship: { size: number; count: number }): void {
    this.selectedShip = ship;
  }

  toggleOrientation(): void {
    this.isHorizontal = !this.isHorizontal;
  }

  onDragStart(event: DragEvent, ship: { size: number; count: number }): void {

    if (ship.count === 0) {
      event.preventDefault();
      return;
    }

    if(this.selectedShip != null){
      this.shipPicked.emit({size: this.selectedShip.size, isHorizontal: this.isHorizontal});
    }

    event.dataTransfer!.setData('text/plain', `${ship.size},${this.isHorizontal}`);
  }

  decreaseShipCount(shipSize: number): void {
    const ship = this.ships.find(s => s.size === shipSize);
    if (ship && ship.count > 0) {
      ship.count--;
      if(ship.count == 0){
        this.selectedShip = null;
      }
    }
  }
}