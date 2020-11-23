using System.Collections.Generic;
using Kztek_Model.Models.WM;

namespace Kztek_Library.Models
{
    public class WM_TaskSubmit
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public int Status { get; set; } // 0 - Đang tiến hành, 1 - Hoàn thành, 2 - Tạm dừng

        public List<string> UserIds { get; set; }

        public List<WM_TaskUser> UserTasks { get; set; }
    }
}