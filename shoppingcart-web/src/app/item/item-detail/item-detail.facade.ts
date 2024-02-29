import { Injectable } from '@angular/core';
import { ShoppingCart } from 'src/app/core/shopping-cart';
import { ItemService } from '../item.service';

@Injectable()
export class ItemDetailFacade {
    constructor(private cart: ShoppingCart, private itemService: ItemService) { }

    addToCart() {
        
    }
}
