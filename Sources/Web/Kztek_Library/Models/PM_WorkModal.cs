using System;
using System.Collections.Generic;
using Kztek_Model.Models.PM;

namespace Kztek_Library.Models
{
    public class PM_WorkModal
    {
        public SelectListModel_Chosen Select_User { get; set; }

        public AJAXModel_Modal Data_Modal { get; set; }

        public List<PM_Work> Data { get; set; }
    }
}
