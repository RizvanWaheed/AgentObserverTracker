import {
  Component,
  OnInit,
  ViewEncapsulation,
  Injector,
  Renderer2
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { LoginComponent } from './login/login.component';
@Component({
  templateUrl: './account.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AccountComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private renderer: Renderer2,private _modalService: BsModalService) {
    super(injector);
  }

  showTenantChange(): boolean {
    return abp.multiTenancy.isEnabled;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, 'login-page');
  }
  openLoginModel(){
    this.showloginUserDialog();
  }
  private showloginUserDialog(): void {
    let loginUserDialog: BsModalRef;
    loginUserDialog = this._modalService.show(
      LoginComponent,
      {
        class: 'modal-md'
      }
    );
  }
}
