import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmptyComponent } from './empty/empty.component';

const routes: Routes = [
    { path: 'login-redirect', component: EmptyComponent },
    { path: 'silent-redirect', component: EmptyComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuthRoutingModule {}
