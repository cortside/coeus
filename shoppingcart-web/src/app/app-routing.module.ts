import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { catalogLazyRoutes } from './catalog/catalog.routes';
import { checkoutLazyLoadingRoutes } from './checkout/checkout-routing.module';
import { orderLazyLoadingRoutes } from './order';

const routes: Routes = [
    { path: '', redirectTo: '/catalog', pathMatch: 'full' },
    ...orderLazyLoadingRoutes,
    ...checkoutLazyLoadingRoutes,
    ...catalogLazyRoutes,
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
