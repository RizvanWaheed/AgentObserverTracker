using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgentObserver.Observer
{
    public class UserAvailablitiy : Entity
    {
        //[Key]
        //public int Id { get; set; }
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
}
