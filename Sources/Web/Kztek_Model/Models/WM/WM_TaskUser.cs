using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.WM
{
    public class WM_TaskUser
    {
        public string Id { get; set; }

        public string TaskId { get; set; }

        public string UserId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCompleted { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsOnScheduled { get; set; } = false;
    }
}
