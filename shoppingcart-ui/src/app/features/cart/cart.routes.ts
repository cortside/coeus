import { Routes } from '@angular/router';

export const cartRoutes: Routes = [
  { path: 'cart', loadComponent: () => import('./cart/cart').then((x) => x.Cart) },
];
