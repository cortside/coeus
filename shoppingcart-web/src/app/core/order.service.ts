import { Injectable } from '@angular/core';

@Injectable()
export class OrderService {
    constructor() {}

    debug(): void {
        console.log('debug');
    }
}
