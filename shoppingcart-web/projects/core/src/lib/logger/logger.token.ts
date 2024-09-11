import { InjectionToken } from "@angular/core";
import { Logger } from "./logger";

export const LOGGER = new InjectionToken<Logger>('Logger');