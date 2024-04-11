import { Component, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemModel } from '../models/item.model';
import { ItemCardComponent } from '../item-card/item-card.component';
import { ItemFacade } from '../item.facade';
import { toSignal } from '@angular/core/rxjs-interop';
import { PagedModel } from 'src/app/models/paged.model';

@Component({
    selector: 'app-item-list',
    standalone: true,
    imports: [CommonModule, ItemCardComponent],
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss'],
})
export class ItemListComponent {
    items: Signal<PagedModel<ItemModel> | undefined>;
    constructor(private facade: ItemFacade) {
        this.items = toSignal(facade.getItems());
    }
}
