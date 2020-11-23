using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.PM
{
    [BsonIgnoreExtraElements]
    public class PM_TeamMember
    {
        public string Id { get; set; }

        public string ProjectId { get; set; } = "";

        public string UserId { get; set; } = "";

        public string CustomerId { get; set; } = "";
    }
}
