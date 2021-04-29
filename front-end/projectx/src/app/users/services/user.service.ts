import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { User } from "../interfaces/user";
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { IDataResponse } from "src/app/shared/IResponse";

@Injectable({providedIn: 'root'})
export class UserService {
  
    private usersUrl: string = "http://localhost:5000/api";

    constructor(private usersClient: HttpClient) {}

    public getUsersAsync(): Observable<User[]>  
    {
        return this.usersClient
                .get<IDataResponse<User[]>>(`${this.usersUrl}/users`)
                .pipe(
                    map(r => 
                    {
                        if(!r.isSuccess)
                        {
                            throw new Error('Get users request failed');
                        }

                        return r.data;
                    }),
                    catchError(this.handleError)
                );
    }

    public addUser(user: User) 
    {
        console.log('Create user: ', user);
    }

    public deleteUser(user: User) 
    {
        console.log('Delete user: ', user);
    }

    private handleError(error: HttpErrorResponse) 
    {
        console.error(error.message);
        return throwError('A data error occurred, please try again.');
    }
}