import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ItemService } from './item.service';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
  standalone: true,
  imports: [RouterOutlet],
  providers: [ItemService]
})
export class ItemComponent {

}
