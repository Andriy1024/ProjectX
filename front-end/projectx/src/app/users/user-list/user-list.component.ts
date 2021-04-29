import { Component, OnInit } from '@angular/core';
import { User } from '../interfaces/user';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

    public users: User[] = [];

    constructor(private _userService: UserService) { }

    public ngOnInit(): void 
    {
        this.getUsers();
    }

    public delete(user: User)
    {
        this._userService.deleteUser(user);
        //this.getUsers();
    }

    private getUsers()
    {
        this._userService.getUsersAsync()
            .subscribe(users => 
            {
              console.log('getUsersAsync', users);
              this.users = users;
            });
    }
}