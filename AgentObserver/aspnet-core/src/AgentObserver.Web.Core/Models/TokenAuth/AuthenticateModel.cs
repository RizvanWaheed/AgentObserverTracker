using System;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Newtonsoft.Json;

namespace AgentObserver.Models.TokenAuth
{
    public class AuthenticateModel
    {
        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        public string Password { get; set; }
        
        public bool RememberClient { get; set; }
    }
    public class RegisterationViaApiModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PlainPass { get; set; }
        public string Email { get; set; }
    }
   

    public class UserDetailObjWithInternalDetails
    {
        public RegisterationViaApiModel UserInternalDetails { get; set; }
        public ExternalAuthModel UserExternalDetails { get; set; }
    }
    public class ExternalLoginModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("data")]
        public UserStats Data { get; set; }
    } 
    public class UserStats
    {
        [JsonProperty("present_days")]
        public long PresentDays { get; set; }

        [JsonProperty("present_scheduled_users")]
        public long PresentScheduledUsers { get; set; }

        [JsonProperty("present_non_scheduled_users")]
        public long PresentNonScheduledUsers { get; set; }

        [JsonProperty("present_leave_scheduled_users")]
        public long PresentLeaveScheduledUsers { get; set; }

        [JsonProperty("present_offsite_scheduled_users")]
        public long PresentOffsiteScheduledUsers { get; set; }

        [JsonProperty("in_time_users")]
        public long InTimeUsers { get; set; }

        [JsonProperty("tardy_days")]
        public long TardyDays { get; set; }

        [JsonProperty("early_leavers")]
        public long EarlyLeavers { get; set; }

        [JsonProperty("late_leavers")]
        public long LateLeavers { get; set; }

        [JsonProperty("working_time")]
        public long WorkingTime { get; set; }

        [JsonProperty("non_working_time")]
        public long NonWorkingTime { get; set; }

        [JsonProperty("spent_time")]
        public long SpentTime { get; set; }

        [JsonProperty("extra_time")]
        public long ExtraTime { get; set; }

        [JsonProperty("short_time")]
        public long ShortTime { get; set; }

        [JsonProperty("auto_logout_per_day")]
        public long AutoLogoutPerDay { get; set; }

        [JsonProperty("on_floor_users")]
        public long OnFloorUsers { get; set; }

        [JsonProperty("scheduled_users")]
        public long ScheduledUsers { get; set; }

        [JsonProperty("absent_days")]
        public long AbsentDays { get; set; }

        [JsonProperty("absent_leave_days")]
        public long AbsentLeaveDays { get; set; }

        [JsonProperty("leave_days")]
        public string LeaveDays { get; set; }

        [JsonProperty("total_users")]
        public long TotalUsers { get; set; }

        [JsonProperty("working_hours")]
        public string WorkingHours { get; set; }

        [JsonProperty("non_working_hours")]
        public string NonWorkingHours { get; set; }

        [JsonProperty("spent_hours")]
        public string SpentHours { get; set; }

        [JsonProperty("extra_hours")]
        public long ExtraHours { get; set; }

        [JsonProperty("short_hours")]
        public string ShortHours { get; set; }

        [JsonProperty("present_perc")]
        public long PresentPerc { get; set; }

        [JsonProperty("absent_perc")]
        public long AbsentPerc { get; set; }

        [JsonProperty("intime_perc")]
        public long IntimePerc { get; set; }

        [JsonProperty("tardy_perc")]
        public long TardyPerc { get; set; }

        [JsonProperty("present_users")]
        public long PresentUsers { get; set; }

        [JsonProperty("absent_users")]
        public long AbsentUsers { get; set; }

        [JsonProperty("outOfOfficeWithoutAtt")]
        public long OutOfOfficeWithoutAtt { get; set; }

        [JsonProperty("outOfOfficeWithAtt")]
        public long OutOfOfficeWithAtt { get; set; }

        [JsonProperty("month_to_date_absent_perc")]
        public long MonthToDateAbsentPerc { get; set; }

        [JsonProperty("wah_users")]
        public long WahUsers { get; set; }

        [JsonProperty("out_of_office_hour")]
        public long OutOfOfficeHour { get; set; }

        [JsonProperty("OOO_hours")]
        public string OooHours { get; set; }

        [JsonProperty("absent_leave_perc")]
        public long AbsentLeavePerc { get; set; }

        [JsonProperty("early_leavers_perc")]
        public long EarlyLeaversPerc { get; set; }

        [JsonProperty("user_absent_days")]
        public long UserAbsentDays { get; set; }

        [JsonProperty("user_remaining_leave_days")]
        public long UserRemainingLeaveDays { get; set; }

        [JsonProperty("user_absent_leave_days")]
        public long UserAbsentLeaveDays { get; set; }

        [JsonProperty("swap_on")]
        public long SwapOn { get; set; }

        [JsonProperty("swap_off")]
        public long SwapOff { get; set; }

        [JsonProperty("total_swap")]
        public long TotalSwap { get; set; }

        [JsonProperty("out_of_office")]
        public long OutOfOffice { get; set; }

        [JsonProperty("log_key")]
        public string LogKey { get; set; }
    }
    
    public partial class ExternalAuthModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("individual_stats")]
        public IndividualStats IndividualStats { get; set; }

        [JsonProperty("combine_stats")]
        public CombineStats CombineStats { get; set; }

        [JsonProperty("username")]
        public long Username { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("log_key")]
        public string LogKey { get; set; }
    }

    public partial class CombineStats
    {
        [JsonProperty("team_schedule")]
        public long TeamSchedule { get; set; }

        [JsonProperty("present")]
        public long Present { get; set; }

        [JsonProperty("absent")]
        public long Absent { get; set; }

        [JsonProperty("work_at_home")]
        public long WorkAtHome { get; set; }

        [JsonProperty("late_arrivals")]
        public long LateArrivals { get; set; }

        [JsonProperty("early_leavers")]
        public long EarlyLeavers { get; set; }
    }

    public partial class IndividualStats
    {
        [JsonProperty("shift_from")]
        public string ShiftFrom { get; set; }

        [JsonProperty("shift_to")]
        public string ShiftTo { get; set; }

        [JsonProperty("is_scheduled")]
        public long IsScheduled { get; set; }

        [JsonProperty("leave_category")]
        public object LeaveCategory { get; set; }

        [JsonProperty("leave_comments")]
        public object LeaveComments { get; set; }

        [JsonProperty("shift_timing")]
        public string ShiftTiming { get; set; }

        [JsonProperty("date_schedule")]
        public DateTime? DateSchedule { get; set; }

        [JsonProperty("att_id")]
        public object AttId { get; set; }

        [JsonProperty("check_in")]
        public object CheckIn { get; set; }

        [JsonProperty("check_out")]
        public object CheckOut { get; set; }

        [JsonProperty("shift_date")]
        public object ShiftDate { get; set; }

        [JsonProperty("shift_from_date")]
        public object ShiftFromDate { get; set; }

        [JsonProperty("shift_to_date")]
        public object ShiftToDate { get; set; }

        [JsonProperty("shift_legend_type")]
        public object ShiftLegendType { get; set; }

        [JsonProperty("user_arrival")]
        public object UserArrival { get; set; }

        [JsonProperty("user_departure")]
        public object UserDeparture { get; set; }

        [JsonProperty("no_of_breaks")]
        public object NoOfBreaks { get; set; }

        [JsonProperty("shift_time_spent")]
        public object ShiftTimeSpent { get; set; }

        [JsonProperty("total_time_spent")]
        public object TotalTimeSpent { get; set; }

        [JsonProperty("shift_short_time")]
        public object ShiftShortTime { get; set; }

        [JsonProperty("total_short_time")]
        public object TotalShortTime { get; set; }

        [JsonProperty("extra_time_spent")]
        public object ExtraTimeSpent { get; set; }

        [JsonProperty("short_hours")]
        public object ShortHours { get; set; }

        [JsonProperty("on_break")]
        public object OnBreak { get; set; }

        [JsonProperty("auto_logout")]
        public object AutoLogout { get; set; }

        [JsonProperty("location_name")]
        public string LocationName { get; set; }

        [JsonProperty("department_name")]
        public string DepartmentName { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("sub_client_name")]
        public string SubClientName { get; set; }
    }
}
