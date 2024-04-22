import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { firstValueFrom, of } from 'rxjs';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { ItemService } from '../core/item.service';
import { RouteContextData } from '../core/route-context-data';

@Injectable()
export class ItemDetailRouteService {
    constructor(
        private router: Router,
        private context: RouteContextData,
        private service: ItemService
    ) { }

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const item = await firstValueFrom(this.service.getItem(route.params['sku']));
        if (!item) {
            // redirect to not found
            this.router.navigate(['/notfound'], { skipLocationChange: true });
            return false;
        }

        this.context.set('item', item);
        return true;
    }

    resolveItem(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.context.get('item');
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const item = this.context.get('item');
        const authorizations = this.service.getItemRelatedAuthorizations();
        return { item, authorizations };
    }
}
