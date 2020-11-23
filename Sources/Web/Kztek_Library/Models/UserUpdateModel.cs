using System;
using Microsoft.AspNetCore.Http;

namespace Kztek_Library.Models
{
    public class UserUpdateModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public IFormFile FileUpload { get; set; }
    }
}
