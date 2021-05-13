import { Component } from '@angular/core';
import { NotificationService } from "./notification.service";
import { Notification, NotificationType } from "./notification";
import { Subscription } from "rxjs";

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationListComponent {

    public notifications: Notification[] = [
      new Notification(1, NotificationType.info, 'title', 'message', 300),
      new Notification(2, NotificationType.info, 'title', 'message', 300),
      new Notification(3, NotificationType.info, 'title', 'message', 300)
    ];

    private _subscription!: Subscription;

    constructor(private _notificationSvc: NotificationService) { }

    private _addNotification(notification: Notification) {
        this.notifications.push(notification);

        if (notification.timeout !== 0)
        {
          setTimeout(() => this.close(notification), notification.timeout);
        }
    }

    public ngOnInit(): void {
        this._subscription = this._notificationSvc.getObservable().subscribe(notification => this._addNotification(notification));
    }

    public ngOnDestroy(): void {
        this._subscription.unsubscribe();
    }

    public close(notification: Notification): void {
        this.notifications = this.notifications.filter(notif => notif.id !== notification.id);
    }

    public className(notification: Notification): string {

        let style: string;

        switch (notification.type) {

        case NotificationType.success:
            style = 'success';
            break;

        case NotificationType.warning:
            style = 'warning';
            break;

        case NotificationType.error:
            style = 'error';
            break;

        default:
            style = 'info';
            break;
        }

        return style;
    }
}