using System;
using System.Collections.Generic;
using Kztek_Model.Models.PM;

namespace Kztek_Library.Models
{
    public class PM_ComponentChild
    {
        public List<PM_Component> Data_All { get; set; }

        public List<PM_Component> Data_Child { get; set; }

        public int MarginLeft { get; set; } = 0;

        public string UserId { get; set; } = "";

        public string ProjectId { get; set; } = "";
    }
}
