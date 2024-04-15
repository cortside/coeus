import { ErrorHandler, Inject, Injectable } from "@angular/core";
import { Logger, LOGGER } from "@muziehdesign/core";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(@Inject (LOGGER) private logger: Logger) {}

    /* eslint-disable  @typescript-eslint/no-explicit-any */
    handleError(error: any): void {
        this.logger.error(error); // TODO: log more things
    }
}