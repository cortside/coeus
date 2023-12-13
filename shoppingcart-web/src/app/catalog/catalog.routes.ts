import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Route, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { ItemService } from '../core/item.service';

const canActivateGuard = (snapshot:ActivatedRouteSnapshot) => {
    const service = inject(ItemService);
    const router = inject(Router);
    return service.getItem(snapshot.params['sku']).pipe(
        map(()=>true),
        catchError(()=> {
            router.navigate(['/notfound'], { skipLocationChange: true });
            return of(false)
        })
    );
}

export const catalogLazyRoutes: Route[] = [
    {
        path: 'catalog',
        loadComponent: () => import('./catalog.component').then((x) => x.CatalogComponent),
        children: [
            { path: '', redirectTo: 'items', pathMatch: 'full' },
            { path: 'items', loadComponent: () => import('./item-list/item-list.component').then((x) => x.ItemListComponent) },
            { 
                path: 'items/:sku', 
                loadComponent: () => import('./item/item.component').then((x) => x.ItemComponent),
                canActivate: [canActivateGuard]
            },
        ],
    },
];
