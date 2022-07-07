import { InjectionToken } from "@angular/core";
import { UserManagerSettings } from "oidc-client";

export const USER_MANAGER_SETTINGS_TOKEN = new InjectionToken<UserManagerSettings>('UserManagerSettings');
