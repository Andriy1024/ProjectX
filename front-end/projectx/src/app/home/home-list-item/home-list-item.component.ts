import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-home-list-item',
  templateUrl: './home-list-item.component.html',
  styleUrls: ['./home-list-item.component.scss']
})
export class HomeListItemComponent implements OnInit {

  @Input()
  public title: string = '';

  @Input()
  public toggleValue: boolean = false;

  constructor() { }

  public ngOnInit(): void {
  }

  public toggle(): void {
      this.toggleValue = !this.toggleValue;
  }

}
