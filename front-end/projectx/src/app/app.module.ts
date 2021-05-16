import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ArticleListComponent } from './blog/article-list/article-list.component';
import { UserListComponent } from './users/user-list/user-list.component';
import { HomeComponent } from './home/home/home.component';
import { ArticleComponent } from './blog/article/article.component';
import { CreateArticleComponent } from './blog/create-article/create-article.component';
import { AUTH_API_URL, BLOG_API_URL, USERS_API_URL } from './app-injection-tokens';
import { environment } from 'src/environments/environment';
import { JwtModule } from '@auth0/angular-jwt'
import { ACCESS_TOKEN_KEY } from './auth/auth.service';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { ProfileComponent } from './users/profile/profile.component';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { NotificationListComponent } from './notifications/notification.component';
import { HomeListItemComponent } from './home/home-list-item/home-list-item.component';
import { ErrorInterceptor } from './http/error.interceptor';

export function tokenGetter()
{
      const stringToken = localStorage.getItem(ACCESS_TOKEN_KEY);

      if(stringToken != null)
      {
          const parsed = JSON.parse(stringToken);
          return parsed?.access_token;
      }

      return null;
}

@NgModule({
  declarations: [
    AppComponent,
    ArticleListComponent,
    UserListComponent,
    HomeComponent,
    ArticleComponent,
    CreateArticleComponent,
    SignInComponent,
    SignUpComponent,
    ProfileComponent,
    NotificationListComponent,
    HomeListItemComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    FlexLayoutModule,

    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: environment.tokenWhiteListedDomains,
        disallowedRoutes: [''],
      },
    })
  ],
  providers: [
    { provide: AUTH_API_URL, useValue: environment.authApi },
    { provide: BLOG_API_URL, useValue: environment.blogApi },
    { provide: USERS_API_URL, useValue: environment.usersApi },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
