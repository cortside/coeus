import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { orderLazyLoadingRoutes } from './order';

const routes: Routes = [
    { path: '', redirectTo: '/catalog', pathMatch: 'full' },
    ...orderLazyLoadingRoutes
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
