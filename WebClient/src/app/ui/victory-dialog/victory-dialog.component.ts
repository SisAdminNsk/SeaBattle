import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-victory-dialog',
  imports: [MatDialogModule, CommonModule],
  templateUrl: './victory-dialog.component.html',
  styleUrl: './victory-dialog.component.less'
})

export class VictoryDialogComponent {
  status: string = '';
  statusMessage: string = '';
  message: string = '';

  constructor(
    private dialogRef: MatDialogRef<VictoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any
  ) {
    this.message = data.message;
    this.statusMessage = data.statusMessage;
    this.status = data.status;
  }

  onClose(): void {
    this.dialogRef.close();
  }

  // Используем геттер вместо метода
  get statusClass(): string {
    switch (this.status) {
      case 'win':
        return 'win';
      case 'defeat':
        return 'defeat';
      case 'draw':
        return 'draw';
      default:
        return '';
    }
  }
}
