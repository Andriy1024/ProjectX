import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from 'src/app/auth/auth.service';
import { IArticle } from '../interfaces/article';
import { BlogService } from '../services/blog.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.scss']
})
export class ArticleListComponent implements OnInit {

    public articles: IArticle[] = [];

    public createArticleFrom: FormGroup = new FormGroup({});

    constructor(private _blogService: BlogService,
                private _authService: AuthService) {}

    public ngOnInit(): void 
    {
        this.getArticles();
    }

    get canCreate(): boolean 
    {
        return this._authService.isAuthenticated(); 
    }

    private getArticles() 
    {
        this._blogService.getArticlesAsync()
                         .subscribe(
                              (articles) => this.articles = articles,
                              (error) => console.log('Get articles error: ', error),
                              () => console.log('Get articles completed.')
                          );
    }
}