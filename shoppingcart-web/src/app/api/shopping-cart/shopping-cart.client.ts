import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AUTHENTICATED_REQUEST } from '@muziehdesign/auth';
import { Observable } from 'rxjs';
import { OrderRequest } from './models/requests/order.request';
import { AuthorizationResponse } from './models/responses/authorization.response';
import { OrderReponse } from './models/responses/order.response';

@Injectable({
    providedIn: 'root',
})
export class ShoppingCartClient {
    constructor(private http: HttpClient) {}

    getAuthorization(): Observable<AuthorizationResponse> {
        return this.http.get<AuthorizationResponse>(`http://localhost:5000/api/v1/authorization`, { context: new HttpContext().set(AUTHENTICATED_REQUEST, true) });
    }

    createOrders(order: OrderRequest): Observable<OrderReponse> {
        return this.http.post<OrderReponse>(`http://localhost:5000/api/v1/orders`, { context: new HttpContext().set(AUTHENTICATED_REQUEST, true) });
    }
}
