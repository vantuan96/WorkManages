using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models
{
    public class SY_Notification
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }
    }
}
