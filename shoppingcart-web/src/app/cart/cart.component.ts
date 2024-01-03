import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';
import { RouterModule } from '@angular/router';
import { FormsModule as MuziehFormsModule, NgFormModelState } from '@muziehdesign/forms';
import { CustomerInputModel } from './customer-input.model';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
    selector: 'app-cart',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule, MuziehFormsModule],
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.scss'],
})
export class CartComponent {
    items$: Observable<CartItemModel[]>;
    model: CustomerInputModel;
    modelState!: NgFormModelState<CustomerInputModel>;
    @ViewChild('cartForm', {static: true}) cartForm!: NgForm;
    constructor(private service: ShoppingCartService) {
        this.items$ = this.service.getCartItems();
        this.model = new CustomerInputModel();
    }

    public createOrder() {
        console.log('attempt to create order');
    }

    checkout(): void {
        console.log('test');
    }
}
