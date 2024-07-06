using System;
using System.Collections.Generic;
using System.Text;

namespace AgentObserver.Models.External
{
    public class UserStatusInputDto
    {
        public string EmpId { get; set; }
        public DateTime RecordDateTime { get; set; }
        public string Key { get; set; }
        public bool InForCheckin { get; set; }
    }
}
