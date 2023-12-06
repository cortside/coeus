import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemService } from 'src/app/core/item.service';
import { ItemModel } from '../models/item.model';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { Signal } from '@angular/core';

@Component({
    selector: 'app-item',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './item.component.html',
    styleUrls: ['./item.component.scss'],
})
export class ItemComponent {
    
    item: Signal<ItemModel | undefined>;
    constructor(private route: ActivatedRoute, private service: ItemService){
        this.item = toSignal(service.getItem(route.snapshot.params['sku']));
    }
}
