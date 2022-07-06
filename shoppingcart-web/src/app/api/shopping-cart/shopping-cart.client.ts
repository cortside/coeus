import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizationResponse } from './models/responses/authorization.response';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartClient {

  constructor(private http:HttpClient) { }

    getAuthorization(): Observable<AuthorizationResponse> {
        return this.http.get<AuthorizationResponse>(`http://localhost:5000/api/v1/authorization`);
    }
}
