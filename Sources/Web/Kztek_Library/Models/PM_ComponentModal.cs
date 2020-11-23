using System;
using Kztek_Model.Models.PM;

namespace Kztek_Library.Models
{
    public class PM_ComponentModal
    {
        public SelectListModel_Chosen Select_ProjectStatus { get; set; }

        public SelectListModel_Chosen Select_ProjectComponents { get; set; }

        public AJAXModel_Modal Data_Modal { get; set; }

        public PM_Component Data { get; set; }
    }
}
