import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '@muziehdesign/auth';
import { Observable } from 'rxjs';
import { PagedModel } from 'src/app/common/paged.model';
import { ShoppingCartService } from 'src/app/core/shopping-cart.service';
import { ItemService } from '../../core/item.service';
import { ItemModel } from '../models/item.model';

@Component({
    selector: 'app-item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss'],
})
export class ItemListComponent {
    items$: Observable<PagedModel<ItemModel>>;
    constructor(private service: ItemService, private shoppingCartService: ShoppingCartService) {
        this.items$ = service.getItems();
    }

    authorize() {}

    addItem(item: ItemModel) {
        this.shoppingCartService.addItem(item.sku, 1);
    }
}
