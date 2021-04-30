import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IArticle } from '../interfaces/article';
import { ICreateArticleCommand } from '../interfaces/commands';
import { BlogService } from '../services/blog.service';

@Component({
  selector: 'app-create-article',
  templateUrl: './create-article.component.html',
  styleUrls: ['./create-article.component.scss']
})
export class CreateArticleComponent implements OnInit {

    public form!: FormGroup;

    constructor(private _formBuilder: FormBuilder,
                private _blogService: BlogService,
                private _router: Router) { }

    public ngOnInit(): void 
    {
        this.form = this._formBuilder.group({
            title: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
            body: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(10)]))
        });
    }

    public onSubmit(command: ICreateArticleCommand): void 
    {
        this._blogService.createArticleAsync(command)
                         .subscribe({
                              next: (val) => console.log('Create article result: ', val),
                              error: (error) => console.log('Create article failed: ', error),
                              complete: () => {
                                  console.log('Article created.');
                                  this._router.navigate(['/blog']);
                                }
                         });
    }

    get title() { return this.form.get('title'); }

    get body() { return this.form.get('body'); }
}