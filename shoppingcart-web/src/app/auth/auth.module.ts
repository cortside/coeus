import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManager } from 'oidc-client';
import { userManagerFactory } from './user-manager.factory';
import { USER_MANAGER_SETTINGS_TOKEN } from './user-manager-settings.token';

@NgModule({
    declarations: [],
    imports: [CommonModule],
    providers: [
        {
            provide: UserManager,
            useFactory: userManagerFactory,
            deps: [USER_MANAGER_SETTINGS_TOKEN],
        },
    ],
})
export class AuthModule {}
