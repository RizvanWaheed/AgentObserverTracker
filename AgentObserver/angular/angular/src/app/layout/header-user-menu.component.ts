import { Component, ChangeDetectionStrategy, Injector } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { AppComponentBase } from 'shared/app-component-base';

@Component({
  selector: 'header-user-menu',
  templateUrl: './header-user-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderUserMenuComponent extends AppComponentBase {
  shownLoginName = "";
  constructor(private _authService: AppAuthService, injector: Injector) {
    super(injector);
  }
  ngOnInit(): void {
    this.shownLoginName = this.appSession.user.name;
  }
  logout(): void {
    this._authService.logout();
  }
}
