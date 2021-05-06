import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
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
                private _router: Router,
                private _authService: AuthService) {}

    public ngOnInit(): void 
    {
        this._route.params.subscribe(p => 
        {
          this.articleId = p['id'];

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
                              (article) => {this.article = article; console.log(this.article)},
                              (error) => console.log('Get article error: ', error),
                              () => console.log('Get article completed')
                         );
    }

    public delete(): void 
    {
        this._blogService.deleteArticleAsync(this.article);
        this._router.navigate(['blog']);
    }

    // Defines if the current user is the owner of the article.
    get canEdit() : boolean
    {
        const result = this._authService.isAuthenticated() && this._authService.currentUser?.id == this.article?.author?.id;
        return result;      
    }
}
