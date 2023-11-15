import { Route } from '@angular/router';

export const catalogLazyRoutes: Route[] = [
    {
        path: 'catalog',
        loadComponent: () => import('./catalog.component').then((x) => x.CatalogComponent),
        children: [
            { path: '', redirectTo: 'items', pathMatch: 'full' },
            { path: 'items', loadComponent: () => import('./item-list/item-list.component').then((x) => x.ItemListComponent) },
            { path: 'items/:id', loadComponent: () => import('./item/item.component').then((x) => x.ItemComponent) },
        ],
    },
];
