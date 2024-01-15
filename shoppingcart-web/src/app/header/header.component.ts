import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser, AuthenticationService } from '@muziehdesign/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  
  user: AuthenticatedUser | undefined;
  constructor(private auth: AuthenticationService) {
    
  }

  async ngOnInit(): Promise<void> {
    this.user = await this.auth.getUser();
}

}
