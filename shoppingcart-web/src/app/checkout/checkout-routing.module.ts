import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { requireAuthentication } from '@muziehdesign/core';
import { CheckoutComponent } from './checkout.component';

export const checkoutLazyLoadingRoutes: Routes = [
    {
        path: 'checkout',
        loadChildren: () => import('./checkout.module').then((m) => m.CheckoutModule),
        canActivate: [requireAuthentication],
    },
];

const routes: Routes = [{ path: '', component: CheckoutComponent }];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CheckoutRoutingModule {}
