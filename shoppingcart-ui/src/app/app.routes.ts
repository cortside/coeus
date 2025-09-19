import { Routes } from '@angular/router';
import { cartRoutes, itemRoutes } from './features';

export const routes: Routes = [
  // default route
  { path: '', pathMatch: 'full', redirectTo: 'items' },

  // features
  ...itemRoutes,
  ...cartRoutes,

  // unrecognized
  { path: '**', redirectTo: '' },
];
