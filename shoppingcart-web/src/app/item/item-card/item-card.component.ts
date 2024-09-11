import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ItemModel } from '../models/item.model';

@Component({
    selector: 'app-item-card',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './item-card.component.html',
    styleUrls: ['./item-card.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ItemCardComponent {
    @Input()
    item!: ItemModel;
}
