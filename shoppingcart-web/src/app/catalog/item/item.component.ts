import { Component, Signal, ViewChildren } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ItemService } from 'src/app/core/item.service';
import { ItemModel } from '../models/item.model';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule as MuziehFormsModule } from '@muziehdesign/forms'; 
import { AddToCartModel } from '../models/add-to-cart.model';
import { ViewChild, AfterViewInit } from '@angular/core';
import { NgFormModelState, NgFormModelStateFactory } from '@muziehdesign/forms';
import { ShoppingCartService } from 'src/app/core/shopping-cart.service';

@Component({
    selector: 'app-item',
    standalone: true,
    imports: [CommonModule, FormsModule, MuziehFormsModule],
    templateUrl: './item.component.html',
    styleUrls: ['./item.component.scss'],
})
export class ItemComponent implements AfterViewInit {
    
    item: Signal<ItemModel | undefined>;
    model:AddToCartModel;
    modelState!: NgFormModelState<AddToCartModel>;
    @ViewChild('cartForm', {static: false}) cartForm!: NgForm;
    constructor(private route: ActivatedRoute, private modelStateFactory: NgFormModelStateFactory, private service: ItemService, private cartService: ShoppingCartService){
        this.item = toSignal(service.getItem(route.snapshot.params['sku']));
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
            this.cartService.addItem(this.item()!.sku, this.model.quantity!);
            console.log('added');
        } else {
            console.log('invalid model', result.errors);
        }
    }
}
