using System;
using System.Collections.Generic;

namespace Kztek_Library.Models
{
    public class WM_ScheduleMobile
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public List<WM_DiaryMobile> Diaries { get; set; }
    }

    public class WM_DiaryMobile
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ScheduleId { get; set; }

        public string UserId { get; set; }
    }
}
