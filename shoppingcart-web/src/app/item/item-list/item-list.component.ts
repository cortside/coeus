import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizationService } from 'src/app/auth/authorization.service';
import { ListResult } from 'src/app/common/list-result';
import { ItemService } from '../item.service';
import { ItemModel } from '../models/item.model';

@Component({
  selector: 'app-item-list',
  templateUrl: './item-list.component.html',
  styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent implements OnInit {

    items$: Observable<ListResult<ItemModel>>;
  constructor(private service: ItemService, private authorizationService: AuthorizationService) {
    this.items$ = service.getItems();
  }

  ngOnInit(): void { }

  authorize() {
    this.authorizationService.authorize();
  }
}
