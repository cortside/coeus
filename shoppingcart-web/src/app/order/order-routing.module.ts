import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { canMatchIfAuthenticated } from '@muziehdesign/auth';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { OrderListComponent } from './order-list/order-list.component';

export const orderLazyLoadingRoutes: Routes = [
    {
        path: 'orders',
        loadChildren: () => import('./order.module').then((m) => m.OrderModule),
        canMatch: [canMatchIfAuthenticated],
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
