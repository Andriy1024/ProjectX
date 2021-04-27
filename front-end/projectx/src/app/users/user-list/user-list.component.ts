import { Component, Injectable, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

  users: User[] = [];

  createArticleFrom: FormGroup = new FormGroup({});

  constructor(private _userService: UserService) { }

  ngOnInit(): void {
    this.users = this._userService.getUsers();

    this.createArticleFrom = new FormGroup({
      name: new FormControl('', Validators.required),
    });
  }

  delete(user: User){
    this._userService.deleteUser(user);
    this.users = this._userService.getUsers();
  }

  createUser() {
    const { name } = this.createArticleFrom.value;
    this._userService.addUser({ id: Guid.create(), name });
    this.users = this._userService.getUsers();
  }
}

@Injectable({providedIn: 'root'})
export class UserService {
  private _users: User[] = [
    {
      id: Guid.create(),
      name: 'Andrii'
    },
    {
      id: Guid.create(),
      name: 'Igor'
    },
    {
      id: Guid.create(),
      name: 'Taras'
    }
  ];

  getUsers() : User[] {
    return this._users;
  }

  addUser(user: User) {
    this._users.push(user);
  }

  deleteUser(user: User) {
    this._users = this._users.filter(u => u.id !== user.id);
  }
}

export interface User {
  id: Guid,
  name: string
}
