import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ItemDetailComponent } from './item-detail/item-detail.component';
import { ItemListComponent } from './item-list/item-list.component';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
  standalone: true,
  imports: [RouterOutlet, ItemComponent, ItemListComponent, ItemDetailComponent]
})
export class ItemComponent {

}
