using System;
namespace Kztek_Library.Models
{
    public class UserForgotModel
    {
        public string Email { get; set; }

        public string Code { get; set; }

        public string NewPass { get; set; }
    }
}
