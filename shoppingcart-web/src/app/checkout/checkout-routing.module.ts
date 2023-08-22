import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { canMatchIfAuthenticated } from '@muziehdesign/auth';
import { CheckoutComponent } from './checkout.component';

export const checkoutLazyLoadingRoutes: Routes = [
  {
      path: 'checkout',
      loadChildren: () => import('./checkout.module').then((m) => m.CheckoutModule),
      canMatch: [canMatchIfAuthenticated]
  },
];

const routes: Routes = [
  {path: '', component: CheckoutComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CheckoutRoutingModule { }
