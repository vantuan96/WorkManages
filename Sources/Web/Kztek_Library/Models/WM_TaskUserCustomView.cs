namespace Kztek_Library.Models
{
    public class WM_TaskUserCustomView
    {
        public string Username { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsOnScheduled { get; set; } = false;
    }
}