using System;
namespace Kztek_Model.Models.MN
{
    public class MN_License
    {
        public string Id { get; set; }

        public string ProjectName { get; set; }

        public bool IsExpire { get; set; }

        public string ExpireDate { get; set; }
    }
}
