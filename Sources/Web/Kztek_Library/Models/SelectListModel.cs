using System.Collections.Generic;

namespace Kztek_Library.Models
{
    public class SelectListModel
    {
        public string ItemValue { get; set; }

        public string ItemText { get; set; }
    }

    public class SelectListModel_Breadcrumb
    {
        public string MenuName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public bool isFolder { get; set; }
    }

    public class SelectListModel_Print_Header
    {
        public string ItemText { get; set; }
    }

    public class SelectListModel_Print_Column_Header
    {
        public string ItemText { get; set; }
    }

    public class SelectListModel_Chosen
    {
        public string IdSelectList { get; set; }

        public List<SelectListModel> Data { get; set; }

        public string Selecteds { get; set; }

        public bool isMultiSelect { get; set; }

        public string Placeholder { get; set; }
    }

    public class SelectListModel_Multi
    {
        public string IdSelectList { get; set; }

        public List<SelectListModel> Data { get; set; }

        public string Selecteds { get; set; }

        public string Placeholder { get; set; }
    }
}