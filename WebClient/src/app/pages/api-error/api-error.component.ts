import { Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-api-error',
  imports: [CommonModule],
  templateUrl: './api-error.component.html',
  styleUrl: './api-error.component.less'
})

export class ApiErrorComponent implements OnInit {

  constructor(private router: Router) {}

  errorMessage: string = "Неизвестная ошибка";

  ngOnInit(): void {

    const state = history.state;

    if (state && state.error) {
      this.errorMessage = state.error;
    }
  }

  goToHomePage() {
    this.router.navigate(['/']);
  }
}
