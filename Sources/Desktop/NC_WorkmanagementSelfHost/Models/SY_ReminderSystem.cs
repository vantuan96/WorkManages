using System;
using System.Collections.Generic;
using System.Text;

namespace NC_WorkmanagementSelfHost.Models
{
    public class SY_ReminderSystem
    {
        public string Id { get; set; } // == RecordId. id của bản ghi cần reminder

        public string Type { get; set; } // project, task....

        public DateTime DateReminder { get; set; }

        public bool isDone { get; set; }
    }
}
