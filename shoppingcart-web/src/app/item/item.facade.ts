import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ItemModelAssembler } from './item-model.assembler';
import { ItemService } from './item.service';
import { ItemModel } from './models/item.model';

@Injectable()
export class ItemFacade {
    constructor(
        private service: ItemService,
        private assembler: ItemModelAssembler
    ) {}

    getItem(sku: string): Observable<ItemModel> {
        return this.service.getItem(sku).pipe(map((x) => this.assembler.toItemModel(x)));
    }
}
