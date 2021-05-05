import { Component } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent 
{
    public title = 'projectx';

    constructor(private _authService: AuthService) {}

    get isAuthenticated(): boolean
    {
        return this._authService.isAuthenticated();
    }

    get userId(): number | undefined
    {
        return this._authService.currentUser?.id;
    }

    public logOut()
    {
        this._authService.logout();
    }
}