namespace Kztek_Library.Models
{
    public class WM_TaskCustomView
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public int Status { get; set; } // 0 - Đang tiến hành, 1 - Hoàn thành, 2 - Tạm dừng
    }
}