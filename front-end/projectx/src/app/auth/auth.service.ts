import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AUTH_API_URL } from "../app-injection-tokens";
import { ISignInCommand } from "./commands";
import { Token } from './token';

// npm i @auth0/angular-jwt

export const ACCESS_TOKEN_KEY = 'access_token';

@Injectable({ providedIn: 'root' })
export class AuthService
{
    constructor(
        private _http: HttpClient,
        private _jwtHelper: JwtHelperService,
        private _router: Router,
        @Inject(AUTH_API_URL) private authUrl: string
    ) {}

    public login(command: ISignInCommand): Observable<Token>
    {
        const params = new HttpParams({
                fromObject: {
                    grant_type: 'password',
                    username: command.email,
                    password: command.password,
                    scope: '',
                    client_id: 'swagger',
                    client_secret: 'swaggerSecret'
                } 
            });

        return this._http.post<Token>(`${this.authUrl}connect/token`, params)
                         .pipe(tap(response => 
                            {
                                console.log(response);
                                localStorage.setItem(ACCESS_TOKEN_KEY, JSON.stringify(
                                      new Token(response.access_token, 
                                                response.refresh_token,
                                                response.expires_in,
                                                response.token_type,
                                                response.scope)));
                            })
                         );
    }

    public isAuthenticated(): boolean
    {
        var stringToken = localStorage.getItem(ACCESS_TOKEN_KEY);
        
        if(stringToken != null)
        {
            const obj = JSON.parse(stringToken);
            var token = obj.access_token;
            console.log('token', token);
            return token != null && !this._jwtHelper.isTokenExpired(token);
        }

        return false;
    }

    public logout(): void
    {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        // this._router.navigate(['']);
    }

    public getToken(): Token | null
    {
        const stringToken = localStorage.getItem(ACCESS_TOKEN_KEY);

        if(stringToken != null)
        {
            const parsed = JSON.parse(stringToken);

            const token = new Token(parsed.access_token, 
                parsed.refresh_token,
                parsed.expires_in,
                parsed.token_type,
                parsed.scope);
            
            return token;
        }

        return null;
    }
}