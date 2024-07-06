using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgentObserver.Observer
{
    public class UserBreaks : Entity
    {
        public DateTime BreakStartTime { get; set; }
        public DateTime? BreakEndTime { get; set; }
        public bool? IsBreak { get; set; }
        public string UserNTLogin { get; set; }
        public string EmpId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
