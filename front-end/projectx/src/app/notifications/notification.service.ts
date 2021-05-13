import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { NotificationType, Notification } from "./notification";

@Injectable({ providedIn: 'root' })
export class NotificationService {

    private _subject = new Subject<Notification>();
    private _idx = 0;

    constructor() {}

    public getObservable(): Observable<Notification> {
        return this._subject.asObservable();
    }

    public info(title: string, message: string, timeout = 3000): void {
        this.next(NotificationType.info, title, message, timeout);
    }

    public success(title: string, message: string, timeout = 3000): void {
        this.next(NotificationType.success, title, message, timeout);
    }

    public warning(title: string, message: string, timeout = 3000): void {
        this.next(NotificationType.warning, title, message, timeout);
    }

    public error(title: string, message: string, timeout = 3000): void {
        this.next(NotificationType.error, title, message, timeout);
    }

    private next(type: NotificationType, title: string, message: string, timeout: number): void {
        this._subject.next(new Notification(
            this._idx++,
            type,
            title,
            message,
            timeout));
    }
}
