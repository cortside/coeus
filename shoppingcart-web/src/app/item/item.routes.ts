import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Route, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { ItemService } from './item.service';

const canActivateGuard = (snapshot: ActivatedRouteSnapshot) => {
    const service = inject(ItemService);
    const router = inject(Router);
    return service.getItem(snapshot.params['sku']).pipe(
        map(() => true),
        catchError(() => {
            router.navigate(['/notfound'], { skipLocationChange: true });
            return of(false);
        })
    );
};

export const itemRoutes: Route[] = [
    {
        path: 'items',
        providers: [ItemService],
        children: [
            { path: '', loadComponent: () => import('./item-list/item-list.component').then((x) => x.ItemListComponent) },
            {
                path: ':sku',
                loadComponent: () => import('./item-detail/item-detail.component').then((x) => x.ItemDetailComponent),
                canActivate: [canActivateGuard],
            },
        ],
    },
];
