import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { User } from "../interfaces/user";
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { IDataResponse } from "src/app/shared/IResponse";
import { ICreateUserCommand } from "../interfaces/commands";
import { USERS_API_URL } from "src/app/app-injection-tokens";

@Injectable({ providedIn: 'root' })
export class UserService {
    
    constructor(
        private usersClient: HttpClient,
        @Inject(USERS_API_URL) private usersUrl: string) {}

    public getUsersAsync(): Observable<User[]>  
    {
        return this.usersClient
                   .get<IDataResponse<User[]>>(`${this.usersUrl}users`)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
    }

    public getUserAsync(id: number): Observable<User>  
    {
        return this.usersClient
                   .get<IDataResponse<User>>(`${this.usersUrl}users/${id}`)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
    }

    public createUserAsync(command: ICreateUserCommand): Observable<User>
    {
        const params = new HttpParams({
            fromObject: {
                FirstName: command.firstName,
                LastName: command.lastName,
                Email: command.email,
                Password: command.password,
                ConfirmPassword: command.confirmPassword,
                'Address.Country': command.country,
                'Address.City': command.city,
                'Address.Street': command.street
            } 
        });
        
        return this.usersClient
                   .post<IDataResponse<User>>(`${this.usersUrl}users`, params)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
    }

    public deleteUser(user: User) 
    {
        console.log('Delete user: ', user);
    }

    private map<TOut>(response: IDataResponse<TOut>): TOut 
    {
        if(!response.isSuccess)
        {
            console.error('error:', response.error);
            throw new Error(`Blog service request failed, error: ${response?.error?.msessage}`);
        }

        return response.data;
    }

    private handleError(error: HttpErrorResponse)
    {
        console.error(error.message);
        return throwError('A data error occurred, please try again.');
    }
}