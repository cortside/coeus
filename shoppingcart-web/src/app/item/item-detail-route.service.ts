import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ItemService } from '../core/item.service';
import { RouteContextData } from '../core/route-context-data';

@Injectable()
export class ItemDetailRouteService {
    constructor(
        private router: Router,
        private context: RouteContextData,
        private service: ItemService
    ) { }

    async canActivate(route: ActivatedRouteSnapshot): Promise<boolean> {
        const item = await firstValueFrom(this.service.getItem(route.params['sku']));
        if (!item) {
            // redirect to not found
            this.router.navigate(['/notfound'], { skipLocationChange: true });
            return false;
        }

        this.context.set('item', item);
        return true;
    }

    resolveItem() {
        return this.context.get('item');
    }

    resolve() {
        const item = this.context.get('item');
        const authorizations = this.service.getItemRelatedAuthorizations();
        return { item, authorizations };
    }
}
