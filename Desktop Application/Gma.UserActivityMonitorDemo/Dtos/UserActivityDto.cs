using System;
using System.Collections.Generic;
using System.Text;

namespace Ibex.UserActivityMonitorApp.Dtos
{
    public class UserActivityDto
    {
        public string UserName { get; set; }
        public string ActivityTime { get; set; }
        public string PressedKey { get; set; }
        public string ActivityType { get; set; }
        public bool NetWorkActivity { get; set; }
    }
    public class UserAvailablitiyDto
    {
        public int Id { get; set; }
        public DateTime CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public string UserNTLogin { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime AvailabilityTime { get; set; }
        public DateTime? BreakStartTime { get; set; }
        public DateTime? BreakEndTime { get; set; }
        public bool? IsBreak { get; set; }
    }
}
