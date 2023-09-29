import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ShoppingCartClient } from 'src/app/api/shopping-cart/shopping-cart.client';
import { CustomerModel } from 'src/app/common/create-customer.model';
import { PagedModel } from 'src/app/common/paged.model';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent {

  //customers$: Observable<PagedModel<CustomerModel>>;
  constructor(private shoppingCartClient: ShoppingCartClient) {
    //update this area below to use the variable 2 lines above and the
    //ShoppingCartClient to get your data from the api

  }
  authorize() {
  }

}
