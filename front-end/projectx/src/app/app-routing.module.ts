import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ArticleListComponent } from './blog/article-list/article-list.component';
import { ArticleComponent } from './blog/article/article.component';
import { CreateArticleComponent } from './blog/create-article/create-article.component';
import { HomeComponent } from './home/home/home.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { ProfileComponent } from './users/profile/profile.component';
import { UserListComponent } from './users/user-list/user-list.component';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'blog', component: ArticleListComponent},
  {path: 'articles/:id', component: ArticleComponent},
  {path: 'articles-create', component: CreateArticleComponent},
  {path: 'users', component: UserListComponent},
  {path: 'users/:id', component: ProfileComponent},
  {path: 'sign-in', component: SignInComponent},
  {path: 'sign-up', component: SignUpComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
