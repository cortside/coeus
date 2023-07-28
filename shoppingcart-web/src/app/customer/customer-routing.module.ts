import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticatedGuard } from '@muziehdesign/auth';

import { CustomerListComponent } from './customer-list/customer-list.component';

export const customerLazyLoadingRoutes: Routes = [
    {
        path: 'customers',
        loadChildren: () => import('./customer.module').then((m) => m.CustomerModule),
        canLoad: [AuthenticatedGuard],
    },
];

const routes: Routes = [
    { path: '', component: CustomerListComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CustomerRoutingModule {}
