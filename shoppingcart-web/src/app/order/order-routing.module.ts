import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticatedGuard } from '../auth/authenticated.guard';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { OrderListComponent } from './order-list/order-list.component';

export const orderLazyLoadingRoutes: Routes = [
    {
        path: 'orders',
        loadChildren: () => import('./order.module').then((m) => m.OrderModule),
        canLoad: [AuthenticatedGuard],
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
