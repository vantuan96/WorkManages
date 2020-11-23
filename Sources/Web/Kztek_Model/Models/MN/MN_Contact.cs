using System;
namespace Kztek_Model.Models.MN
{
    public class MN_Contact
    {
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public string Value { get; set; }

        public int ContactType { get; set; } // 0 - Phone, 1 - Email.....
    }
}
