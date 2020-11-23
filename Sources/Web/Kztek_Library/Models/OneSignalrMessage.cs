using System;
namespace Kztek_Library.Models
{
    public class OneSignalrMessage
    {
        public string Id { get; set; } = "";

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string View { get; set; } = ""; //Dùng cho mobile để link về nội dung của message

        public string[] PlayerIds { get; set; }

        public string UserIds { get; set; } = "";
    }
}
