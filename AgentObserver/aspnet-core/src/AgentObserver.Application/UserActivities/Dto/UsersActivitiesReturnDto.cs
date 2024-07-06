using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgentObserver.Users.Dto
{
    public class UserDetailDto
    {
        public string id { get; set; }
        public string EmpId { get; set; }
        public string text { get; set; }
    }
    public class KeyValueDto
    {
        public string id { get; set; }
        public string text { get; set; }
    }
    public class UserActivityDto
    {
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("ActivityTime")]
        public string ActivityTime { get; set; }
        [JsonProperty("PressedKey")]
        public string PressedKey { get; set; }
        [JsonProperty("ActivityType")]
        public string ActivityType { get; set; }
        [JsonProperty("NetWorkActivity")]
        public bool NetWorkActivity { get; set; }
    }
    public class UserActivityReturnDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmpId { get; set; }
        public string ActivityDateTime { get; set; }
        public string LastUpdateTime { get; set; }
        public double TotalInActiveTimeInMinutes { get; set; }
        public double TotalActiveTimeInMinutes { get; set; }
        public string TotalInActiveTime { get; set; }
        public string TotalActiveTime { get; set; }
        public string CheckinTime { get; set; }
        public string CheckoutTime { get; set; }
        public List<HourBaseActivity> HourlyActivities { get; set; }
        public string ShiftHours { get; set; }
        public string BreakTime { get; set; }
        public string BreakDuration { get; set; }
        public bool? IsBreak { get; set; }
    }
    public class UsersActivitiesReturnDto
    {
        public bool IsRoleManager { get; set; }
        public bool IsManagerOrSuperVisor { get; set; }
        public DateTime? Datetime { get; set; }
        public List<UserActivityReturnDto> Activities { get; set; }
        public string AppUsers { get; set; }
        public string ManagerOrSuperVisor { get; set; }
        public string Department { get; set; }
        public string Team { get; set; }
    }
    public class HourBaseActivity
    {
        public string TimeOfDay { get; set; }
        public int InActiveDuration { get; set; }
        //public string MinInActiveDuration { get; set; }
        public int ActiveDuration { get; set; }
        public bool IsNetworkAvailable { get; set; }
        public int MaxContinousInActiveDuration { get; set; }
        public int MinInActiveDuration { get; set; }
    }
    public class HourKeyValuePair
    {
        public string Time { get; set; }
        public int Hour { get; set; }
        public int DefaultHourFormat { get; set; }
    }
    public class ExecutiveStat
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
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

    public class CombineStats
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

    public class IndividualStats
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

    public class ExecutiveStatDto
    {
        public string Message { get; set; }
        public bool Status { get; set; }

        public DataDto Data { get; set; }
    }

    public partial class DataDto
    {
        public IndividualStatsDto IndividualStats { get; set; }
        public CombineStatsDto CombineStats { get; set; }
        public long Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string LogKey { get; set; }
    }

    public partial class CombineStatsDto
    {
        public long TeamSchedule { get; set; }
        public long Present { get; set; }
        public long Absent { get; set; }
        public long WorkAtHome { get; set; }
        public long LateArrivals { get; set; }
        public long EarlyLeavers { get; set; }
    }

    public partial class IndividualStatsDto
    {
        public string ShiftFrom { get; set; }
        public string ShiftTo { get; set; }
        public long IsScheduled { get; set; }
        public object LeaveCategory { get; set; }
        public object LeaveComments { get; set; }
        public string ShiftTiming { get; set; }
        public DateTime? DateSchedule { get; set; }
        public object AttId { get; set; }
        public object CheckIn { get; set; }
        public object CheckOut { get; set; }
        public object ShiftDate { get; set; }
        public object ShiftFromDate { get; set; }
        public object ShiftToDate { get; set; }
        public object ShiftLegendType { get; set; }
        public object UserArrival { get; set; }
        public object UserDeparture { get; set; }
        public object NoOfBreaks { get; set; }
        public object ShiftTimeSpent { get; set; }
        public object TotalTimeSpent { get; set; }
        public object ShiftShortTime { get; set; }
        public object TotalShortTime { get; set; }
        public object ExtraTimeSpent { get; set; }
        public object ShortHours { get; set; }
        public object OnBreak { get; set; }
        public object AutoLogout { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string ClientName { get; set; }
        public string SubClientName { get; set; }
    }
}
