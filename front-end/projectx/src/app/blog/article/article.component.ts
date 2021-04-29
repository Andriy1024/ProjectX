import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IFullArticle } from '../interfaces/article';
import { BlogService } from '../services/blog.service';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.scss']
})
export class ArticleComponent implements OnInit {

    public article!: IFullArticle;
    public articleId!: number;

    constructor(private _route: ActivatedRoute, 
                private _blogService: BlogService,
                private _router: Router) {}

    public ngOnInit(): void 
    {
        this._route.params.subscribe(p => 
        {
          this.articleId = p['id'];
          
          console.log('article id: ', this.articleId)

          if(this.articleId) { 
            this.getArticle(this.articleId);
            return;
          }

          throw new Error('Article id undefined');
        });
    }

    public getArticle(id: number): void 
    {
        this._blogService.getArticleAsync({ id: id })
                         .subscribe(
                              (article) => this.article = article,
                              (error) => console.log('Get article error: ', error),
                              () => console.log('Get article completed')
                         );
    }

    public delete(): void 
    {
        this._blogService.deleteArticleAsync(this.article);
        this._router.navigate(['blog']);
    }
}
