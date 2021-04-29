import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
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

    constructor(private _blogService: BlogService) {}

    public ngOnInit(): void 
    {
        this.getArticles();
    }

    public delete(article: IArticle): void 
    {
        this._blogService.deleteArticleAsync(article);
        this.getArticles();
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