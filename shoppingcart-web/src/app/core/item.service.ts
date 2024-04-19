import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { PagedResponse } from '../api/paged.response';
import { UnexpectedError } from './errors';

@Injectable({
    providedIn: 'root',
})
export class ItemService {
    constructor(private client: CatalogClient) {}

    getItem(sku: string): Observable<ItemResponse | undefined> {
        return this.client.getItem(sku).pipe(
            catchError((error: HttpErrorResponse) => {
                if(error.status === HttpStatusCode.NotFound) {
                    return of(undefined);
                }
                return throwError(() => new UnexpectedError(error));
            })
        );
    }

    getItems(): Observable<PagedResponse<ItemResponse>> {
        return this.client.getItems();
    }
}