import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { IFullArticle } from '../interfaces/article';
import { BlogService } from '../services/blog.service';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.scss']
})
export class ArticleComponent implements OnInit {

  public article!: IFullArticle;
  public articleId!: Guid;

  constructor(private _route: ActivatedRoute, 
              private _blogService: BlogService,
              private _router: Router) {}

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
    this.article = this._blogService.getFullArticle(id);
  }

  public delete(): void {
    this._blogService.deleteArticle(this.article);
    this._router.navigate(['/blog']);
  }

}
