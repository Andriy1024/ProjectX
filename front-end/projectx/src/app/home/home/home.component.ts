import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';
import { CurrentUser } from 'src/app/auth/currentUser';
import { Token } from 'src/app/auth/token';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    public token: Token | null = null;
    public currentUser: CurrentUser | null = null;

    constructor(public _authService: AuthService) { }

    public ngOnInit(): void 
    {
        this.token = this._authService.getToken();
        this.currentUser = this._authService.currentUser;
    }

}
