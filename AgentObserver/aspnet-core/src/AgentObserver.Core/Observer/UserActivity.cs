using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgentObserver.Observer
{
    public class UserActivity : Entity 
    {
        //[Key]
        //public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime ActivityTime { get; set; }
        public string PressedKey { get; set; }
        public string ActivityType { get; set; }
        public bool NetWorkActivity { get; set; }
    }
}
