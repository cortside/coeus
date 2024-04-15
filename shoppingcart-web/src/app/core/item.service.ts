import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { PagedResponse } from '../api/paged.response';

@Injectable({
    providedIn: 'root',
})
export class ItemService {
    constructor(private client: CatalogClient) {}

    getItem(sku: string): Observable<ItemResponse> {
        return this.client.getItem(sku).pipe(
            catchError((error: HttpErrorResponse) => {
                console.log(error);
                throw new Error('not yet implemeted');
            })
        );
    }

    getItems(): Observable<PagedResponse<ItemResponse>> {
        return this.client.getItems();
    }
}