import { Routes } from '@angular/router';

export const itemRoutes: Routes = [
  { path: 'items', loadComponent: () => import('./item-list/item-list').then((x) => x.ItemList) },
];
