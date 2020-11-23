using System;
using System.Collections.Generic;
using Kztek_Model.Models.MN;

namespace Kztek_Library.Models
{
    public class MN_CustomerDetailCustomView
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public string CustomerGroupName { get; set; }

        public List<MN_Contact> Contacts { get; set; }
    }
}
