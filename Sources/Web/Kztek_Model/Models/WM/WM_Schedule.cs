using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.WM
{
    public class WM_Schedule
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateStart { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateEnd { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }
    }

    public class WM_Schedule_Submit
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }

    }
}