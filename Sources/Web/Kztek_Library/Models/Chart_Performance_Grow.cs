using System;
using System.Collections.Generic;

namespace Kztek_Library.Models
{
    public class Chart_Performance_Grow
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public List<Chart_Performance_Grow_Detail> Details { get; set; }
    }

    public class Chart_Performance_Grow_Detail
    {
        public string Month { get; set; }

        public double Project_CurrentPercent { get; set; }

        public double Project_GrowPercent { get; set; }

        public double Task_CurrentPercent { get; set; }

        public double Task_GrowPercent { get; set; }
    }

    public class Chart_Performance_Grow_Personal
    {
        public double Project_CurrentPercent { get; set; }

        public double Project_GrowPercent { get; set; }

        public double Task_CurrentPercent { get; set; }

        public double Task_GrowPercent { get; set; }
    }
}
