import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '@muziehdesign/auth';
import { Observable } from 'rxjs';
import { ListResult } from 'src/app/common/list-result';
import { ShoppingCartService } from 'src/app/core/shopping-cart.service';
import { ItemService } from '../../core/item.service';
import { ItemModel } from '../models/item.model';

@Component({
    selector: 'app-item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss'],
})
export class ItemListComponent implements OnInit {
    items$: Observable<ListResult<ItemModel>>;
    constructor(private service: ItemService, private shoppingCartService: ShoppingCartService) {
        this.items$ = service.getItems();
    }

    ngOnInit(): void {}

    authorize() {}

    addItem(item: ItemModel) {
        this.shoppingCartService.addItem(item.sku, 1);
    }
}
