import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { catalogLazyRoutes } from './catalog/catalog.routes';
import { checkoutLazyLoadingRoutes } from './checkout/checkout-routing.module';
import { orderLazyLoadingRoutes } from './order';
import { customerLazyLoadingRoutes } from './customer/customer-routing.module';

const routes: Routes = [
    { path: '', redirectTo: '/catalog/items', pathMatch: 'full' },
    ...orderLazyLoadingRoutes,
    ...checkoutLazyLoadingRoutes,
    ...catalogLazyRoutes,
    ...customerLazyLoadingRoutes
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
