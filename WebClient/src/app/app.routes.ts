import { Routes } from '@angular/router';
import { ApiErrorComponent } from './pages/api-error/api-error.component';
import { GameComponent } from './pages/game/game.component';

export const routes: Routes = [
    {path: 'api-error', component: ApiErrorComponent},
    {path: '', component: GameComponent}
];

