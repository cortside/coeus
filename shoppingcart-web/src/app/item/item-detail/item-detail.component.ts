import { Component, Signal, ViewChildren } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ItemModel } from '../models/item.model';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule as MuziehFormsModule } from '@muziehdesign/forms'; 
import { AddToCartModel } from '../models/add-to-cart.model';
import { ViewChild } from '@angular/core';
import { NgFormModelState, NgFormModelStateFactory } from '@muziehdesign/forms';
import { ItemFacade } from '../item.facade';

@Component({
  selector: 'app-item-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, MuziehFormsModule],
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.scss']
})
export class ItemDetailComponent {
  item: Signal<ItemModel | undefined>;
  model:AddToCartModel;
  modelState!: NgFormModelState<AddToCartModel>;
  @ViewChild('cartForm', {static: false}) cartForm!: NgForm;
  constructor(private route: ActivatedRoute, private modelStateFactory: NgFormModelStateFactory, private facade: ItemFacade) {
      this.item = toSignal(facade.getItem(route.snapshot.params['sku']));
      //this.item = toSignal(facade.loadItem(route.snapshot.data));
      this.model = new AddToCartModel();
      this.model.quantity = 1;
  }

  ngAfterViewInit():void {
      this.modelState = this.modelStateFactory.create(this.cartForm, this.model);
  }

  public async addToCart() {
      // TODO: troubleshoot needed for undefined form error
      this.modelState = this.modelState || this.modelStateFactory.create(this.cartForm, this.model);
      const result = await this.modelState.validate();
      if(result.valid) {
          await this.facade.addItemToCart(this.item()!.sku, this.model.quantity!);
          console.log('added');
      } else {
          console.log('invalid model', result.errors);
      }
  }
}
