import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemService } from 'src/app/core/item.service';
import { ItemModel } from '../models/item.model';
import { PagedModel } from 'src/app/common/paged.model';
import { Observable } from 'rxjs';
import { ItemCardComponent } from '../item-card/item-card.component';

@Component({
    selector: 'app-item-list',
    standalone: true,
    imports: [CommonModule, ItemCardComponent],
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss'],
})
export class ItemListComponent {
    items$: Observable<PagedModel<ItemModel>>;
    constructor(private service: ItemService) {
        this.items$ = service.getItems();
    }
}
