using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.MN
{
    public class MN_KeyCard
    {
        public string Id { get; set; }

        public string CardNo { get; set; }

        public string CardNumber { get; set; }

        public string KeySecurityId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }
    }
}
