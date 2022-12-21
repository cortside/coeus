import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ItemRoutingModule } from './item-routing.module';
import { ItemListComponent } from './item-list/item-list.component';
import { ItemCardComponent } from './item-card/item-card.component';
import { PageHeaderComponent } from 'muzieh-ngcomponents';


@NgModule({
  declarations: [
    ItemListComponent,
    ItemCardComponent,
  ],
  imports: [
    CommonModule,
    ItemRoutingModule,
    PageHeaderComponent
  ]
})
export class ItemModule { }
