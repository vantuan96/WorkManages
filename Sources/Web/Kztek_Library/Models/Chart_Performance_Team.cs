using System;
namespace Kztek_Library.Models
{
    public class Chart_Performance_Team
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public Chart_Performance_Team_Detail Percent { get; set; }
    }

    public class Chart_Performance_Team_Detail
    {
        public double Project_Percent { get; set; }

        public double Task_Percent { get; set; }
    }
}
