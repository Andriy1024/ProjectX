import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ArticleListComponent } from './blog/article-list/article-list.component';
import { UserListComponent } from './users/user-list/user-list.component';
import { HomeComponent } from './home/home/home.component';
import { ArticleComponent } from './blog/article/article.component';

@NgModule({
  declarations: [
    AppComponent,
    ArticleListComponent,
    UserListComponent,
    HomeComponent,
    ArticleComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
