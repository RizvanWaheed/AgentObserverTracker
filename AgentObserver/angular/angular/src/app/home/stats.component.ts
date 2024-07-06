import { Component, Injector, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import { UserActivityReturnDto, UsersActivitiesReturnDto, ExecutiveStatDto, UserActivityServiceProxy, DataDto } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';
import {MatDatepickerInputEvent} from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { FormControl } from '@angular/forms';
import * as moment from 'moment';
@Component({
  templateUrl: './stats.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.Default
})
export class StatsComponent extends AppComponentBase {
  ActivityDto: UserActivityReturnDto;
  ActivityWithDetailsDto: UsersActivitiesReturnDto;
  StatsDto: DataDto;
  totalSpendTime = "00:00";
  queryDate = new Date();
  recordDate = new FormControl(moment(this.queryDate).format())
  totalExtraTime = "00:00";
  statsUserName = "";
  constructor(
    public _userActivityService: UserActivityServiceProxy,
    private route: ActivatedRoute,
    injector: Injector
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.route.queryParams.subscribe( (params) => 
          {
            this.statsUserName = params.username;
            this.GetUserStats(params.username,this.queryDate);
          });
  }
  addEvent(type: string, event: MatDatepickerInputEvent<Date>) {
    debugger;
    this.queryDate = event.value;
    this.GetUserStats(this.statsUserName,this.queryDate);
  }
  GetUserStats(username,date) {
    abp.ui.setBusy();
    this._userActivityService.getUserStats(date , '', username, false)
    .pipe(
      finalize(() => {
        abp.ui.clearBusy();
      })
    )
    .subscribe((result) => {
          let momentDate = moment(date).toDate();  //moment().format()
          this.recordDate = new FormControl(momentDate)
          console.log(result);
          this.ActivityDto = new UserActivityReturnDto();
          this.ActivityDto = result.activities[0];
        });
  }
  
}
