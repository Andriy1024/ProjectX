import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { NotificationService } from '../notifications/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private _authenticationService: AuthService,
                private notificationService: NotificationService)
                { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if ([401, 403].includes(err.status) && this._authenticationService.isAuthenticated()) {
                // auto logout if 401 or 403 response returned from api
                this._authenticationService.logout();
            }

            const error = (err && err.error && err.error.message) || err.statusText;
            // this.notificationService.error('Http error', error);
            console.error(err);
            return throwError(error);
        }))
    }
}
