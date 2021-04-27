import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Guid } from "guid-typescript";
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

  public ngOnInit(): void {
    this.articles = this._blogService.getArticles();

    this.createArticleFrom = new FormGroup({
      name: new FormControl('', Validators.required),
    });
  }

  public delete(article: IArticle): void {
    this._blogService.deleteArticle(article);
    this.articles = this._blogService.getArticles();
  }
}