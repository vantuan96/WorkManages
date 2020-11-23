using System;
namespace Kztek_Model.Models.MN
{
    public class MN_Customer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public string CustomerGroupId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
