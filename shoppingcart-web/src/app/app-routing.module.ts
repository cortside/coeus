import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { requireAuthentication } from '@muziehdesign/core';
import { cartRoutes } from './cart/cart.routes';
import { catalogLazyRoutes } from './catalog/catalog.routes';
import { checkoutLazyLoadingRoutes } from './checkout/checkout-routing.module';
import { orderLazyLoadingRoutes } from './order';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { ProfileComponent } from './profile/profile.component';

const routes: Routes = [
        { path: '', redirectTo: '/catalog/items', pathMatch: 'full' }, 
        { path: 'logout', redirectTo: '/', pathMatch: 'full' }, 
        ...orderLazyLoadingRoutes, 
        ...checkoutLazyLoadingRoutes, 
        ...catalogLazyRoutes,
        ...cartRoutes,
        { path: 'profile', component: ProfileComponent, canActivate: [requireAuthentication] }, // TODO
        { path: '**', component: PageNotFoundComponent }
    ];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
