import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { IDataResponse, IResponse } from "src/app/shared/IResponse";
import { IArticle, IFullArticle } from "../interfaces/article";
import { ICreateArticleCommand, IDeleteArticleCommand } from "../interfaces/commands";
import { IArticlesQuery, IFindArticleQuery } from "../interfaces/queries";

@Injectable({providedIn: 'root'})
export class BlogService {

    private _blogUrl: string = "http://localhost:5001/api"

    constructor(private _blogClient: HttpClient) {}

    public getArticlesAsync(query: IArticlesQuery|null=null): Observable<IArticle[]> 
    {
        return this._blogClient
                   .get<IDataResponse<IArticle[]>>(`${this._blogUrl}/articles`)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
    }

    public getArticleAsync(query: IFindArticleQuery): Observable<IFullArticle> 
    {
        return this._blogClient
                   .get<IDataResponse<IFullArticle>>(`${this._blogUrl}/articles/${query.id}`)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
    }

    public deleteArticleAsync(command: IDeleteArticleCommand) : void
    {
        this._blogClient.delete(`${this._blogUrl}/articles/${command.id}`)
                        .pipe(catchError(this.handleError))
                        .subscribe({
                            next: (val) => console.log('Delete article result: ', val),
                            error: (error) => console.log('Delete article error: ', error),
                            complete: () => console.log('Delete article completed')
                        });
    }

    public createArticleAsync(command: ICreateArticleCommand): Observable<IArticle>
    {
        return this._blogClient
                   .post<IDataResponse<IFullArticle>>(`${this._blogUrl}/articles`, command)
                   .pipe(
                        map(r => this.map(r)),
                        catchError(this.handleError)
                    );
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