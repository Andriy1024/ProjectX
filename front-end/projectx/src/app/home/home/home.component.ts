import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';
import { Token } from 'src/app/auth/token';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public token: Token | null = null;

  constructor(public _authService: AuthService) { }

  public ngOnInit(): void 
  {
      // this._authService.login('admin@projectX.com', 'AdminPass123$$')
      //     .subscribe(t => 
      //       {
      //         this.token = new Token(t.access_token, t.refresh_token, t.expires_in, t.token_type, t.scope);
      //       });

      this.token = this._authService.getToken();
  }

}
