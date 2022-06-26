import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ItemResponse } from './models/responses/item.response';

@Injectable({
    providedIn: 'root'
})
export class CatalogClient {
    constructor(private http: HttpClient) {}

    getItem(sku: string): Observable<ItemResponse> {
        return this.http.get<ItemResponse>(
            `http://localhost:5001/api/v1/items/${sku}`
        );
    }
}
