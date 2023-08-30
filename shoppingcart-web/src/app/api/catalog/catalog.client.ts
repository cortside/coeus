import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ItemResponse } from './models/responses/item.response';
import { AppConfig } from 'src/environments/app-config';
import { PagedResponse } from '../paged.response';
import { AUTHENTICATED_REQUEST } from '@muziehdesign/core';

@Injectable({
    providedIn: 'root',
})
export class CatalogClient {
    private items: Partial<ItemResponse>[] = [
        {
            sku: 'SKU00001',
            name: 'Pappy Van Winkle 10 Year',
        },
        {
            sku: 'SKU00002',
            name: 'Pappy Van Winkle 12 Year',
        },
        {
            sku: 'SKU00003',
            name: 'Pappy Van Winkle 15 Year',
        },
        {
            sku: 'SKU00004',
            name: 'Pappy Van Winkle 18 Year',
        },
        {
            sku: 'SKU00005',
            name: 'Pappy Van Winkle 20 Year',
        },
        {
            sku: 'SKU00006',
            name: 'Pappy Van Winkle 23 Year',
        },
        {
            sku: 'SKU00007',
            name: 'Pappy Van Winkle 25 Year',
        },
        {
            sku: 'SKU00007',
            name: 'Fireball Cinnamon Whisky',
        },
    ];
    constructor(
        private http: HttpClient,
        private config: AppConfig
    ) {}

    getItem(sku: string): Observable<ItemResponse> {
        const item = this.items.find((i) => i.sku == sku);
        return this.http.get<ItemResponse>(`${this.config.catalogApi?.url}/api/v1/items/${sku}`, { context: new HttpContext().set(AUTHENTICATED_REQUEST, true) }).pipe(map((x) => Object.assign({}, x, item)));
    }

    getItems(): Observable<PagedResponse<ItemResponse>> {
        return this.http.get<PagedResponse<ItemResponse>>(`${this.config.catalogApi?.url}/api/v1/items`, { context: new HttpContext().set(AUTHENTICATED_REQUEST, true) });
    }
}
