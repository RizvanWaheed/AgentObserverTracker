using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgentObserver.Observer
{
    public class ActivityUser : Entity
    {
        //[Key]
        //public int Id { get; set; }
        public string UserName { get; set; }
        public string EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        public string TeamName { get; set; }
        public string ManagerOrSupervisorName { get; set; }
        public string Location { get; set; }
    }
}
