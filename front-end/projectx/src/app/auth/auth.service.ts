import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AUTH_API_URL } from "../app-injection-tokens";
import { ISignInCommand } from "./commands";
import { CurrentUser } from "./currentUser";
import { Token } from './token';

// npm i @auth0/angular-jwt

export const ACCESS_TOKEN_KEY = 'access_token';

@Injectable({ providedIn: 'root' })
export class AuthService
{
    public currentUser: CurrentUser | null = null;

    constructor(
        private _http: HttpClient,
        private _jwtHelper: JwtHelperService,
        private _router: Router,
        @Inject(AUTH_API_URL) private authUrl: string
    ) 
    {
        const token = this.getAccessToken();
        if (token)
        {
            const decodedToken = this._jwtHelper.decodeToken(token);
            this.currentUser = new CurrentUser(decodedToken.sub, decodedToken.username);
        }
    }

    public login(command: ISignInCommand): Observable<Token>
    {
        const params = new HttpParams({
                fromObject: {
                    grant_type: 'password',
                    username: command.email,
                    password: command.password,
                    scope: '',
                    client_id: 'webclient',
                    client_secret: 'webclientSecret'
                } 
            });

        return this._http.post<Token>(`${this.authUrl}connect/token`, params)
                         .pipe(tap(response => 
                            {
                                const token =  new Token(response.access_token, 
                                    response.refresh_token,
                                    response.expires_in,
                                    response.token_type,
                                    response.scope);

                                this.storageToken(token);
                                const decodedToken = this._jwtHelper.decodeToken(response.access_token);
                                this.currentUser = new CurrentUser(decodedToken.sub, decodedToken.username);
                            })
                         );
    }

    public isAuthenticated(): boolean
    {   
        const token = this.getAccessToken();
        
        if(!token) return false;

        const result = !this._jwtHelper.isTokenExpired(token);
        
        if(!result)
        {
            localStorage.removeItem(ACCESS_TOKEN_KEY);
        }

        return result;
    }

    public logout(): void
    {
        this.currentUser = null;
        localStorage.removeItem(ACCESS_TOKEN_KEY);
    }

    public getToken(): Token | null
    {
        const stringToken = this.getStringTokenObject();

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

    private getStringTokenObject(): string | null
    {
        return localStorage.getItem(ACCESS_TOKEN_KEY);
    }

    private getAccessToken(): any
    {
        var stringToken = this.getStringTokenObject();
        
        if(!stringToken) return false;
        
        const obj = JSON.parse(stringToken);
        
        return obj.access_token;
    }

    private storageToken(token: Token)
    {
        localStorage.setItem(ACCESS_TOKEN_KEY, JSON.stringify(token));
    }
}