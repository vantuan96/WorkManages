using System;
using System.Collections.Generic;

namespace Kztek_Library.Models
{
    public class Chart_Performance_Monthly
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public List<Chart_Performance_Monthly_Detail> Details { get; set; }
    }

    public class Chart_Performance_Monthly_Detail
    {
        public string Month { get; set; }

        public int Project_Total { get; set; }

        public int Project_Completed { get; set; }

        public int Project_Completed_onTime { get; set; }

        public int Project_Completed_notOnTime { get; set; }

        public int Task_Total { get; set; }

        public int Task_Completed { get; set; }

        public int Task_Completed_onTime { get; set; }

        public int Task_Completed_notOnTime { get; set; }
    }

}
