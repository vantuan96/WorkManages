using System.Collections.Generic;
using Kztek_Library.Models;

namespace Kztek_Library.Helpers
{
    public class StaticList
    {
        /// <summary>
        /// Danh sách loại menu
        /// </summary>
        /// <returns>List<SelectListModel></returns>
        public static List<SelectListModel> MenuType()
        {
            var list = new List<SelectListModel> {
                                        new SelectListModel { ItemValue = "1", ItemText = "Menu"},
                                        new SelectListModel { ItemValue = "2", ItemText = "Function"}
                                    };
            return list;
        }

        /// <summary>
        /// Danh sách trạng thái project
        /// </summary>
        /// <returns>List<SelectListModel></returns>
        public static List<SelectListModel> ProjectStatus()
        {
            var list = new List<SelectListModel> {
                                        new SelectListModel { ItemValue = "0", ItemText = "Đang tiến hành"},
                                        new SelectListModel { ItemValue = "1", ItemText = "Hoàn thành"},
                                        new SelectListModel { ItemValue = "2", ItemText = "Tạm dừng"}
                                    };
            return list;
        }

        /// <summary>
        /// Danh sách loại contact
        /// </summary>
        /// <returns>List<SelectListModel></returns>
        public static List<SelectListModel> ContactType()
        {
            var list = new List<SelectListModel> {
                                        new SelectListModel { ItemValue = "0", ItemText = "Phone"},
                                        new SelectListModel { ItemValue = "1", ItemText = "Email"},
                                    };
            return list;
        }
    }
}