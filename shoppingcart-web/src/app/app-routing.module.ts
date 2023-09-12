import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StyleGuideComponent } from 'muzieh-design';
import { catalogLazyRoutes } from './catalog/catalog.routes';
import { checkoutLazyLoadingRoutes } from './checkout/checkout-routing.module';
import { orderLazyLoadingRoutes } from './order';

const routes: Routes = [{ path: '', redirectTo: '/catalog/items', pathMatch: 'full' }, { path: 'logout', redirectTo: '/', pathMatch: 'full' }, { path: 'styleguide', component: StyleGuideComponent }, ...orderLazyLoadingRoutes, ...checkoutLazyLoadingRoutes, ...catalogLazyRoutes];

@NgModule({
    imports: [RouterModule.forRoot(routes), StyleGuideComponent],
    exports: [RouterModule],
})
export class AppRoutingModule {}
