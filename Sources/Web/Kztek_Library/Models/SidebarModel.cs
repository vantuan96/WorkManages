using System.Collections.Generic;
using Kztek_Model.Models;

namespace Kztek_Library.Models
{
    public class SidebarModel
    {
        public string Id { get; set; } = "";

        public string ControllerName { get; set; } = "";

        public string ActionName { get; set; } = "";

        public List<SY_MenuFunction> Data { get; set; } = new List<SY_MenuFunction>();

        public List<SY_MenuFunction> Data_Child { get; set; } = new List<SY_MenuFunction>();

        public SY_MenuFunction CurrentView { get; set; } = null;

        public string Breadcrumb { get; set; } = "";
    }
}