using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.PM
{
    [BsonIgnoreExtraElements]
    public class PM_Work
    {
        public string Id { get; set; }

        public string ProjectId { get; set; } = "";

        public string ComponentId { get; set; } = "0";

        public string UserId { get; set; } = "";

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateRealStart { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCompleted { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsOnScheduled { get; set; } = false;
    }
}
