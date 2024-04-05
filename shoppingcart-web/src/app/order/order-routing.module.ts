import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { requireAuthentication, requireAuthorization } from '@muziehdesign/core';
import { SHOPPING_CART_PERMISSIONS } from '../core/permissions';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { OrderListComponent } from './order-list/order-list.component';

export const orderLazyLoadingRoutes: Routes = [
    {
        path: 'orders',
        loadChildren: () => import('./order.module').then((m) => m.OrderModule),
        canActivate: [requireAuthentication, requireAuthorization(SHOPPING_CART_PERMISSIONS.getOrders)],
    },
];

const routes: Routes = [
    { path: '', component: OrderListComponent },
    { path: ':orderId', component: OrderDetailsComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class OrderRoutingModule {}
