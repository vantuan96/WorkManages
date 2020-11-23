using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.WM
{
    public class WM_Diary
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ScheduleId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }

        public string UserId { get; set; }
    }
}