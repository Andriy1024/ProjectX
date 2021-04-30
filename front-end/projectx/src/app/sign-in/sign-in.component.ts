import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { ISignInCommand } from '../auth/commands';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent implements OnInit {

    public form!: FormGroup;
    public error: any = null;

    constructor(private _formBuilder: FormBuilder,
                private _authService: AuthService,
                private _router: Router) { }

    public ngOnInit(): void 
    {
        this.form = this._formBuilder.group({
            email: this._formBuilder.control('', Validators.compose([Validators.required, Validators.email])),
            password: this._formBuilder.control('', Validators.compose([Validators.required]))
        });
    }

    public onSubmit(command: ISignInCommand): void 
    {
        this._authService
            .login(command)
            .subscribe((token) => console.log(token),
                      (error) => this.error = error,
                      () => this._router.navigate(['']));
    }

    get email() { return this.form.get('email'); }

    get password() { return this.form.get('password'); }
}
