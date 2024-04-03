import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { PagedResponse } from '../api/paged.response';

@Injectable()
export class ItemService implements OnDestroy {
    constructor(private client: CatalogClient) {}

    ngOnDestroy(): void {
        console.log('destroying item service');
    }

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
