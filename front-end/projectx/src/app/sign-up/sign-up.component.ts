import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ICreateUserCommand } from '../users/interfaces/commands';
import { UserService } from '../users/services/user.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

    public form!: FormGroup;
    public httpError: any = null;

    constructor(private _formBuilder: FormBuilder,
                private _usersService: UserService,
                private _router: Router) { }

    public ngOnInit(): void 
    {
        this.form = this._formBuilder.group({
            firstName: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
            lastName: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
            email: this._formBuilder.control('', Validators.compose([Validators.required, Validators.email])),
            password: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
            confirmPassword: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
            country: this._formBuilder.control('', Validators.compose([Validators.required])),
            city: this._formBuilder.control('', Validators.compose([Validators.required])),
            street: this._formBuilder.control('', Validators.compose([Validators.required,])),
        });
    }

    public onSubmit(command: ICreateUserCommand): void 
    {
        this._usersService.createUserAsync(command)
                          .subscribe({
                              next: (val) => console.log('Create article result: ', val),
                              error: (error) => {
                                  console.log('Create article failed: ', error);
                                  this.httpError = error;
                              },
                              complete: () => {
                                  console.log('User created.');
                                  this._router.navigate(['/users']);
                                }
                          });
    }
}
