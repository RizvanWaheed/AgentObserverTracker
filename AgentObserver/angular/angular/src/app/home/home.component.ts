import { Component, Injector, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from 'rxjs/operators';
import { UserActivityReturnDto, UsersActivitiesReturnDto, ExecutiveStatDto, UserActivityServiceProxy, DataDto } from '@shared/service-proxies/service-proxies';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { FormControl } from '@angular/forms';
// import {MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS} from '@angular/material-moment-adapter';
// import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from '@angular/material/core';
// import * as _moment from 'moment';
import * as moment from 'moment';
// tslint:disable-next-line:no-duplicate-imports

// export const MY_FORMATS = {
//   parse: {
//     dateInput: 'LL',
//   },
//   display: {
//     dateInput: 'LL',
//     monthYearLabel: 'MMM YYYY',
//     dateA11yLabel: 'LL',
//     monthYearA11yLabel: 'MMMM YYYY',
//   },
// };

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.Default,
  selector: 'home-component'
})
export class HomeComponent extends AppComponentBase {
  ActivityDto: UserActivityReturnDto;
  ActivityWithDetailsDto: UsersActivitiesReturnDto;
  StatsDto: DataDto;
  showManagerDashboard = false;
  totalCheckins = 0;
  allUserActiveTime = "00:00";
  allUserInActiveTime = "00:00";
  allUserBreakTime = "00:00";
  totalSpendTime= "00:00";
  totalExtraTime= "00:00";
  queryDate = new Date();
  recordDate = new FormControl(moment(this.queryDate).format())
  constructor(
    public _userActivityService: UserActivityServiceProxy,
    injector: Injector
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.GetUserStats(this.queryDate);
    this.GetUserExternalStats();
  }
  addEvent(type: string, event: MatDatepickerInputEvent<Date>) {
    debugger;
    this.queryDate = event.value;
    this.GetUserStats(this.queryDate);
  }
  GetUserStats(date) {
    debugger;
    this.showManagerDashboard = !this.showManagerDashboard;
    abp.ui.setBusy();
    this._userActivityService.getUserStats(date , '', '', this.showManagerDashboard)
    .pipe(
      finalize(() => {
        abp.ui.clearBusy();
      })
    )
    .subscribe((result) => {
          let momentDate = moment(date).toDate();  //moment().format()
          this.recordDate = new FormControl(momentDate)
          this.totalCheckins = 0;
          this.ActivityDto = new UserActivityReturnDto();
          this.ActivityWithDetailsDto = new UsersActivitiesReturnDto();          
          this.ActivityWithDetailsDto = result;
          this.ActivityDto = result.activities[0];
          let totalActiveTime = 0;
          let totalinActiveTime = 0;
          let totalBreak = 0;
          debugger;
          this.ActivityWithDetailsDto.activities.forEach(element => {
            debugger;
            totalActiveTime += element.totalActiveTimeInMinutes;
            totalinActiveTime += element.totalInActiveTimeInMinutes;
            if (element.checkinTime != null){
              this.totalCheckins++;
            }
          });
          this.allUserActiveTime = this.getMinsinTime(totalActiveTime);
          this.allUserInActiveTime = this.getMinsinTime(totalinActiveTime);
        });
  }
  getSingleUserDetail(username){
    
  }
  getSecondsinTime(seconds){
    const hour = Math.floor(Math.abs(seconds) / 3600) > 9 ? Math.floor(Math.abs(seconds) / 3600) : Math.floor(Math.abs(seconds) / 3600);
    const min = Math.floor((Math.abs(seconds) / 60) % 60) > 9 ? Math.floor((Math.abs(seconds) / 60) % 60) : '00';
    return hour + ':' + min;
  }
  getMinsinTime(mins){
    const hour = Math.floor(Math.abs(mins) / 60) > 9 ? Math.floor(Math.abs(mins) / 60) : Math.floor(Math.abs(mins) / 60);
    const min = Math.floor((Math.abs(mins)) % 60) > 9 ? Math.floor((Math.abs(mins)) % 60) : '00';
    return hour + ':' + min;
  }
  GetUserExternalStats() {
    abp.ui.setBusy();
    this._userActivityService.getExternalStats(abp.utils.getCookieValue(AppConsts.authorization.externalToken))
      .pipe(
        finalize(() => {
          abp.ui.clearBusy();
        })
      )
      .subscribe((result) => {
        console.log(result);
        this.StatsDto = result.data;
        this.totalSpendTime = this.getSecondsinTime(this.StatsDto.individualStats.totalTimeSpent);
        this.totalExtraTime = this.getSecondsinTime(this.StatsDto.individualStats.extraTimeSpent);
      });
  }
}
