using System;
namespace Kztek_Library.Models
{
    public class Chart_Performance_Personal
    {
        public int Project_Total { get; set; }

        public int Project_Completed_onTime { get; set; }

        public int Project_Completed_notOnTime { get; set; }

        public int Project_NotComplete { get; set; }

        public int Task_Total { get; set; }

        public int Task_Completed_onTime { get; set; }

        public int Task_Completed_notOnTime { get; set; }

        public int Task_NotComplete { get; set; }

        public Chart_Performance_Personal_Pie ProjectStatus { get; set; }

        public Chart_Performance_Personal_Pie TaskStatus { get; set; }
    }

    public class Chart_Performance_Personal_Pie
    {
        public double OnTime { get; set; }

        public double Late { get; set; }

        public double Doing { get; set; }
    }
}
