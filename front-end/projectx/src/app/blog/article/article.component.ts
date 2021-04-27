import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Guid } from 'guid-typescript';
import { Article } from '../interfaces/article';
import { BlogService } from '../services/blog.service';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.scss']
})
export class ArticleComponent implements OnInit {

  public article: Article | undefined;
  public articleId: Guid | undefined;

  constructor(private _route: ActivatedRoute, private _blogService: BlogService) {}

  public ngOnInit(): void {
    this._route.params.subscribe(p => 
      {
        this.articleId = p['id'];
        
        console.log('id', this.articleId)

        if(this.articleId) { 
           this.getArticle(this.articleId);
           return;
        }

        throw new Error('Article id undefined');
      });
  }

  public getArticle(id: Guid): void {
    this.article = this._blogService.getArticle(id);
  }

}
