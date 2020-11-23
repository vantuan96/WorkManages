using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Kztek_Model.Models.PM
{
    [BsonIgnoreExtraElements]
    public class PM_Component
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateStart { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateEnd { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }

        public int Status { get; set; } // 0 - Đang tiến hành, 1 - Hoàn thành, 2 - Tạm dừng

        public string ProjectId { get; set; } = "";

        public string ParentId { get; set; } = "0";

        public string Label { get; set; } = "";
    }

    public class PM_Component_Submit
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DateCreated { get; set; }

        public int Status { get; set; } // 0 - Đang tiến hành, 1 - Hoàn thành, 2 - Tạm dừng

        public string ProjectId { get; set; } = "";

        public string ParentId { get; set; } = "0";

        public string Label { get; set; } = "";
    }
}
