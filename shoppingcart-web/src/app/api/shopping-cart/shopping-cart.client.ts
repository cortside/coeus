import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AUTHENTICATED_REQUEST } from '@muziehdesign/core';
import { Observable } from 'rxjs';
import { AppConfig } from 'src/environments/app-config';
import { PagedResponse } from '../paged.response';
import { OrderRequest } from './models/requests/order.request';
import { AuthorizationResponse } from './models/responses/authorization.response';
import { OrderReponse } from './models/responses/order.response';

@Injectable({
    providedIn: 'root',
})
export class ShoppingCartClient {
    constructor(
        private http: HttpClient,
        private config: AppConfig
    ) {}

    getAuthorization(): Observable<AuthorizationResponse> {
        return this.http.get<AuthorizationResponse>(`${this.config.shoppingCartApi?.url}/api/v1/authorization`, {
            context: new HttpContext().set(AUTHENTICATED_REQUEST, true),
        });
    }

    createOrders(order: OrderRequest): Observable<OrderReponse> {
        return this.http.post<OrderReponse>(`${this.config.shoppingCartApi?.url}/api/v1/orders`, order, { context: new HttpContext().set(AUTHENTICATED_REQUEST, true) });
    }

    getOrders(): Observable<PagedResponse<OrderReponse>> {
        return this.http.get<PagedResponse<OrderReponse>>(`${this.config.shoppingCartApi?.url}/api/v1/orders`, {context: new HttpContext().set(AUTHENTICATED_REQUEST, true)});
    }
}
