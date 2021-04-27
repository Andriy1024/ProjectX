import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ICreateArticleCommand } from '../interfaces/article';
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

  public ngOnInit(): void {
    this.form = this._formBuilder.group({
      name: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(5)])),
      content: this._formBuilder.control('', Validators.compose([Validators.required, Validators.minLength(10)]))
    });
  }

  public onSubmit(command: ICreateArticleCommand): void {
    this._blogService.createArticle(command);
    this._router.navigate(['/blog']);
  }

  get name() { return this.form.get('name'); }

  get content() { return this.form.get('content'); }
}
