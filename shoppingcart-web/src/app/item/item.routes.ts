import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Route, Router, RouterStateSnapshot } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { ItemService } from '../core/item.service';
import { ItemDetailRouteService } from './item-detail-route.service';
import { ItemDetailComponent } from './item-detail/item-detail.component';
import { ItemListComponent } from './item-list/item-list.component';
import { ItemModelAssembler } from './item-model.assembler';
import { ItemFacade } from './item.facade';

const canActivateGuard = (route: ActivatedRouteSnapshot) => {
    const service = inject(ItemService);
    const router = inject(Router);
    return service.getItem(route.params['sku']).pipe(
        map(() => true),
        catchError(() => {
            router.navigate(['/notfound'], { skipLocationChange: true });
            return of(false);
        })
    );
};

const canActivateGuard2 = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const service = inject(ItemDetailRouteService);
    console.log('activating');
    return service.canActivate(route, state);
};

const itemResolver = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const service = inject(ItemDetailRouteService);
    return service.resolve(route, state);
};

const titleResolver = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    console.log('resolving title', route.data);
    return Promise.resolve('test title');
};

export const itemRoutes: Route[] = [
    {
        path: '',
        providers: [ItemService, ItemFacade, ItemModelAssembler, ItemDetailRouteService],
        children: [
            { path: '', component: ItemListComponent },
            {
                path: ':sku',
                component: ItemDetailComponent,
                canActivate: [canActivateGuard2],
                resolve: { data: itemResolver },
                title: titleResolver
            },
        ],
    },
] satisfies Route[];
