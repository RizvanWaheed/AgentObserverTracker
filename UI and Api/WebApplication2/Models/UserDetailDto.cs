using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgentObserverWeb.Models
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
}