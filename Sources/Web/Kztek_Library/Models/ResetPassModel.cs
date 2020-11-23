using System;
namespace Kztek_Library.Models
{
    public class ResetPassModel
    {
        public string UserId { get; set; }

        public string OldPass { get; set; }

        public string NewPass { get; set; }
    }
}
