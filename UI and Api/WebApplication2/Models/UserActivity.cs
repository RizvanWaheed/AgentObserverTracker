using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgentObserverWeb.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime ActivityTime { get; set; }
        public string PressedKey { get; set; }
        public string ActivityType { get; set; }
        public bool NetWorkActivity { get; set; }
    }
    public class UserAvailablitiy
    {
        [Key]
        public int Id { get; set; }
        public DateTime CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public DateTime? BreakStartTime { get; set; }
        public DateTime? BreakEndTime { get; set; }
        public bool? IsBreak { get; set; }
        public string UserNTLogin { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime AvailabilityTime { get; set; }
    }
    public class UserBreaks
    {
        [Key]
        public int Id { get; set; }
        public DateTime BreakStartTime { get; set; }
        public DateTime? BreakEndTime { get; set; }
        public bool? IsBreak { get; set; }
        public string UserNTLogin { get; set; }
        public string EmpId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
    public class ActivityUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        public string TeamName { get; set; }
        public string ManagerOrSupervisorName { get; set; }
        public string Location { get; set; }
    }

    public class UserActivityDto
    {
        public string UserName { get; set; }
        public string ActivityTime { get; set; }
        public string PressedKey { get; set; }
        public string ActivityType { get; set; }
        public bool NetWorkActivity { get; set; }
    }
    public class UserActivityReturnDto
    {
        public string UserName { get; set; }
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
    public class KeyValuePair
    {
        public string Time { get; set; }
        public int Hour { get; set; }
        public int DefaultHourFormat { get; set; }
    }
}