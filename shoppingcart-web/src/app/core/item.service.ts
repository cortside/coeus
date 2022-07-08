import { Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ListResult } from '../common/list-result';
import { ItemModel } from '../item/models/item.model';

@Injectable({
  providedIn: 'root'
})
export class ItemService {

  constructor(private client: CatalogClient) { }

  getItems() : Observable<ListResult<ItemModel>> {
    return forkJoin([
        this.client.getItem('SKU0000001'),
        this.client.getItem('SKU0000002'),
        this.client.getItem('SKU0000003'),
        this.client.getItem('SKU0000004')
    ]).pipe(
        map(x=> <ListResult<ItemModel>>{results: x as ItemModel[]})
    );
  }

}
