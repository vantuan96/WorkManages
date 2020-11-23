using System;
namespace Kztek_Library.Models
{
    public class OS_MessageModal
    {
        public SelectListModel_Chosen Select_Users { get; set; }

        public AJAXModel_Modal Data_Modal { get; set; }

        public OneSignalrMessage Data { get; set; }
    }
}
