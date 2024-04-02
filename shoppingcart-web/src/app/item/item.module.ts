import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemDetailComponent } from './item-detail/item-detail.component';
import { ItemListComponent } from './item-list/item-list.component';
import { ItemComponent } from './item.component';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ItemComponent,
    ItemDetailComponent,
    ItemListComponent
  ]
})
export class ItemModule { }
