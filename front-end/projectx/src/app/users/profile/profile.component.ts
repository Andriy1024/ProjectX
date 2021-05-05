import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../interfaces/user';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

    public user!: User;
    public userId!: number;

    constructor(private _route: ActivatedRoute, 
                private _userService: UserService,
                private _router: Router)
    {
        this._route.params.subscribe(p => 
          {
            this.userId = p['id'];
            
            console.log('article id: ', this.userId)

            if(this.userId) { 
                this.getUser(this.userId);
                return;
            }

            throw new Error('User id undefined');
          });
    }

    public ngOnInit(): void 
    {
    }

    private getUser(id: number)
    {
        this._userService.getUserAsync(id)
              .subscribe(
                  (user) => this.user = user,
                  (error) => console.log('Get user error: ', error),
                  () => console.log('Get user completed')
              );
    }
}
