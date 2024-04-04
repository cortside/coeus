import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Route, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { ItemService } from '../core/item.service';
import { ItemDetailComponent } from './item-detail/item-detail.component';
import { ItemListComponent } from './item-list/item-list.component';
import { ItemComponent } from './item.component';

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
        path: '',
        providers: [ItemService],
        component: ItemComponent,
        children: [
            { path: '', component: ItemListComponent },
            { path: ':sku', component: ItemDetailComponent, canActivate: [canActivateGuard] },
        ],
    },
] satisfies Route[];
