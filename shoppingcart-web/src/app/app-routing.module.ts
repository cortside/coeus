import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { checkoutLazyLoadingRoutes } from './checkout/checkout-routing.module';
import { orderLazyLoadingRoutes } from './order';
import { customerLazyLoadingRoutes } from './customer/customer-routing.module';

const routes: Routes = [
    { path: '', redirectTo: '/catalog', pathMatch: 'full' },
    ...orderLazyLoadingRoutes,
    ...checkoutLazyLoadingRoutes,
    ...customerLazyLoadingRoutes
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
