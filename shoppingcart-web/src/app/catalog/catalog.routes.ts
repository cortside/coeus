import { Route } from '@angular/router';

export const catalogRoutes: Route[] = [{ path: 'catalog2', loadComponent: () => import('./catalog.component').then((x) => x.CatalogComponent) }];
