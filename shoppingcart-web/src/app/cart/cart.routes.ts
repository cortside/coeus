import { Route } from '@angular/router';

export const cartRoutes: Route[] = [
    {
        path: 'cart',
        loadComponent: () => import('./cart.component').then((x) => x.CartComponent),
    },
];
