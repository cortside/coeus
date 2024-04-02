import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ItemModelAssembler } from './item-model.assembler';
import { ItemFacade } from './item.facade';
import { ItemService } from './item.service';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
  standalone: true,
  imports: [RouterOutlet],
  providers: [ItemService, ItemModelAssembler, ItemFacade, ItemModelAssembler]
})
export class ItemComponent {

}
